using Bitfinex.Net.Converters;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CryptoExchange.Net.Authentication;
using Bitfinex.Net.Enums;
using System.Threading;
using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.Socket;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using Bitfinex.Net.Objects.Options;

namespace Bitfinex.Net.Clients.SpotApi
{
    /// <inheritdoc cref="IBitfinexSocketClientSpotApi" />
    public class BitfinexSocketClientSpotApi : SocketApiClient, IBitfinexSocketClientSpotApi
    {
        #region fields
        private readonly JsonSerializer _bookSerializer = new JsonSerializer();
        private readonly Random _random = new Random();
        private readonly string? _affCode;

        /// <inheritdoc />
        public new BitfinexSocketOptions ClientOptions => (BitfinexSocketOptions)base.ClientOptions;
        #endregion

        #region ctor
        internal BitfinexSocketClientSpotApi(ILogger logger, BitfinexSocketOptions options) :
            base(logger, options.Environment.SocketAddress, options, options.SpotOptions)
        {
            ContinueOnQueryResponse = true;
            UnhandledMessageExpected = true;

            AddGenericHandler("HB", (messageEvent) => { });
            AddGenericHandler("Info", InfoHandler);
            AddGenericHandler("Conf", ConfHandler);

            _affCode = options.AffiliateCode;
            _bookSerializer.Converters.Add(new OrderBookEntryConverter());
        }
        #endregion
        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new BitfinexAuthenticationProvider(credentials, ClientOptions.NonceProvider ?? new BitfinexNonceProvider());

        #region public methods

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<DataEvent<BitfinexStreamSymbolOverview>> handler, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            var internalHandler = new Action<DataEvent<JToken>>(data =>
            {
                HandleData("Ticker", (JArray)data.Data[1]!, symbol, data, handler);
            });
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), new BitfinexSubscriptionRequest("ticker", symbol), null, false, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, Precision precision, Frequency frequency, int length, Action<DataEvent<IEnumerable<BitfinexOrderBookEntry>>> handler, Action<DataEvent<int>>? checksumHandler = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            length.ValidateIntValues(nameof(length), 1, 25, 100, 250);
            if (precision == Precision.R0)
                throw new ArgumentException("Invalid precision R0, use SubscribeToRawBookUpdatesAsync instead");

            var internalHandler = new Action<DataEvent<JToken>>(data =>
            {
                if (data.Data[1]?.ToString() == "cs")
                {
                    // Process
                    checksumHandler?.Invoke(data.As(data.Data[2]!.Value<int>(), symbol));
                }
                else
                {
                    var dataArray = (JArray)data.Data[1]!;
                    if (dataArray.Count == 0)
                        // Empty array
                        return;

                    if (dataArray[0].Type == JTokenType.Array)
                    {
                        HandleData("Book snapshot", dataArray, symbol, data, handler, _bookSerializer);
                    }
                    else
                    {
                        HandleSingleToArrayData("Book update", dataArray, symbol, data, handler, _bookSerializer);
                    }
                }
            });

