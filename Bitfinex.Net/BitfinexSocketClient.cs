using Bitfinex.Net.Converters;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.SocketObjects;
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
using Bitfinex.Net.Interfaces;
using Microsoft.Extensions.Logging;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;

namespace Bitfinex.Net
{
    /// <summary>
    /// Socket client for the Bitfinex API
    /// </summary>
    public class BitfinexSocketClient : SocketClient, IBitfinexSocketClient
    {
        #region fields
        private static BitfinexSocketClientOptions defaultOptions = new BitfinexSocketClientOptions();
        private static BitfinexSocketClientOptions DefaultOptions => defaultOptions.Copy<BitfinexSocketClientOptions>();

        private readonly string? _affCode;

        private readonly Random random = new Random();

        private readonly JsonSerializer _bookSerializer = new JsonSerializer();
        #endregion

        #region ctor
        /// <summary>
        /// Create a new instance of BitfinexSocketClient using the default options
        /// </summary>
        public BitfinexSocketClient() : this(DefaultOptions)
        {
        }

        /// <summary>
        /// Create a new instance of BitfinexSocketClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public BitfinexSocketClient(BitfinexSocketClientOptions options) : base("Bitfinex", options, options.ApiCredentials == null ? null : new BitfinexAuthenticationProvider(options.ApiCredentials, options.NonceProvider))
        {
            if (options == null)
                throw new ArgumentException("Cant pass null options, use empty constructor for default");

            ContinueOnQueryResponse = true;
            UnhandledMessageExpected = true;
            _bookSerializer.Converters.Add(new OrderBookEntryConverter());
            _affCode = options.AffiliateCode;

            AddGenericHandler("HB", (messageEvent) => { });
            AddGenericHandler("Info", InfoHandler);
            AddGenericHandler("Conf", ConfHandler);
        }
        #endregion

        #region public methods
        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        /// <param name="nonceProvider">Optional nonce provider for signing requests. Careful providing a custom provider; once a nonce is sent to the server, every request after that needs a higher nonce than that</param>
        public void SetApiCredentials(string apiKey, string apiSecret, INonceProvider? nonceProvider = null)
        {
            SetAuthenticationProvider(new BitfinexAuthenticationProvider(new ApiCredentials(apiKey, apiSecret), nonceProvider));
        }

        /// <summary>
        /// set the default options used when creating a client without specifying options
        /// </summary>
        /// <param name="newDefaultOptions"></param>
        public static void SetDefaultOptions(BitfinexSocketClientOptions newDefaultOptions)
        {
            defaultOptions = newDefaultOptions;
        }

