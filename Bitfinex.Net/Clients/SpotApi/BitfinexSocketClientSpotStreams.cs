using Bitfinex.Net.Converters;
using Bitfinex.Net.Objects;
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
using CryptoExchange.Net.Logging;
using Bitfinex.Net.Interfaces.Clients.SpotApi;

namespace Bitfinex.Net.Clients.SpotApi
{
    /// <inheritdoc cref="IBitfinexSocketClientSpotStreams" />
    public class BitfinexSocketClientSpotStreams : SocketApiClient, IBitfinexSocketClientSpotStreams
    {
        #region fields
        private readonly BitfinexSocketClient _baseClient;
        private readonly BitfinexSocketClientOptions _options;
        private readonly Log _log;

        private readonly JsonSerializer _bookSerializer = new JsonSerializer();
        private readonly Random random = new Random();
        private readonly string? _affCode;
        #endregion

        #region ctor
        internal BitfinexSocketClientSpotStreams(Log log, BitfinexSocketClient baseClient, BitfinexSocketClientOptions options) :
            base(options, options.SpotStreamsOptions)
        {
            _log = log;
            _baseClient = baseClient;
            _options = options;

            _affCode = options.AffiliateCode;
            _bookSerializer.Converters.Add(new OrderBookEntryConverter());
        }
        #endregion
        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new BitfinexAuthenticationProvider(credentials, _options.NonceProvider ?? new BitfinexNonceProvider());