            var sub = new BitfinexBookSubscriptionRequest(
                symbol,
                JsonConvert.SerializeObject(precision, new PrecisionConverter(false)),
                JsonConvert.SerializeObject(frequency, new FrequencyConverter(false)),
                length);
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), sub, null, false, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToRawOrderBookUpdatesAsync(string symbol, int limit, Action<DataEvent<IEnumerable<BitfinexRawOrderBookEntry>>> handler, Action<DataEvent<int>>? checksumHandler = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            var internalHandler = new Action<DataEvent<JToken>>(data =>
            {
                if (data.Data[1]?.ToString() == "cs")
                {
                    // Process
                    checksumHandler?.Invoke(data.As(data.Data[2]!.Value<int>(), symbol));
                }
                else
                {
                    var dataArray = (JArray)data.Data[1]!;
                    if (dataArray[0].Type == JTokenType.Array)
                        HandleData("Raw book snapshot", dataArray, symbol, data, handler);
                    else
                        HandleSingleToArrayData("Raw book update", dataArray, symbol, data, handler);
                }
            });
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), new BitfinexRawBookSubscriptionRequest(symbol, "R0", limit), null, false, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<DataEvent<IEnumerable<BitfinexTradeSimple>>> handler, CancellationToken ct = default)
        {
            var internalHandler = new Action<DataEvent<JToken>>(data =>
            {
                var arr = (JArray)data.Data;
                if (arr[1].Type == JTokenType.Array)
                {
                    HandleData("Trade snapshot", (JArray)arr[1], symbol, data, handler);
                }
                else
                {
                    var desResult = Deserialize<BitfinexTradeSimple>(arr[2]);
                    if (!desResult)
                    {
                        _logger.Log(LogLevel.Warning, "Failed to deserialize trade object: " + desResult.Error);
                        return;
                    }
                    desResult.Data.UpdateType = BitfinexEvents.EventMapping[arr[1].ToString()];
                    handler(data.As<IEnumerable<BitfinexTradeSimple>>(new[] { desResult.Data }, symbol));
                }
            });
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), new BitfinexSubscriptionRequest("trades", symbol), null, false, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, KlineInterval interval, Action<DataEvent<IEnumerable<BitfinexKline>>> handler, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            var internalHandler = new Action<DataEvent<JToken>>(data =>
            {
                var dataArray = (JArray)data.Data[1]!;
                if (dataArray.Count == 0)
                {
                    _logger.Log(LogLevel.Warning, "No data in kline update, check if the symbol is correct");
                    return;
                }

                if (dataArray[0].Type == JTokenType.Array)
                    HandleData("Kline snapshot", dataArray, symbol, data, handler);
                else
                    HandleSingleToArrayData("Kline update", dataArray, symbol, data, handler);
            });
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), new BitfinexKlineSubscriptionRequest(symbol, JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false))), null, false, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToUserTradeUpdatesAsync(
            Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexOrder>>>> orderHandler,
            Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexTradeDetails>>>> tradeHandler,
             Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexPosition>>>> positionHandler,
             CancellationToken ct = default)
        {
            var tokenHandler = new Action<DataEvent<JToken>>(tokenData =>
            {
                HandleAuthUpdate(tokenData, orderHandler, "Orders");
                HandleAuthUpdate(tokenData, tradeHandler, "Trades");
                HandleAuthUpdate(tokenData, positionHandler, "Positions");
            });

            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), null, "Orders|Trades|Positions", true, tokenHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToBalanceUpdatesAsync(Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexWallet>>>> walletHandler, CancellationToken ct = default)
        {
            var tokenHandler = new Action<DataEvent<JToken>>(tokenData =>
            {
                HandleAuthUpdate(tokenData, walletHandler, "Wallet");
            });

            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), null, "Wallet", true, tokenHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToFundingUpdatesAsync(
            Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexFundingOffer>>>> fundingOfferHandler,
            Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexFundingCredit>>>> fundingCreditHandler,
            Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexFunding>>>> fundingLoanHandler,
            CancellationToken ct = default)
        {
            var tokenHandler = new Action<DataEvent<JToken>>(tokenData =>
            {
                HandleAuthUpdate(tokenData, fundingOfferHandler, "FundingOffers");
                HandleAuthUpdate(tokenData, fundingCreditHandler, "FundingCredits");
                HandleAuthUpdate(tokenData, fundingLoanHandler, "FundingLoans");
            });

            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), null, "FundingOffers|FundingCredits|FundingLoans", true, tokenHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<BitfinexOrder>> PlaceOrderAsync(OrderSide side, OrderType type, string symbol, decimal quantity, long? groupId = null, long? clientOrderId = null, decimal? price = null, decimal? priceTrailing = null, decimal? priceAuxiliaryLimit = null, decimal? priceOcoStop = null, OrderFlags? flags = null, int? leverage = null, DateTime? cancelTime = null, string? affiliateCode = null)
        {
            symbol.ValidateBitfinexSymbol();
            _logger.Log(LogLevel.Information, "Going to place order");
            clientOrderId ??= GenerateClientOrderId();

            var affCode = affiliateCode ?? _affCode;
            var query = new BitfinexSocketQuery(clientOrderId.ToString(), BitfinexEventType.OrderNew, new BitfinexNewOrder
            {
                Amount = side == OrderSide.Buy ? quantity : -quantity,
                OrderType = type,
                Symbol = symbol,
                Price = price,
                ClientOrderId = clientOrderId,
                Flags = flags,
                GroupId = groupId,
                PriceAuxiliaryLimit = priceAuxiliaryLimit,
                PriceOCOStop = priceOcoStop,
                PriceTrailing = priceTrailing,
                Leverage = leverage,
                CancelAfter = cancelTime,
                Meta = affCode == null ? null : new BitfinexMeta() { AffiliateCode = affCode }
            });

            return await QueryAsync<BitfinexOrder>(BaseAddress.AppendPath("ws/2"), query, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<BitfinexOrder>> UpdateOrderAsync(long orderId, decimal? price = null, decimal? quantity = null, decimal? delta = null, decimal? priceAuxiliaryLimit = null, decimal? priceTrailing = null, OrderFlags? flags = null)
        {
            _logger.Log(LogLevel.Information, "Going to update order " + orderId);
            var query = new BitfinexSocketQuery(orderId.ToString(CultureInfo.InvariantCulture), BitfinexEventType.OrderUpdate, new BitfinexUpdateOrder
            {
                OrderId = orderId,
                Amount = quantity,
                Price = price,
                Flags = flags,
                PriceAuxiliaryLimit = priceAuxiliaryLimit?.ToString(CultureInfo.InvariantCulture),
                PriceTrailing = priceTrailing?.ToString(CultureInfo.InvariantCulture)
            });

            return await QueryAsync<BitfinexOrder>(BaseAddress.AppendPath("ws/2"), query, true).ConfigureAwait(false);
        }

        ///// <summary>
        ///// Cancel all open orders
        ///// </summary>
        ///// <returns></returns>
        //public CallResult<bool> CancelAllOrders() => CancelAllOrdersAsync().Result;
        ///// <summary>
        ///// Cancel all open orders
        ///// </summary>
        ///// <returns></returns>
        //public async Task<CallResult<bool>> CancelAllOrdersAsync()
        //{
        //    // Doesn't seem to work even though it is implemented as described at https://docs.bitfinex.com/v2/reference#ws-input-order-cancel-multi 
        //    _log.Write(LogLevel.Information, "Going to cancel all orders");
        //    var query = new BitfinexSocketQuery(null, BitfinexEventType.OrderCancelMulti, new BitfinexMultiCancel { All = true });

        //    return await Query<bool>(query, true).ConfigureAwait(false);
        //}

        /// <inheritdoc />
        public async Task<CallResult<BitfinexOrder>> CancelOrderAsync(long orderId)
        {
            _logger.Log(LogLevel.Information, "Going to cancel order " + orderId);
            var query = new BitfinexSocketQuery(orderId.ToString(CultureInfo.InvariantCulture), BitfinexEventType.OrderCancel, new JObject { ["id"] = orderId });

            return await QueryAsync<BitfinexOrder>(BaseAddress.AppendPath("ws/2"), query, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<bool>> CancelOrdersByGroupIdAsync(long groupOrderId)
        {
            return await CancelOrdersAsync(null, null, new Dictionary<long, long?> { { groupOrderId, null } }).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<bool>> CancelOrdersByGroupIdsAsync(IEnumerable<long> groupOrderIds)
        {
            groupOrderIds.ValidateNotNull(nameof(groupOrderIds));
            return await CancelOrdersAsync(null, null, groupOrderIds.ToDictionary(v => v, k => (long?)null)).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<bool>> CancelOrdersAsync(IEnumerable<long> orderIds)
        {
            orderIds.ValidateNotNull(nameof(orderIds));
            return await CancelOrdersAsync(orderIds, null).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<bool>> CancelOrdersByClientOrderIdsAsync(Dictionary<long, DateTime> clientOrderIds)
        {
            return await CancelOrdersAsync(null, clientOrderIds).ConfigureAwait(false);
        }
        #endregion

        #region private methods
        private void HandleData<T>(string name, JArray dataArray, string? symbol, DataEvent<JToken> dataEvent, Action<DataEvent<T>> handler, JsonSerializer? serializer = null)
        {
            var desResult = Deserialize<T>(dataArray, serializer: serializer);
            if (!desResult)
            {
                _logger.Log(LogLevel.Warning, $"Failed to Deserialize {name} object: " + desResult.Error);
                return;
            }

            handler(dataEvent.As(desResult.Data, symbol));
        }

        private void HandleSingleToArrayData<T>(string name, JArray dataArray, string? symbol, DataEvent<JToken> dataEvent, Action<DataEvent<IEnumerable<T>>> handler, JsonSerializer? serializer = null)
        {
            var wrapperArray = new JArray { dataArray };

            var desResult = Deserialize<IEnumerable<T>>(wrapperArray, serializer: serializer);
            if (!desResult)
            {
                _logger.Log(LogLevel.Warning, $"Failed to Deserialize  {name} object: " + desResult.Error);
                return;
            }

            handler(dataEvent.As(desResult.Data, symbol));
        }

        private async Task<CallResult<bool>> CancelOrdersAsync(IEnumerable<long>? orderIds = null, Dictionary<long, DateTime>? clientOrderIds = null, Dictionary<long, long?>? groupOrderIds = null)
        {
            if (orderIds == null && clientOrderIds == null && groupOrderIds == null)
                throw new ArgumentException("Either orderIds, clientOrderIds or groupOrderIds should be provided");

            _logger.Log(LogLevel.Information, "Going to cancel multiple orders");
            var cancelObject = new BitfinexMultiCancel { OrderIds = orderIds };
            if (clientOrderIds != null)
            {
                cancelObject.ClientIds = new object[clientOrderIds.Count][];
                for (var i = 0; i < cancelObject.ClientIds.Length; i++)
                {
                    cancelObject.ClientIds[i] = new object[]
                    {
                        clientOrderIds.ElementAt(i).Key,
                        clientOrderIds.ElementAt(i).Value.ToString("yyyy-MM-dd")
                    };
                }
            }
            if (groupOrderIds != null)
                cancelObject.GroupIds = new[] { groupOrderIds.Select(g => g.Key).ToArray() };

            var query = new BitfinexSocketQuery(null, BitfinexEventType.OrderCancelMulti, cancelObject);
            return await QueryAsync<bool>(query, true).ConfigureAwait(false);
        }

        private void HandleAuthUpdate<T>(DataEvent<JToken> token, Action<DataEvent<BitfinexSocketEvent<IEnumerable<T>>>> action, string category)
        {
            var evntStr = token.Data[1]?.ToString();
            if (evntStr == null)
                return;

            var evntType = BitfinexEvents.EventMapping[evntStr];
            var evnt = BitfinexEvents.Events.Single(e => e.EventType == evntType);
            if (evnt.Category != category)
                return;

            if (action == null)
            {
                _logger.Log(LogLevel.Debug, $"Ignoring {evnt.EventType} event because not subscribed");
                return;
            }

            IEnumerable<T> data;
            if (evnt.Single)
            {
                var result = Deserialize<T>(token.Data[2]!);
                if (!result)
                {
                    _logger.Log(LogLevel.Warning, "Failed to Deserialize data: " + result.Error);
                    return;
                }
                data = new[] { result.Data };
            }
            else
            {
                var result = Deserialize<IEnumerable<T>>(token.Data[2]!);
                if (!result)
                {
                    _logger.Log(LogLevel.Warning, "Failed to Deserialize data: " + result.Error);
                    return;
                }
                data = result.Data;
            }

            action(token.As(new BitfinexSocketEvent<IEnumerable<T>>(evntType, data)));
        }

        private long GenerateClientOrderId()
        {
            var buffer = new byte[8];
            _random.NextBytes(buffer);
            return (long)Math.Round(Math.Abs(BitConverter.ToInt32(buffer, 0)) / 1000m);
        }


        private void ConfHandler(MessageEvent messageEvent)
        {
            var confEvent = messageEvent.JsonData.Type == JTokenType.Object && messageEvent.JsonData["event"]?.ToString() == "conf";
            if (!confEvent)
                return;

            // Could check conf result;
        }

        private void InfoHandler(MessageEvent messageEvent)
        {
            var infoEvent = messageEvent.JsonData.Type == JTokenType.Object && messageEvent.JsonData["event"]?.ToString() == "info";
            if (!infoEvent)
                return;

            _logger.Log(LogLevel.Debug, $"Socket {messageEvent.Connection.SocketId} Info event received: {messageEvent.JsonData}");
            if (messageEvent.JsonData["code"] == null)
            {
                // welcome event, send a config message for receiving checsum updates for order book subscriptions
                messageEvent.Connection.Send(new BitfinexSocketConfig { Event = "conf", Flags = 131072 });
                return;
            }

            var code = messageEvent.JsonData["code"]?.Value<int>();
            switch (code)
            {
                case 20051:
                    _logger.Log(LogLevel.Information, $"Socket {messageEvent.Connection.SocketId} Code {code} received, reconnecting socket");
                    messageEvent.Connection.PausedActivity = true; // Prevent new operations to be send
                    _ = messageEvent.Connection.TriggerReconnectAsync();
                    break;
                case 20060:
                    _logger.Log(LogLevel.Information, $"Socket {messageEvent.Connection.SocketId} Code {code} received, entering maintenance mode");
                    messageEvent.Connection.PausedActivity = true;
                    break;
                case 20061:
                    _logger.Log(LogLevel.Information, $"Socket {messageEvent.Connection.SocketId} Code {code} received, leaving maintenance mode. Reconnecting/Resubscribing socket.");
                    _ = messageEvent.Connection.TriggerReconnectAsync(); // Closing it via socket will automatically reconnect
                    break;
                default:
                    _logger.Log(LogLevel.Warning, $"Socket {messageEvent.Connection.SocketId} Unknown info code received: {code}");
                    break;
            }
        }


        /// <inheritdoc />
        protected override async Task<bool> UnsubscribeAsync(SocketConnection connection, SocketSubscription subscription)
        {
            if (subscription.Request == null)
            {
                // If we don't have a request object we can't unsubscribe it. Probably is an auth subscription which gets pushed regardless
                // Just returning true here will remove the handler and close the socket if there are no other handlers left on the socket, which is the best we can do
                return true;
            }

            var channelId = ((BitfinexSubscriptionRequest)subscription.Request!).ChannelId;
            var unsub = new BitfinexUnsubscribeRequest(channelId);
            var result = false;
            await connection.SendAndWaitAsync(unsub, ClientOptions.RequestTimeout, null, data =>
            {
                if (data.Type != JTokenType.Object)
                    return false;

                var evnt = data["event"]?.ToString();
                var channel = data["chanId"]?.ToString();
                if (evnt == null || channel == null)
                    return false;

                if (!int.TryParse(channel, out var chan))
                    return false;

                result = evnt == "unsubscribed" && channelId == chan;
                return result;
            }).ConfigureAwait(false);
            return result;
        }

        private static BitfinexAuthentication GetAuthObject(SocketApiClient apiClient, params string[] filter)
        {
            var authProvider = (BitfinexAuthenticationProvider)apiClient.AuthenticationProvider!;
            var n = authProvider.GetNonce().ToString();
            var authentication = new BitfinexAuthentication
            {
                Event = "auth",
                ApiKey = authProvider.GetApiKey(),
                Nonce = n,
                Payload = "AUTH" + n
            };
            if (filter.Any())
                authentication.Filter = filter;
            authentication.Signature = authProvider.Sign(authentication.Payload).ToLower(CultureInfo.InvariantCulture);
            return authentication;
        }

        #endregion

        /// <inheritdoc />
        protected override async Task<CallResult<bool>> AuthenticateSocketAsync(SocketConnection s)
        {
            if (s.ApiClient.AuthenticationProvider == null)
                return new CallResult<bool>(new NoApiCredentialsError());

            var authObject = GetAuthObject(s.ApiClient);
            var result = new CallResult<bool>(new ServerError("No response from server"));
            await s.SendAndWaitAsync(authObject, ClientOptions.RequestTimeout, null, tokenData =>
            {
                if (tokenData.Type != JTokenType.Object)
                    return false;

                if (tokenData["event"]?.ToString() != "auth")
                    return false;

                var authResponse = Deserialize<BitfinexAuthenticationResponse>(tokenData);
                if (!authResponse)
                {
                    _logger.Log(LogLevel.Warning, $"Socket {s.SocketId} authentication failed: " + authResponse.Error);
                    result = new CallResult<bool>(authResponse.Error!);
                    return false;
                }

                if (authResponse.Data.Status != "OK")
                {
                    var error = new ServerError(authResponse.Data.ErrorCode, authResponse.Data.ErrorMessage ?? "-");
                    result = new CallResult<bool>(error);
                    _logger.Log(LogLevel.Debug, $"Socket {s.SocketId} authentication failed: " + error);
                    return false;
                }

                _logger.Log(LogLevel.Debug, $"Socket {s.SocketId} authentication completed");
                result = new CallResult<bool>(true);
                return true;
            }).ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc />
#pragma warning disable 8765
        protected override bool HandleQueryResponse<T>(SocketConnection s, object request, JToken data, out CallResult<T>? callResult)
#pragma warning restore 8765
        {
            callResult = null;
            if (data.Type != JTokenType.Array)
                return false;

            var array = (JArray)data;
            if (array.Count < 3)
                return false;

            var bfRequest = (BitfinexSocketQuery)request;
            var evntString = data[1]!.ToString();
            if (!BitfinexEvents.EventMapping.TryGetValue(evntString, out var eventType))
                return false;

            if (eventType == BitfinexEventType.Notification)
            {
                var notificationData = (JArray)data[2]!;
                var notificationType = BitfinexEvents.EventMapping[notificationData[1].ToString()];
                if (notificationType != BitfinexEventType.OrderNewRequest
                    && notificationType != BitfinexEventType.OrderCancelRequest
                    && notificationType != BitfinexEventType.OrderUpdateRequest
                    && notificationType != BitfinexEventType.OrderCancelMultiRequest)
                {
                    return false;
                }

                var statusString = (notificationData[6].ToString()).ToLower(CultureInfo.InvariantCulture);
                if (statusString == "error")
                {
                    if (bfRequest.QueryType == BitfinexEventType.OrderNew && notificationType == BitfinexEventType.OrderNewRequest)
                    {
                        var orderData = notificationData[4];
                        if (orderData[2]?.ToString() != bfRequest.Id)
                            return false;

                        callResult = new CallResult<T>(new ServerError(notificationData[7].ToString()));
                        return true;
                    }

                    if (bfRequest.QueryType == BitfinexEventType.OrderCancel && notificationType == BitfinexEventType.OrderCancelRequest)
                    {
                        var orderData = notificationData[4];
                        if (orderData[0]?.ToString() != bfRequest.Id)
                            return false;

                        callResult = new CallResult<T>(new ServerError(notificationData[7].ToString()));
                        return true;
                    }

                    if (bfRequest.QueryType == BitfinexEventType.OrderUpdate && notificationType == BitfinexEventType.OrderUpdateRequest)
                    {
                        // OrderUpdateRequest not found notification doesn't carry the order id, where as OrderCancelRequest not found notification does..
                        // Anyway, can't check for ids, so just assume its for this one

                        callResult = new CallResult<T>(new ServerError(notificationData[7].ToString()));
                        return true;
                    }

                    if (bfRequest.QueryType == BitfinexEventType.OrderCancelMulti && notificationType == BitfinexEventType.OrderCancelMultiRequest)
                    {
                        callResult = new CallResult<T>(new ServerError(notificationData[7].ToString()));
                        return true;
                    }
                }

                if (notificationType == BitfinexEventType.OrderNewRequest
                || notificationType == BitfinexEventType.OrderUpdateRequest
                || notificationType == BitfinexEventType.OrderCancelRequest)
                {
                    if (bfRequest.QueryType == BitfinexEventType.OrderNew
                    || bfRequest.QueryType == BitfinexEventType.OrderUpdate
                    || bfRequest.QueryType == BitfinexEventType.OrderCancel)
                    {
                        var orderData = notificationData[4];
                        var dataOrderId = orderData[0]?.ToString();
                        var dataOrderClientId = orderData[2]?.ToString();
                        if (dataOrderId == bfRequest.Id || dataOrderClientId == bfRequest.Id)
                        {
                            var desResult = Deserialize<T>(orderData);
                            if (!desResult)
                            {
                                callResult = new CallResult<T>(desResult.Error!);
                                return true;
                            }

                            callResult = new CallResult<T>(desResult.Data);
                            return true;
                        }
                    }
                }

                if (notificationType == BitfinexEventType.OrderCancelMultiRequest)
                {
                    callResult = new CallResult<T>(Deserialize<T>(JToken.Parse("true")).Data);
                    return true;
                }
            }

            if (bfRequest.QueryType == BitfinexEventType.OrderCancelMulti && eventType == BitfinexEventType.OrderCancel)
            {
                callResult = new CallResult<T>(Deserialize<T>(JToken.Parse("true")).Data);
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        protected override bool HandleSubscriptionResponse(SocketConnection s, SocketSubscription subscription, object request, JToken data, out CallResult<object>? callResult)
        {
            callResult = null;
            if (data.Type != JTokenType.Object)
                return false;

            var infoEvent = data["event"]?.ToString() == "subscribed";
            var errorEvent = data["event"]?.ToString() == "error";
            if (!infoEvent && !errorEvent)
                return false;

            if (infoEvent)
            {
                var subResponse = Deserialize<BitfinexSubscribeResponse>(data);
                if (!subResponse)
                {
                    callResult = new CallResult<object>(subResponse.Error!);
                    _logger.Log(LogLevel.Warning, $"Socket {s.SocketId} subscription failed: " + subResponse.Error);
                    return false;
                }

                var bRequest = (BitfinexSubscriptionRequest)request;
                if (!bRequest.CheckResponse(data))
                    return false;

                bRequest.ChannelId = subResponse.Data.ChannelId;
                callResult = subResponse.As<object>(subResponse.Data);
                return true;
            }
            else
            {
                var subResponse = Deserialize<BitfinexErrorResponse>(data);
                if (!subResponse)
                {
                    callResult = new CallResult<object>(subResponse.Error!);
                    _logger.Log(LogLevel.Warning, $"Socket {s.SocketId} subscription failed: " + subResponse.Error);
                    return false;
                }

                var error = new ServerError(subResponse.Data.Code, subResponse.Data.Message);
                callResult = new CallResult<object>(error);
                _logger.Log(LogLevel.Debug, $"Socket {s.SocketId} subscription failed: " + error);
                return true;
            }
        }

        /// <inheritdoc />
        protected override bool MessageMatchesHandler(SocketConnection socketConnection, JToken message, object request)
        {
            if (message.Type != JTokenType.Array)
                return false;

            var array = (JArray)message;
            if (array.Count < 2)
                return false;

            if (!int.TryParse(array[0].ToString(), out var channelId))
                return false;

            if (channelId == 0)
                return false;

            var subId = ((BitfinexSubscriptionRequest)request).ChannelId;
            return channelId == subId && array[1].ToString() != "hb";
        }

        /// <inheritdoc />
        protected override bool MessageMatchesHandler(SocketConnection socketConnection, JToken message, string identifier)
        {
            if (message.Type == JTokenType.Object)
            {
                if (identifier == "Info")
                    return message["event"]?.ToString() == "info";
                if (identifier == "Conf")
                    return message["event"]?.ToString() == "conf";
            }

            else if (message.Type == JTokenType.Array)
            {
                var array = (JArray)message;
                if (array.Count < 2)
                    return false;

                if (identifier == "HB")
                    return array[1].ToString() == "hb";

                if (!int.TryParse(array[0].ToString(), out var channelId))
                    return false;

                if (channelId != 0)
                    return false;

                var split = identifier.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var id in split)
                {
                    var events = BitfinexEvents.GetEventsForCategory(id);
                    var eventTypeString = array[1].ToString();
                    var eventType = BitfinexEvents.EventMapping[eventTypeString];
                    var evnt = events.SingleOrDefault(e => e.EventType == eventType);
                    if (evnt != null)
                        return true;
                }
            }

            return false;
        }
    }
}