        /// <summary>
        /// Subscribes to ticker updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<DataEvent<BitfinexStreamSymbolOverview>> handler)
        {
            symbol.ValidateBitfinexSymbol();
            var internalHandler = new Action<DataEvent<JToken>>(data =>
            {
                HandleData("Ticker", (JArray) data.Data[1]!, symbol, data, handler);
            });
            return await SubscribeAsync(new BitfinexSubscriptionRequest("ticker", symbol), null, false, internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribes to order book updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="precision">The precision of the updates</param>
        /// <param name="frequency">The frequency of updates</param>
        /// <param name="length">The range for the order book updates, either 25 or 100</param>
        /// <param name="handler">The handler for the data</param>
        /// <param name="checksumHandler">The handler for the checksum, can be used to validate a order book implementation</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToBookUpdatesAsync(string symbol, Precision precision, Frequency frequency, int length, Action<DataEvent<IEnumerable<BitfinexOrderBookEntry>>> handler, Action<DataEvent<int>>? checksumHandler = null)
        {
            symbol.ValidateBitfinexSymbol();
            length.ValidateIntValues(nameof(length), 25, 100);
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
            return await SubscribeAsync(sub, null, false, internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribes to raw order book updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="limit">The range for the order book updates</param>
        /// <param name="handler">The handler for the data</param>
        /// <param name="checksumHandler">The handler for the checksum, can be used to validate a order book implementation</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToRawBookUpdatesAsync(string symbol, int limit, Action<DataEvent<IEnumerable<BitfinexRawOrderBookEntry>>> handler, Action<DataEvent<int>>? checksumHandler = null)
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
            return await SubscribeAsync(new BitfinexRawBookSubscriptionRequest(symbol, "R0", limit), null, false, internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribes to public trade updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<DataEvent<IEnumerable<BitfinexTradeSimple>>> handler)
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
                        log.Write(LogLevel.Warning, "Failed to deserialize trade object: " + desResult.Error);
                        return;
                    }
                    desResult.Data.UpdateType = BitfinexEvents.EventMapping[arr[1].ToString()];
                    handler(data.As<IEnumerable<BitfinexTradeSimple>>(new[] { desResult.Data }, symbol));
                }
            });
            return await SubscribeAsync(new BitfinexSubscriptionRequest("trades", symbol), null, false, internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribes to kline updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="interval">The interval of the klines</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, TimeFrame interval, Action<DataEvent<IEnumerable<BitfinexKline>>> handler)
        {
            symbol.ValidateBitfinexSymbol();
            var internalHandler = new Action<DataEvent<JToken>>(data =>
            {
                var dataArray = (JArray)data.Data[1]!;
                if (dataArray.Count == 0)
                {
                    log.Write(LogLevel.Warning, "No data in kline update, check if the symbol is correct");
                    return;
                }

                if (dataArray[0].Type == JTokenType.Array)
                    HandleData("Kline snapshot", dataArray, symbol, data, handler);
                else
                    HandleSingleToArrayData("Kline update", dataArray, symbol, data, handler);
            });
            return await SubscribeAsync(new BitfinexKlineSubscriptionRequest(symbol, JsonConvert.SerializeObject(interval, new TimeFrameConverter(false))), null, false, internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to trading information updates
        /// </summary>
        /// <param name="orderHandler">Data handler for order updates. Can be null if not interested</param>
        /// <param name="tradeHandler">Data handler for trade execution updates. Can be null if not interested</param>
        /// <param name="positionHandler">Data handler for position updates. Can be null if not interested</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToTradingUpdatesAsync(
            Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexOrder>>>> orderHandler,
            Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexTradeDetails>>>> tradeHandler,
             Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexPosition>>>> positionHandler)
        {
            var tokenHandler = new Action<DataEvent<JToken>>(tokenData =>
            {
                HandleAuthUpdate(tokenData, orderHandler, "Orders");
                HandleAuthUpdate(tokenData, tradeHandler, "Trades");
                HandleAuthUpdate(tokenData, positionHandler, "Positions");
            });

            return await SubscribeAsync(null, "Orders|Trades|Positions", true, tokenHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to wallet information updates
        /// </summary>
        /// <param name="walletHandler">Data handler for wallet updates</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToWalletUpdatesAsync(Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexWallet>>>> walletHandler)
        {
            var tokenHandler = new Action<DataEvent<JToken>>(tokenData =>
            {
                HandleAuthUpdate(tokenData, walletHandler, "Wallet");
            });

            return await SubscribeAsync(null, "Wallet", true, tokenHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to funding information updates
        /// </summary>
        /// <param name="fundingOfferHandler">Subscribe to funding offer updates. Can be null if not interested</param>
        /// <param name="fundingCreditHandler">Subscribe to funding credit updates. Can be null if not interested</param>
        /// <param name="fundingLoanHandler">Subscribe to funding loan updates. Can be null if not interested</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToFundingUpdatesAsync(
            Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexFundingOffer>>>> fundingOfferHandler,
            Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexFundingCredit>>>> fundingCreditHandler,
            Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexFunding>>>> fundingLoanHandler)
        {
            var tokenHandler = new Action<DataEvent<JToken>>(tokenData =>
            {
                HandleAuthUpdate(tokenData, fundingOfferHandler, "FundingOffers");
                HandleAuthUpdate(tokenData, fundingCreditHandler, "FundingCredits");
                HandleAuthUpdate(tokenData, fundingLoanHandler, "FundingLoans");
            });

            return await SubscribeAsync(null, "FundingOffers|FundingCredits|FundingLoans", true, tokenHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Places a new order
        /// </summary>
        /// <param name="type">The type of the order</param>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="amount">The amount of the order, positive for buying, negative for selling</param>
        /// <param name="groupId">Group id to assign to the order</param>
        /// <param name="clientOrderId">Client order id to assign to the order</param>
        /// <param name="price">Price of the order</param>
        /// <param name="priceTrailing">Trailing price of the order</param>
        /// <param name="priceAuxiliaryLimit">Auxiliary limit price of the order</param>
        /// <param name="priceOcoStop">Oco stop price of the order</param>
        /// <param name="flags">Additional flags</param>
        /// <param name="affiliateCode">Affiliate code for the order</param>
        /// <returns></returns>
        public async Task<CallResult<BitfinexOrder>> PlaceOrderAsync(OrderType type, string symbol, decimal amount, long? groupId = null, long? clientOrderId = null, decimal? price = null, decimal? priceTrailing = null, decimal? priceAuxiliaryLimit = null, decimal? priceOcoStop = null, OrderFlags? flags = null, string? affiliateCode = null)
        {
            symbol.ValidateBitfinexSymbol();
            log.Write(LogLevel.Information, "Going to place order");
            if (clientOrderId == null)
                clientOrderId = GenerateClientOrderId();

            var affCode = affiliateCode ?? _affCode;
            var query = new BitfinexSocketQuery(clientOrderId.ToString(), BitfinexEventType.OrderNew, new BitfinexNewOrder
            {
                Amount = amount,
                OrderType = type,
                Symbol = symbol,
                Price = price,
                ClientOrderId = clientOrderId,
                Flags = flags,
                GroupId = groupId,
                PriceAuxiliaryLimit = priceAuxiliaryLimit,
                PriceOCOStop = priceOcoStop,
                PriceTrailing = priceTrailing,
                Meta = affCode == null ? null: new BitfinexMeta() { AffiliateCode = affCode }
            });

            return await QueryAsync<BitfinexOrder>(query, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates an order
        /// </summary>
        /// <param name="orderId">The id of the order to update</param>
        /// <param name="price">The new price of the order</param>
        /// <param name="amount">The new amount of the order</param>
        /// <param name="delta">The delta to change</param>
        /// <param name="priceAuxiliaryLimit">the new aux limit price</param>
        /// <param name="priceTrailing">The new trailing price</param>
        /// <param name="flags">The new flags</param>
        /// <returns></returns>
        public async Task<CallResult<BitfinexOrder>> UpdateOrderAsync(long orderId, decimal? price = null, decimal? amount = null, decimal? delta = null, decimal? priceAuxiliaryLimit = null, decimal? priceTrailing = null, OrderFlags? flags = null)
        {
            log.Write(LogLevel.Information, "Going to update order " + orderId);
            var query = new BitfinexSocketQuery(orderId.ToString(CultureInfo.InvariantCulture), BitfinexEventType.OrderUpdate, new BitfinexUpdateOrder
            {
                OrderId = orderId,
                Amount = amount,
                Price = price,
                Flags = flags,
                PriceAuxiliaryLimit = priceAuxiliaryLimit?.ToString(CultureInfo.InvariantCulture),
                PriceTrailing = priceTrailing?.ToString(CultureInfo.InvariantCulture)
            });

            return await QueryAsync<BitfinexOrder>(query, true).ConfigureAwait(false);
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
        //    log.Write(LogLevel.Information, "Going to cancel all orders");
        //    var query = new BitfinexSocketQuery(null, BitfinexEventType.OrderCancelMulti, new BitfinexMultiCancel { All = true });

        //    return await Query<bool>(query, true).ConfigureAwait(false);
        //}

        /// <summary>
        /// Cancels an order
        /// </summary>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <returns></returns>
        public async Task<CallResult<BitfinexOrder>> CancelOrderAsync(long orderId)
        {
            log.Write(LogLevel.Information, "Going to cancel order " + orderId);
            var query = new BitfinexSocketQuery(orderId.ToString(CultureInfo.InvariantCulture), BitfinexEventType.OrderCancel, new JObject { ["id"] = orderId });

            return await QueryAsync<BitfinexOrder>(query, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancels multiple orders based on their groupId
        /// </summary>
        /// <param name="groupOrderId">The group id to cancel</param>
        /// <returns>True if successfully committed on server</returns>
        public async Task<CallResult<bool>> CancelOrdersByGroupIdAsync(long groupOrderId)
        {
            return await CancelOrdersAsync(null, null, new Dictionary<long, long?> { { groupOrderId, null } }).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancels multiple orders based on their groupIds
        /// </summary>
        /// <param name="groupOrderIds">The group ids to cancel</param>
        /// <returns>True if successfully committed on server</returns>
        public async Task<CallResult<bool>> CancelOrdersByGroupIdsAsync(IEnumerable<long> groupOrderIds)
        {
            groupOrderIds.ValidateNotNull(nameof(groupOrderIds));
            return await CancelOrdersAsync(null, null, groupOrderIds.ToDictionary(v => v, k => (long?)null)).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancels multiple orders based on their order ids
        /// </summary>
        /// <param name="orderIds">The order ids to cancel</param>
        /// <returns>True if successfully committed on server</returns>
        public async Task<CallResult<bool>> CancelOrdersAsync(IEnumerable<long> orderIds)
        {
            orderIds.ValidateNotNull(nameof(orderIds));
            return await CancelOrdersAsync(orderIds, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancels multiple orders based on their clientOrderIds
        /// </summary>
        /// <param name="clientOrderIds">The client order ids to cancel, listed as (clientOrderId, Day) pair. ClientOrderIds are unique per day, so timestamp should be provided</param>
        /// <returns>True if successfully committed on server</returns>
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
                log.Write(LogLevel.Warning, $"Failed to deserialize {name} object: " + desResult.Error);
                return;
            }

            handler(dataEvent.As(desResult.Data, symbol));
        }

        private void HandleSingleToArrayData<T>(string name, JArray dataArray, string? symbol, DataEvent<JToken> dataEvent, Action<DataEvent<IEnumerable<T>>> handler, JsonSerializer? serializer = null)
        {
            var wrapperArray = new JArray {dataArray};

            var desResult = Deserialize<IEnumerable<T>>(wrapperArray, serializer: serializer);
            if (!desResult)
            {
                log.Write(LogLevel.Warning, $"Failed to deserialize  {name} object: " + desResult.Error);
                return;
            }

            handler(dataEvent.As(desResult.Data, symbol));
        }

        private async Task<CallResult<bool>> CancelOrdersAsync(IEnumerable<long>? orderIds = null, Dictionary<long, DateTime>? clientOrderIds = null, Dictionary<long, long?>? groupOrderIds = null)
        {
            if (orderIds == null && clientOrderIds == null && groupOrderIds == null)
                throw new ArgumentException("Either orderIds, clientOrderIds or groupOrderIds should be provided");

            log.Write(LogLevel.Information, "Going to cancel multiple orders");
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
                log.Write(LogLevel.Debug, $"Ignoring {evnt.EventType} event because not subscribed");
                return;
            }

            IEnumerable<T> data;
            if (evnt.Single)
            {
                var result = Deserialize<T>(token.Data[2]!);
                if (!result)
                {
                    log.Write(LogLevel.Warning, "Failed to deserialize data: " + result.Error);
                    return;
                }
                data = new[] { result.Data };
            }
            else
            {
                var result = Deserialize<IEnumerable<T>>(token.Data[2]!);
                if (!result)
                {
                    log.Write(LogLevel.Warning, "Failed to deserialize data: " + result.Error);
                    return;
                }
                data = result.Data;
            }

            action(token.As(new BitfinexSocketEvent<IEnumerable<T>>(evntType, data)));
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

            log.Write(LogLevel.Debug, $"Socket {messageEvent.Connection.Socket.Id} Info event received: {messageEvent.JsonData}");
            if (messageEvent.JsonData["code"] == null)
            {
                // welcome event
                messageEvent.Connection.Send(new BitfinexSocketConfig { Event = "conf", Flags = 131072 });
                return;
            }

            var code = messageEvent.JsonData["code"]?.Value<int>();
            switch (code)
            {
                case 20051:
                    log.Write(LogLevel.Information, $"Socket {messageEvent.Connection.Socket.Id} Code {code} received, reconnecting socket");
                    messageEvent.Connection.PausedActivity = true; // Prevent new operations to be send
                    messageEvent.Connection.Socket.CloseAsync();
                    break;
                case 20060:
                    log.Write(LogLevel.Information, $"Socket {messageEvent.Connection.Socket.Id} Code {code} received, entering maintenance mode");
                    messageEvent.Connection.PausedActivity = true;
                    break;
                case 20061:
                    log.Write(LogLevel.Information, $"Socket {messageEvent.Connection.Socket.Id} Code {code} received, leaving maintenance mode. Reconnecting/Resubscribing socket.");
                    messageEvent.Connection.Socket.CloseAsync(); // Closing it via socket will automatically reconnect
                    break;
                default:
                    log.Write(LogLevel.Warning, $"Socket {messageEvent.Connection.Socket.Id} Unknown info code received: {code}");
                    break;
            }
        }

        /// <inheritdoc />
        protected override async Task<bool> UnsubscribeAsync(SocketConnection connection, SocketSubscription subscription)
        {
            if(subscription.Request == null)
            {
                // If we don't have a request object we can't unsubscribe it. Probably is an auth subscription which gets pushed regardless
                // Just returning true here will remove the handler and close the socket if there are no other handlers left on the socket, which is the best we can do
                return true;
            }

            var channelId = ((BitfinexSubscriptionRequest) subscription.Request!).ChannelId;
            var unsub = new BitfinexUnsubscribeRequest(channelId);
            var result = false;
            await connection.SendAndWaitAsync(unsub, ResponseTimeout, data =>
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

        private BitfinexAuthentication GetAuthObject(params string[] filter)
        {
            var n = ((BitfinexAuthenticationProvider)authProvider!).GetNonce().ToString();
            var authentication = new BitfinexAuthentication
            {
                Event = "auth",
                ApiKey = authProvider!.Credentials.Key!.GetString(),
                Nonce = n,
                Payload = "AUTH" + n
            };
            if (filter.Any())
                authentication.Filter = filter;
            authentication.Signature = authProvider.Sign(authentication.Payload).ToLower(CultureInfo.InvariantCulture);
            return authentication;
        }

        private long GenerateClientOrderId()
        {
            var buffer = new byte[8];
            random.NextBytes(buffer);
            return (long)Math.Round(Math.Abs(BitConverter.ToInt32(buffer, 0)) / 1000m);
        }
        #endregion

        /// <inheritdoc />
        protected override async Task<CallResult<bool>> AuthenticateSocketAsync(SocketConnection s)
        {
            if (authProvider == null)
                return new CallResult<bool>(false, new NoApiCredentialsError());

            var authObject = GetAuthObject();
            var result = new CallResult<bool>(false, new ServerError("No response from server"));
            await s.SendAndWaitAsync(authObject, ResponseTimeout, tokenData =>
            {
                if (tokenData.Type != JTokenType.Object)
                    return false;

                if (tokenData["event"]?.ToString() != "auth")
                    return false;

                var authResponse = Deserialize<BitfinexAuthenticationResponse>(tokenData, false);
                if (!authResponse)
                {
                    log.Write(LogLevel.Warning, $"Socket {s.Socket.Id} authentication failed: " + authResponse.Error);
                    result = new CallResult<bool>(false, authResponse.Error);
                    return false;
                }

                if (authResponse.Data.Status != "OK")
                {
                    var error = new ServerError(authResponse.Data.ErrorCode, authResponse.Data.ErrorMessage ?? "-");
                    result = new CallResult<bool>(false, error);
                    log.Write(LogLevel.Debug, $"Socket {s.Socket.Id} authentication failed: " + error);
                    return false;
                }

                log.Write(LogLevel.Debug, $"Socket {s.Socket.Id} authentication completed");
                result = new CallResult<bool>(true, null);
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

            var array = (JArray) data;
            if (array.Count < 3)
                return false;

            var bfRequest = (BitfinexSocketQuery)request;
            var eventType = BitfinexEvents.EventMapping[data[1]!.ToString()];

            if (eventType == BitfinexEventType.Notification)
            {
                var notificationData = (JArray) data[2]!;
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

                        callResult = new CallResult<T>(default, new ServerError(notificationData[7].ToString()));
                        return true;
                    }

                    if (bfRequest.QueryType == BitfinexEventType.OrderCancel && notificationType == BitfinexEventType.OrderCancelRequest)
                    {
                        var orderData = notificationData[4];
                        if (orderData[0]?.ToString() != bfRequest.Id)
                            return false;

                        callResult = new CallResult<T>(default, new ServerError(notificationData[7].ToString()));
                        return true;
                    }

                    if (bfRequest.QueryType == BitfinexEventType.OrderUpdate && notificationType == BitfinexEventType.OrderUpdateRequest)
                    {
                        // OrderUpdateRequest not found notification doesn't carry the order id, where as OrderCancelRequest not found notification does..
                        // Anyway, can't check for ids, so just assume its for this one

                        callResult = new CallResult<T>(default, new ServerError(notificationData[7].ToString()));
                        return true;
                    }

                    if (bfRequest.QueryType == BitfinexEventType.OrderCancelMulti && notificationType == BitfinexEventType.OrderCancelMultiRequest)
                    {
                        callResult = new CallResult<T>(default, new ServerError(notificationData[7].ToString()));
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
                                callResult = new CallResult<T>(default, desResult.Error);
                                return true;
                            }

                            callResult = new CallResult<T>(desResult.Data, null);
                            return true;
                        }
                    }
                }

                if (notificationType == BitfinexEventType.OrderCancelMultiRequest)
                {
                    callResult = new CallResult<T>(Deserialize<T>(JToken.Parse("true")).Data, null);
                    return true;
                }
            }

            if (bfRequest.QueryType == BitfinexEventType.OrderCancelMulti && eventType == BitfinexEventType.OrderCancel)
            {
                callResult = new CallResult<T>(Deserialize<T>(JToken.Parse("true")).Data, null);
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
                var subResponse = Deserialize<BitfinexSubscribeResponse>(data, false);
                if (!subResponse)
                {
                    callResult = new CallResult<object>(null, subResponse.Error);
                    log.Write(LogLevel.Warning, $"Socket {s.Socket.Id} subscription failed: " + subResponse.Error);
                    return false;
                }

                var bRequest = (BitfinexSubscriptionRequest) request;
                if (!bRequest.CheckResponse(data))
                    return false;                

                bRequest.ChannelId = subResponse.Data.ChannelId;
                callResult = new CallResult<object>(subResponse.Data, subResponse.Error);
                return true;
            }
            else
            {
                var subResponse = Deserialize<BitfinexErrorResponse>(data, false);
                if (!subResponse)
                {
                    callResult = new CallResult<object>(null, subResponse.Error);
                    log.Write(LogLevel.Warning, $"Socket {s.Socket.Id} subscription failed: " + subResponse.Error);
                    return false;
                }

                var error = new ServerError(subResponse.Data.Code, subResponse.Data.Message);
                callResult = new CallResult<object>(null, error);
                log.Write(LogLevel.Debug, $"Socket {s.Socket.Id} subscription failed: " + error);
                return true;
            }
        }

        /// <inheritdoc />
        protected override bool MessageMatchesHandler(JToken message, object request)
        {
            if (message.Type != JTokenType.Array)
                return false;

            var array = (JArray) message;
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
        protected override bool MessageMatchesHandler(JToken message, string identifier)
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
                var array = (JArray) message;
                if (array.Count < 2)
                    return false;

                if (identifier == "HB")
                    return array[1].ToString() == "hb";

                if (!int.TryParse(array[0].ToString(), out var channelId))
                    return false;

                if (channelId != 0)
                    return false;

                var split = identifier.Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries);
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