        #region public methods

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<DataEvent<BitfinexStreamSymbolOverview>> handler, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            var internalHandler = new Action<DataEvent<JToken>>(data =>
            {
                HandleData("Ticker", (JArray)data.Data[1]!, symbol, data, handler);
            });
            return await _baseClient.SubscribeInternalAsync(this, new BitfinexSubscriptionRequest("ticker", symbol), null, false, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, Precision precision, Frequency frequency, int length, Action<DataEvent<IEnumerable<BitfinexOrderBookEntry>>> handler, Action<DataEvent<int>>? checksumHandler = null, CancellationToken ct = default)
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
            return await _baseClient.SubscribeInternalAsync(this, sub, null, false, internalHandler, ct).ConfigureAwait(false);
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
            return await _baseClient.SubscribeInternalAsync(this, new BitfinexRawBookSubscriptionRequest(symbol, "R0", limit), null, false, internalHandler, ct).ConfigureAwait(false);
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
                    var desResult = _baseClient.DeserializeInternal<BitfinexTradeSimple>(arr[2]);
                    if (!desResult)
                    {
                        _log.Write(LogLevel.Warning, "Failed to deserialize trade object: " + desResult.Error);
                        return;
                    }
                    desResult.Data.UpdateType = BitfinexEvents.EventMapping[arr[1].ToString()];
                    handler(data.As<IEnumerable<BitfinexTradeSimple>>(new[] { desResult.Data }, symbol));
                }
            });
            return await _baseClient.SubscribeInternalAsync(this, new BitfinexSubscriptionRequest("trades", symbol), null, false, internalHandler, ct).ConfigureAwait(false);
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
                    _log.Write(LogLevel.Warning, "No data in kline update, check if the symbol is correct");
                    return;
                }

                if (dataArray[0].Type == JTokenType.Array)
                    HandleData("Kline snapshot", dataArray, symbol, data, handler);
                else
                    HandleSingleToArrayData("Kline update", dataArray, symbol, data, handler);
            });
            return await _baseClient.SubscribeInternalAsync(this, new BitfinexKlineSubscriptionRequest(symbol, JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false))), null, false, internalHandler, ct).ConfigureAwait(false);
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

            return await _baseClient.SubscribeInternalAsync(this, null, "Orders|Trades|Positions", true, tokenHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToBalanceUpdatesAsync(Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexWallet>>>> walletHandler, CancellationToken ct = default)
        {
            var tokenHandler = new Action<DataEvent<JToken>>(tokenData =>
            {
                HandleAuthUpdate(tokenData, walletHandler, "Wallet");
            });

            return await _baseClient.SubscribeInternalAsync(this, null, "Wallet", true, tokenHandler, ct).ConfigureAwait(false);
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

            return await _baseClient.SubscribeInternalAsync(this, null, "FundingOffers|FundingCredits|FundingLoans", true, tokenHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<BitfinexOrder>> PlaceOrderAsync(OrderType type, string symbol, decimal quantity, long? groupId = null, long? clientOrderId = null, decimal? price = null, decimal? priceTrailing = null, decimal? priceAuxiliaryLimit = null, decimal? priceOcoStop = null, OrderFlags? flags = null, string? affiliateCode = null)
        {
            symbol.ValidateBitfinexSymbol();
            _log.Write(LogLevel.Information, "Going to place order");
            clientOrderId ??= GenerateClientOrderId();

            var affCode = affiliateCode ?? _affCode;
            var query = new BitfinexSocketQuery(clientOrderId.ToString(), BitfinexEventType.OrderNew, new BitfinexNewOrder
            {
                Amount = quantity,
                OrderType = type,
                Symbol = symbol,
                Price = price,
                ClientOrderId = clientOrderId,
                Flags = flags,
                GroupId = groupId,
                PriceAuxiliaryLimit = priceAuxiliaryLimit,
                PriceOCOStop = priceOcoStop,
                PriceTrailing = priceTrailing,
                Meta = affCode == null ? null : new BitfinexMeta() { AffiliateCode = affCode }
            });

            return await _baseClient.QueryInternalAsync<BitfinexOrder>(this, query, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<BitfinexOrder>> UpdateOrderAsync(long orderId, decimal? price = null, decimal? quantity = null, decimal? delta = null, decimal? priceAuxiliaryLimit = null, decimal? priceTrailing = null, OrderFlags? flags = null)
        {
            _log.Write(LogLevel.Information, "Going to update order " + orderId);
            var query = new BitfinexSocketQuery(orderId.ToString(CultureInfo.InvariantCulture), BitfinexEventType.OrderUpdate, new BitfinexUpdateOrder
            {
                OrderId = orderId,
                Amount = quantity,
                Price = price,
                Flags = flags,
                PriceAuxiliaryLimit = priceAuxiliaryLimit?.ToString(CultureInfo.InvariantCulture),
                PriceTrailing = priceTrailing?.ToString(CultureInfo.InvariantCulture)
            });

            return await _baseClient.QueryInternalAsync<BitfinexOrder>(this, query, true).ConfigureAwait(false);
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

        /// <inheritdoc />
        public async Task<CallResult<BitfinexOrder>> CancelOrderAsync(long orderId)
        {
            _log.Write(LogLevel.Information, "Going to cancel order " + orderId);
            var query = new BitfinexSocketQuery(orderId.ToString(CultureInfo.InvariantCulture), BitfinexEventType.OrderCancel, new JObject { ["id"] = orderId });

            return await _baseClient.QueryInternalAsync<BitfinexOrder>(this, query, true).ConfigureAwait(false);
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
            var desResult = _baseClient.DeserializeInternal<T>(dataArray, serializer: serializer);
            if (!desResult)
            {
                _log.Write(LogLevel.Warning, $"Failed to _baseClient.DeserializeInternal {name} object: " + desResult.Error);
                return;
            }

            handler(dataEvent.As(desResult.Data, symbol));
        }

        private void HandleSingleToArrayData<T>(string name, JArray dataArray, string? symbol, DataEvent<JToken> dataEvent, Action<DataEvent<IEnumerable<T>>> handler, JsonSerializer? serializer = null)
        {
            var wrapperArray = new JArray { dataArray };

            var desResult = _baseClient.DeserializeInternal<IEnumerable<T>>(wrapperArray, serializer: serializer);
            if (!desResult)
            {
                _log.Write(LogLevel.Warning, $"Failed to _baseClient.DeserializeInternal  {name} object: " + desResult.Error);
                return;
            }

            handler(dataEvent.As(desResult.Data, symbol));
        }

        private async Task<CallResult<bool>> CancelOrdersAsync(IEnumerable<long>? orderIds = null, Dictionary<long, DateTime>? clientOrderIds = null, Dictionary<long, long?>? groupOrderIds = null)
        {
            if (orderIds == null && clientOrderIds == null && groupOrderIds == null)
                throw new ArgumentException("Either orderIds, clientOrderIds or groupOrderIds should be provided");

            _log.Write(LogLevel.Information, "Going to cancel multiple orders");
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
            return await _baseClient.QueryInternalAsync<bool>(this, query, true).ConfigureAwait(false);
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
                _log.Write(LogLevel.Debug, $"Ignoring {evnt.EventType} event because not subscribed");
                return;
            }

            IEnumerable<T> data;
            if (evnt.Single)
            {
                var result = _baseClient.DeserializeInternal<T>(token.Data[2]!);
                if (!result)
                {
                    _log.Write(LogLevel.Warning, "Failed to _baseClient.DeserializeInternal data: " + result.Error);
                    return;
                }
                data = new[] { result.Data };
            }
            else
            {
                var result = _baseClient.DeserializeInternal<IEnumerable<T>>(token.Data[2]!);
                if (!result)
                {
                    _log.Write(LogLevel.Warning, "Failed to _baseClient.DeserializeInternal data: " + result.Error);
                    return;
                }
                data = result.Data;
            }

            action(token.As(new BitfinexSocketEvent<IEnumerable<T>>(evntType, data)));
        }

        private long GenerateClientOrderId()
        {
            var buffer = new byte[8];
            random.NextBytes(buffer);
            return (long)Math.Round(Math.Abs(BitConverter.ToInt32(buffer, 0)) / 1000m);
        }
        #endregion

    }
}
