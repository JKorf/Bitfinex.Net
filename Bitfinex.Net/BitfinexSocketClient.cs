using Bitfinex.Net.Converters;
using Bitfinex.Net.Interfaces;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.SocketObjects;
using CryptoExchange.Net;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

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

        private readonly Random random = new Random();
        private static readonly object nonceLock = new object();
        private static long lastNonce;
        internal static string Nonce
        {
            get
            {
                lock (nonceLock)
                {
                    var nonce = (long)Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds * 1000);
                    if (nonce == lastNonce)
                        nonce += 1;

                    lastNonce = nonce;
                    return lastNonce.ToString(CultureInfo.InvariantCulture);
                }
            }
        }
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
        public BitfinexSocketClient(BitfinexSocketClientOptions options) : base(options, options.ApiCredentials == null ? null : new BitfinexAuthenticationProvider(options.ApiCredentials))
        {
            ContinueOnQueryResponse = true;
            AddGenericHandler("HB", (wrapper, msg) => { });
            AddGenericHandler("Info", InfoHandler);
        }
        #endregion

        #region public methods
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
        public CallResult<UpdateSubscription> SubscribeToTickerUpdates(string symbol, Action<BitfinexMarketOverview> handler) => SubscribeToTickerUpdatesAsync(symbol, handler).Result;
        /// <summary>
        /// Subscribes to ticker updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<BitfinexMarketOverview> handler)
        {
            var internalHandler = new Action<JToken>(data =>
            {
                HandleData("Ticker", (JArray) data[1], handler);
            });
            return await Subscribe(new BitfinexSubscriptionRequest("ticker", symbol), null, false, internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribes to order book updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="precision">The precision of the updates</param>
        /// <param name="frequency">The frequency of updates</param>
        /// <param name="length">The range for the order book updates</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        public CallResult<UpdateSubscription> SubscribeToBookUpdates(string symbol, Precision precision, Frequency frequency, int length, Action<IEnumerable<BitfinexOrderBookEntry>> handler) => SubscribeToBookUpdatesAsync(symbol, precision, frequency, length, handler).Result;
        /// <summary>
        /// Subscribes to order book updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="precision">The precision of the updates</param>
        /// <param name="frequency">The frequency of updates</param>
        /// <param name="length">The range for the order book updates, either 25 or 100</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToBookUpdatesAsync(string symbol, Precision precision, Frequency frequency, int length, Action<IEnumerable<BitfinexOrderBookEntry>> handler)
        {
            //if (length != 25 && length != 100)
            //    return new CallResult<UpdateSubscription>(null, new ArgumentError("Limit should be either 25 or 100"));

            var internalHandler = new Action<JToken>(data =>
            {
                var dataArray = (JArray)data[1];
                if (dataArray[0].Type == JTokenType.Array)
                    HandleData("Book snapshot", dataArray, handler);
                else
                    HandleSingleToArrayData("Book update", dataArray, handler);
            });

            var sub = new BitfinexBookSubscriptionRequest(
                symbol,
                JsonConvert.SerializeObject(precision, new PrecisionConverter(false)),
                JsonConvert.SerializeObject(frequency, new FrequencyConverter(false)),
                length);
            return await Subscribe(sub, null, false, internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribes to raw order book updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="limit">The range for the order book updates</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        public CallResult<UpdateSubscription> SubscribeToRawBookUpdates(string symbol, int limit, Action<IEnumerable<BitfinexRawOrderBookEntry>> handler) => SubscribeToRawBookUpdatesAsync(symbol, limit, handler).Result;
        /// <summary>
        /// Subscribes to raw order book updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="limit">The range for the order book updates</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToRawBookUpdatesAsync(string symbol, int limit, Action<IEnumerable<BitfinexRawOrderBookEntry>> handler)
        {
            var internalHandler = new Action<JToken>(data =>
            {
                var dataArray = (JArray)data[1];
                if (dataArray[0].Type == JTokenType.Array)
                    HandleData("Raw book snapshot", dataArray, handler);
                else
                    HandleSingleToArrayData("Raw book update", dataArray, handler);
            });
            return await Subscribe(new BitfinexRawBookSubscriptionRequest(symbol, "R0", limit), null, false, internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribes to public trade updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        public CallResult<UpdateSubscription> SubscribeToTradeUpdates(string symbol, Action<IEnumerable<BitfinexTradeSimple>> handler) => SubscribeToTradeUpdatesAsync(symbol, handler).Result;
        /// <summary>
        /// Subscribes to public trade updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<IEnumerable<BitfinexTradeSimple>> handler)
        {
            var internalHandler = new Action<JToken>(data =>
            {
                var arr = (JArray)data;
                if (arr[1].Type == JTokenType.Array)
                {
                    HandleData("Trade snapshot", (JArray)arr[1], handler);
                }
                else
                {
                    var desResult = Deserialize<BitfinexTradeSimple>(arr[2]);
                    if (!desResult)
                    {
                        log.Write(LogVerbosity.Warning, "Failed to deserialize trade object: " + desResult.Error);
                        return;
                    }
                    desResult.Data.UpdateType = BitfinexEvents.EventMapping[(string)arr[1]];
                    handler(new[] { desResult.Data });
                }
            });
            return await Subscribe(new BitfinexSubscriptionRequest("trades", symbol), null, false, internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribes to candle updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="interval">The interval of the candles</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        public CallResult<UpdateSubscription> SubscribeToCandleUpdates(string symbol, TimeFrame interval, Action<IEnumerable<BitfinexCandle>> handler) => SubscribeToCandleUpdatesAsync(symbol, interval, handler).Result;
        /// <summary>
        /// Subscribes to candle updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="interval">The interval of the candles</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToCandleUpdatesAsync(string symbol, TimeFrame interval, Action<IEnumerable<BitfinexCandle>> handler)
        {
            if (symbol.Length == 6)
                symbol = "t" + symbol.ToUpper();
            else
                symbol = symbol.Substring(0, 1) + symbol.Substring(1).ToUpper();

            var internalHandler = new Action<JToken>(data =>
            {
                var dataArray = (JArray)data[1];
                if (dataArray.Count == 0)
                {
                    log.Write(LogVerbosity.Warning, "No data in kline update, check if the symbol is correct");
                    return;
                }

                if (dataArray[0].Type == JTokenType.Array)
                    HandleData("Kline snapshot", dataArray, handler);
                else
                    HandleSingleToArrayData("Kline update", dataArray, handler);
            });
            return await Subscribe(new BitfinexKlineSubscriptionRequest(symbol, JsonConvert.SerializeObject(interval, new TimeFrameConverter(false))), null, false, internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to trading information updates
        /// </summary>
        /// <param name="orderHandler">Data handler for order updates. Can be null if not interested</param>
        /// <param name="tradeHandler">Data handler for trade execution updates. Can be null if not interested</param>
        /// <param name="positionHandler">Data handler for position updates. Can be null if not interested</param>
        /// <returns></returns>
        public CallResult<UpdateSubscription> SubscribeToTradingUpdates(
            Action<BitfinexSocketEvent<IEnumerable<BitfinexOrder>>> orderHandler,
            Action<BitfinexSocketEvent<IEnumerable<BitfinexTradeDetails>>> tradeHandler,
            Action<BitfinexSocketEvent<IEnumerable<BitfinexPosition>>> positionHandler) => SubscribeToTradingUpdatesAsync(orderHandler, tradeHandler, positionHandler).Result;
        /// <summary>
        /// Subscribe to trading information updates
        /// </summary>
        /// <param name="orderHandler">Data handler for order updates. Can be null if not interested</param>
        /// <param name="tradeHandler">Data handler for trade execution updates. Can be null if not interested</param>
        /// <param name="positionHandler">Data handler for position updates. Can be null if not interested</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToTradingUpdatesAsync(
            Action<BitfinexSocketEvent<IEnumerable<BitfinexOrder>>> orderHandler,
            Action<BitfinexSocketEvent<IEnumerable<BitfinexTradeDetails>>> tradeHandler,
             Action<BitfinexSocketEvent<IEnumerable<BitfinexPosition>>> positionHandler)
        {
            var tokenHandler = new Action<JToken>(tokenData =>
            {
                HandleAuthUpdate(tokenData, orderHandler, "Orders");
                HandleAuthUpdate(tokenData, tradeHandler, "Trades");
                HandleAuthUpdate(tokenData, positionHandler, "Positions");
            });

            return await Subscribe(null, "Orders|Trades|Positions", true, tokenHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to wallet information updates
        /// </summary>
        /// <param name="walletHandler">Data handler for wallet updates</param>
        /// <returns></returns>
        public CallResult<UpdateSubscription> SubscribeToWalletUpdates(Action<BitfinexSocketEvent<IEnumerable<BitfinexWallet>>> walletHandler) => SubscribeToWalletUpdatesAsync(walletHandler).Result;
        /// <summary>
        /// Subscribe to wallet information updates
        /// </summary>
        /// <param name="walletHandler">Data handler for wallet updates</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToWalletUpdatesAsync(Action<BitfinexSocketEvent<IEnumerable<BitfinexWallet>>> walletHandler)
        {
            var tokenHandler = new Action<JToken>(tokenData =>
            {
                HandleAuthUpdate(tokenData, walletHandler, "Wallet");
            });

            return await Subscribe(null, "Wallet", true, tokenHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to funding information updates
        /// </summary>
        /// <param name="fundingOfferHandler">Subscribe to funding offer updates. Can be null if not interested</param>
        /// <param name="fundingCreditHandler">Subscribe to funding credit updates. Can be null if not interested</param>
        /// <param name="fundingLoanHandler">Subscribe to funding loan updates. Can be null if not interested</param>
        /// <returns></returns>
        public CallResult<UpdateSubscription> SubscribeToFundingUpdates(
            Action<BitfinexSocketEvent<IEnumerable<BitfinexFundingOffer>>> fundingOfferHandler,
            Action<BitfinexSocketEvent<IEnumerable<BitfinexFundingCredit>>> fundingCreditHandler,
            Action<BitfinexSocketEvent<IEnumerable<BitfinexFunding>>> fundingLoanHandler) => SubscribeToFundingUpdatesAsync(fundingOfferHandler, fundingCreditHandler, fundingLoanHandler).Result;
        /// <summary>
        /// Subscribe to funding information updates
        /// </summary>
        /// <param name="fundingOfferHandler">Subscribe to funding offer updates. Can be null if not interested</param>
        /// <param name="fundingCreditHandler">Subscribe to funding credit updates. Can be null if not interested</param>
        /// <param name="fundingLoanHandler">Subscribe to funding loan updates. Can be null if not interested</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToFundingUpdatesAsync(
            Action<BitfinexSocketEvent<IEnumerable<BitfinexFundingOffer>>> fundingOfferHandler,
            Action<BitfinexSocketEvent<IEnumerable<BitfinexFundingCredit>>> fundingCreditHandler,
            Action<BitfinexSocketEvent<IEnumerable<BitfinexFunding>>> fundingLoanHandler)
        {
            var tokenHandler = new Action<JToken>(tokenData =>
            {
                HandleAuthUpdate(tokenData, fundingOfferHandler, "FundingOffers");
                HandleAuthUpdate(tokenData, fundingCreditHandler, "FundingCredits");
                HandleAuthUpdate(tokenData, fundingLoanHandler, "FundingLoans");
            });

            return await Subscribe(null, "FundingOffers|FundingCredits|FundingLoans", true, tokenHandler).ConfigureAwait(false);
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
        /// <returns></returns>
        public CallResult<BitfinexOrder> PlaceOrder(OrderType type, string symbol, decimal amount, long? groupId = null, long? clientOrderId = null, decimal? price = null, decimal? priceTrailing = null, decimal? priceAuxiliaryLimit = null, decimal? priceOcoStop = null, OrderFlags? flags = null) =>
            PlaceOrderAsync(type, symbol, amount, groupId, clientOrderId, price, priceTrailing, priceAuxiliaryLimit, priceOcoStop, flags).Result;
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
        /// <returns></returns>
        public async Task<CallResult<BitfinexOrder>> PlaceOrderAsync(OrderType type, string symbol, decimal amount, long? groupId = null, long? clientOrderId = null, decimal? price = null, decimal? priceTrailing = null, decimal? priceAuxiliaryLimit = null, decimal? priceOcoStop = null, OrderFlags? flags = null)
        {
            log.Write(LogVerbosity.Info, "Going to place order");
            if (clientOrderId == null)
                clientOrderId = GenerateClientOrderId();

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
                PriceTrailing = priceTrailing
            });

            return await Query<BitfinexOrder>(query, true).ConfigureAwait(false);
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
        public CallResult<BitfinexOrder> UpdateOrder(long orderId, decimal? price = null, decimal? amount = null, decimal? delta = null, decimal? priceAuxiliaryLimit = null, decimal? priceTrailing = null, OrderFlags? flags = null) =>
            UpdateOrderAsync(orderId, price, amount, delta, priceAuxiliaryLimit, priceTrailing, flags).Result;

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
            log.Write(LogVerbosity.Info, "Going to update order " + orderId);
            var query = new BitfinexSocketQuery(orderId.ToString(), BitfinexEventType.OrderUpdate, new BitfinexUpdateOrder
            {
                OrderId = orderId,
                Amount = amount,
                Price = price,
                Flags = flags,
                PriceAuxiliaryLimit = priceAuxiliaryLimit?.ToString(CultureInfo.InvariantCulture),
                PriceTrailing = priceTrailing?.ToString(CultureInfo.InvariantCulture)
            });

            return await Query<BitfinexOrder>(query, true).ConfigureAwait(false);
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
        //    log.Write(LogVerbosity.Info, "Going to cancel all orders");
        //    var query = new BitfinexSocketQuery(null, BitfinexEventType.OrderCancelMulti, new BitfinexMultiCancel { All = true });

        //    return await Query<bool>(query, true).ConfigureAwait(false);
        //}

        /// <summary>
        /// Cancels an order
        /// </summary>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <returns></returns>
        public CallResult<BitfinexOrder> CancelOrder(long orderId) => CancelOrderAsync(orderId).Result;

        /// <summary>
        /// Cancels an order
        /// </summary>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <returns></returns>
        public async Task<CallResult<BitfinexOrder>> CancelOrderAsync(long orderId)
        {
            log.Write(LogVerbosity.Info, "Going to cancel order " + orderId);
            var query = new BitfinexSocketQuery(orderId.ToString(), BitfinexEventType.OrderCancel, new JObject { ["id"] = orderId });

            return await Query<BitfinexOrder>(query, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancels multiple orders based on their groupId
        /// </summary>
        /// <param name="groupOrderId">The group id to cancel</param>
        /// <returns>True if successfully committed on server</returns>
        public CallResult<bool> CancelOrdersByGroupId(long groupOrderId) => CancelOrdersByGroupIdAsync(groupOrderId).Result;

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
        public CallResult<bool> CancelOrdersByGroupIds(long[] groupOrderIds) => CancelOrdersByGroupIdsAsync(groupOrderIds).Result;

        /// <summary>
        /// Cancels multiple orders based on their groupIds
        /// </summary>
        /// <param name="groupOrderIds">The group ids to cancel</param>
        /// <returns>True if successfully committed on server</returns>
        public async Task<CallResult<bool>> CancelOrdersByGroupIdsAsync(long[] groupOrderIds)
        {
            return await CancelOrdersAsync(null, null, groupOrderIds.ToDictionary(v => v, k => (long?)null)).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancels multiple orders based on their order ids
        /// </summary>
        /// <param name="orderIds">The order ids to cancel</param>
        /// <returns>True if successfully committed on server</returns>
        public CallResult<bool> CancelOrders(long[] orderIds) => CancelOrdersAsync(orderIds).Result;

        /// <summary>
        /// Cancels multiple orders based on their order ids
        /// </summary>
        /// <param name="orderIds">The order ids to cancel</param>
        /// <returns>True if successfully committed on server</returns>
        public async Task<CallResult<bool>> CancelOrdersAsync(long[] orderIds)
        {
            return await CancelOrdersAsync(orderIds, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancels multiple orders based on their clientOrderIds
        /// </summary>
        /// <param name="clientOrderIds">The client order ids to cancel, listed as (clientOrderId, Day) pair. ClientOrderIds are unique per day, so timestamp should be provided</param>
        /// <returns>True if successfully committed on server</returns>
        public CallResult<bool> CancelOrdersByClientOrderIds(Dictionary<long, DateTime> clientOrderIds) => CancelOrdersByClientOrderIdsAsync(clientOrderIds).Result;

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
        private void HandleData<T>(string name, JArray dataArray, Action<T> handler)
        {
            var desResult = Deserialize<T>(dataArray);
            if (!desResult)
            {
                log.Write(LogVerbosity.Warning, $"Failed to deserialize {name} object: " + desResult.Error);
                return;
            }

            handler(desResult.Data);
        }

        private void HandleSingleToArrayData<T>(string name, JArray dataArray, Action<IEnumerable<T>> handler)
        {
            var desResult = Deserialize<T>(dataArray);
            if (!desResult)
            {
                log.Write(LogVerbosity.Warning, $"Failed to deserialize  {name} object: " + desResult.Error);
                return;
            }

            handler(new[] { desResult.Data });
        }

        private async Task<CallResult<bool>> CancelOrdersAsync(long[]? orderIds = null, Dictionary<long, DateTime>? clientOrderIds = null, Dictionary<long, long?>? groupOrderIds = null)
        {
            if (orderIds == null && clientOrderIds == null && groupOrderIds == null)
                return new CallResult<bool>(false, new ArgumentError("Either orderIds, clientOrderIds or groupOrderIds should be provided"));

            log.Write(LogVerbosity.Info, "Going to cancel multiple orders");
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
            return await Query<bool>(query, true).ConfigureAwait(false);
        }
        
        private void HandleAuthUpdate<T>(JToken token, Action<BitfinexSocketEvent<IEnumerable<T>>> action, string category)
        {
            var evntType = BitfinexEvents.EventMapping[(string)token[1]];
            var evnt = BitfinexEvents.Events.Single(e => e.EventType == evntType);
            if (evnt.Category != category)
                return;

            if (action == null)
            {
                log.Write(LogVerbosity.Debug, $"Ignoring {evnt.EventType} event because not subscribed");
                return;
            }

            IEnumerable<T> data;
            if (evnt.Single)
            {
                var result = Deserialize<T>(token[2]);
                if (!result)
                {
                    log.Write(LogVerbosity.Warning, "Failed to deserialize data: " + result.Error);
                    return;
                }
                data = new[] { result.Data };
            }
            else
            {
                var result = Deserialize<IEnumerable<T>>(token[2]);
                if (!result)
                {
                    log.Write(LogVerbosity.Warning, "Failed to deserialize data: " + result.Error);
                    return;
                }
                data = result.Data;
            }

            action(new BitfinexSocketEvent<IEnumerable<T>>(evntType, data));
        }


        private void InfoHandler(SocketConnection connection, JToken data)
        {
            var infoEvent = data.Type == JTokenType.Object && (string)data["event"] == "info";
            if (!infoEvent)
                return;

            log.Write(LogVerbosity.Debug, $"Info event received: {data}");
            if (data["code"] == null)
                return;

            var code = (int)data["code"];
            switch (code)
            {
                case 20051:
                    log.Write(LogVerbosity.Info, $"Code {code} received, reconnecting socket");
                    connection.Socket.Close(); // Closing it via socket will automatically reconnect
                    break;
                case 20060:
                    log.Write(LogVerbosity.Info, $"Code {code} received, entering maintenance mode");
                    connection.PausedActivity = true;
                    break;
                case 20061:
                    log.Write(LogVerbosity.Info, $"Code {code} received, leaving maintenance mode. Reconnecting/Resubscribing socket.");
                    connection.PausedActivity = false;
                    connection.Socket.Close(); // Closing it via socket will automatically reconnect
                    break;
                default:
                    log.Write(LogVerbosity.Warning, $"Unknown info code received: {code}");
                    break;
            }
        }

        /// <inheritdoc />
        protected override async Task<bool> Unsubscribe(SocketConnection connection, SocketSubscription subscription)
        {
            var channelId = ((BitfinexSubscriptionRequest) subscription.Request!).ChannelId;
            var unsub = new BitfinexUnsubscribeRequest(channelId);
            var result = false;
            await connection.SendAndWait(unsub, ResponseTimeout, data =>
            {
                if (data.Type != JTokenType.Object)
                    return false;

                var evnt = (string)data["event"];
                var channel = (string)data["chanId"];
                if (!int.TryParse(channel, out var chan))
                    return false;

                result = evnt == "unsubscribed" && channelId == chan;
                return result;
            }).ConfigureAwait(false);
            return result;
        }

        private BitfinexAuthentication GetAuthObject(params string[] filter)
        {
            var n = Nonce;
            var authentication = new BitfinexAuthentication
            {
                Event = "auth",
                ApiKey = authProvider!.Credentials.Key!.GetString(),
                Nonce = n,
                Payload = "AUTH" + n
            };
            if (filter.Any())
                authentication.Filter = filter;
            authentication.Signature = authProvider.Sign(authentication.Payload).ToLower();
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
        protected override async Task<CallResult<bool>> AuthenticateSocket(SocketConnection s)
        {
            if (authProvider == null)
                return new CallResult<bool>(false, new NoApiCredentialsError());

            var authObject = GetAuthObject();
            var result = new CallResult<bool>(false, new ServerError("No response from server"));
            await s.SendAndWait(authObject, ResponseTimeout, tokenData =>
            {
                if (tokenData.Type != JTokenType.Object)
                    return false;

                if ((string) tokenData["event"] != "auth")
                    return false;

                var authResponse = Deserialize<BitfinexAuthenticationResponse>(tokenData, false);
                if (!authResponse)
                {
                    log.Write(LogVerbosity.Warning, $"Socket {s.Socket.Id} authentication failed: " + authResponse.Error);
                    result = new CallResult<bool>(false, authResponse.Error);
                    return false;
                }

                if (authResponse.Data.Status != "OK")
                {
                    var error = new ServerError(authResponse.Data.ErrorCode, authResponse.Data.ErrorMessage ?? "-");
                    result = new CallResult<bool>(false, error);
                    log.Write(LogVerbosity.Debug, $"Socket {s.Socket.Id} authentication failed: " + error);
                    return false;
                }

                log.Write(LogVerbosity.Debug, $"Socket {s.Socket.Id} authentication completed");
                result = new CallResult<bool>(true, null);
                return true;
            }).ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc />
        protected override bool HandleQueryResponse<T>(SocketConnection s, object request, JToken data, out CallResult<T>? callResult)
        {
            callResult = null;
            if (data.Type != JTokenType.Array)
                return false;

            var array = (JArray) data;
            if (array.Count < 3)
                return false;

            var bfRequest = (BitfinexSocketQuery)request;
            var eventType = BitfinexEvents.EventMapping[(string)data[1]];

            if (eventType == BitfinexEventType.Notification)
            {
                var notificationData = (JArray) data[2];
                var notificationType = BitfinexEvents.EventMapping[(string) notificationData[1]];
                if (notificationType != BitfinexEventType.OrderNewRequest
                    && notificationType != BitfinexEventType.OrderCancelRequest
                    && notificationType != BitfinexEventType.OrderUpdateRequest
                    && notificationType != BitfinexEventType.OrderCancelMultiRequest)
                {
                    return false;
                }

                if (((string) notificationData[6]).ToLower() == "error")
                {
                    if (bfRequest.QueryType == BitfinexEventType.OrderNew && notificationType == BitfinexEventType.OrderNewRequest)
                    {
                        var orderData = notificationData[4];
                        if ((string) orderData[2] != bfRequest.Id)
                            return false;

                        callResult = new CallResult<T>(default, new ServerError((string)notificationData[7]));
                        return true;
                    }

                    if (bfRequest.QueryType == BitfinexEventType.OrderCancel && notificationType == BitfinexEventType.OrderCancelRequest)
                    {
                        var orderData = notificationData[4];
                        if ((string) orderData[0] != bfRequest.Id)
                            return false;

                        callResult = new CallResult<T>(default, new ServerError((string)notificationData[7]));
                        return true;
                    }

                    if (bfRequest.QueryType == BitfinexEventType.OrderUpdate && notificationType == BitfinexEventType.OrderUpdateRequest)
                    {
                        // OrderUpdateRequest not found notification doesn't carry the order id, where as OrderCancelRequest not found notification does..
                        // Anyway, can't check for ids, so just assume its for this one

                        callResult = new CallResult<T>(default, new ServerError((string)notificationData[7]));
                        return true;
                    }

                    if (bfRequest.QueryType == BitfinexEventType.OrderCancelMulti && notificationType == BitfinexEventType.OrderCancelMultiRequest)
                    {
                        callResult = new CallResult<T>(default, new ServerError((string)notificationData[7]));
                        return true;
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


            if (eventType == BitfinexEventType.OrderNew
                || eventType == BitfinexEventType.OrderUpdate
                || eventType == BitfinexEventType.OrderCancel)
            {
                if (bfRequest.QueryType == BitfinexEventType.OrderNew
                || bfRequest.QueryType == BitfinexEventType.OrderUpdate
                || bfRequest.QueryType == BitfinexEventType.OrderCancel)
                {
                    var orderData = array[2];
                    var dataOrderId = orderData[0].ToString();
                    var dataOrderClientId = orderData[2].ToString();
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

            return false;
        }

        /// <inheritdoc />
        protected override bool HandleSubscriptionResponse(SocketConnection s, SocketSubscription subscription, object request, JToken data, out CallResult<object>? callResult)
        {
            callResult = null;
            if (data.Type != JTokenType.Object)
                return false;

            var infoEvent = (string)data["event"] == "subscribed";
            var errorEvent = (string)data["event"] == "error";
            if (!infoEvent && !errorEvent)
                return false;

            if (infoEvent)
            {
                var subResponse = Deserialize<BitfinexSubscribeResponse>(data, false);
                if (!subResponse)
                {
                    callResult = new CallResult<object>(null, subResponse.Error);
                    log.Write(LogVerbosity.Warning, $"Socket {s.Socket.Id} subscription failed: " + subResponse.Error);
                    return false;
                }

                var bRequest = (BitfinexSubscriptionRequest) request;
                if (!bRequest.CheckResponse(data))
                    return false;
                
                log.Write(LogVerbosity.Debug, $"Socket {s.Socket.Id} subscription completed, {bRequest.Symbol} - {subResponse.Data.ChannelId}");
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
                    log.Write(LogVerbosity.Warning, $"Socket {s.Socket.Id} subscription failed: " + subResponse.Error);
                    return false;
                }

                var error = new ServerError(subResponse.Data.Code, subResponse.Data.Message);
                callResult = new CallResult<object>(null, error);
                log.Write(LogVerbosity.Debug, $"Socket {s.Socket.Id} subscription failed: " + error);
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
                    return (string)message["event"] == "info";
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
                    var eventTypeString = (string) array[1];
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
