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
    public class BitfinexSocketClient : SocketClient, IBitfinexSocketClient
    {
        #region fields
        private static BitfinexSocketClientOptions defaultOptions = new BitfinexSocketClientOptions();
        private static BitfinexSocketClientOptions DefaultOptions => defaultOptions.Copy();

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

        private TimeSpan socketResponseTimeout;
        private TimeSpan socketNoDataTimeout;

        private const string HeartbeatHandlerName = "HeartbeatHandler";
        private const string InfoHandlerName = "InfoHandlerName";
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
            Configure(options);
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
            var internalHandler = new Action<JToken>(data => HandleData("Ticker", (JArray)data[1], handler));
            return await Subscribe(new BitfinexSubscriptionRequest("ticker", symbol), internalHandler).ConfigureAwait(false);
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
        public CallResult<UpdateSubscription> SubscribeToBookUpdates(string symbol, Precision precision, Frequency frequency, int length, Action<BitfinexOrderBookEntry[]> handler) => SubscribeToBookUpdatesAsync(symbol, precision, frequency, length, handler).Result;
        /// <summary>
        /// Subscribes to order book updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="precision">The precision of the updates</param>
        /// <param name="frequency">The frequency of updates</param>
        /// <param name="length">The range for the order book updates</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToBookUpdatesAsync(string symbol, Precision precision, Frequency frequency, int length, Action<BitfinexOrderBookEntry[]> handler)
        {
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
            return await Subscribe(sub, internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribes to raw order book updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="limit">The range for the order book updates</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        public CallResult<UpdateSubscription> SubscribeToRawBookUpdates(string symbol, int limit, Action<BitfinexRawOrderBookEntry[]> handler) => SubscribeToRawBookUpdatesAsync(symbol, limit, handler).Result;
        /// <summary>
        /// Subscribes to raw order book updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="limit">The range for the order book updates</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToRawBookUpdatesAsync(string symbol, int limit, Action<BitfinexRawOrderBookEntry[]> handler)
        {
            var internalHandler = new Action<JToken>(data =>
            {
                var dataArray = (JArray)data[1];
                if(dataArray[0].Type == JTokenType.Array)
                    HandleData("Raw book snapshot", dataArray, handler);                
                else
                    HandleSingleToArrayData("Raw book update", dataArray, handler);           
            });
            return await Subscribe(new BitfinexRawBookSubscriptionRequest(symbol, "R0", limit), internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribes to public trade updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        public CallResult<UpdateSubscription> SubscribeToTradeUpdates(string symbol, Action<BitfinexTradeSimple[]> handler) => SubscribeToTradeUpdatesAsync(symbol, handler).Result;
        /// <summary>
        /// Subscribes to public trade updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<BitfinexTradeSimple[]> handler)
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
                    if (!desResult.Success)
                    {
                        log.Write(LogVerbosity.Warning, "Failed to deserialize trade object: " + desResult.Error);
                        return;
                    }
                    desResult.Data.UpdateType = BitfinexEvents.EventMapping[(string)arr[1]];
                    handler(new[] { desResult.Data });
                }
            });
            return await Subscribe(new BitfinexSubscriptionRequest("trades", symbol), internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribes to candle updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="interval">The interval of the candles</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        public CallResult<UpdateSubscription> SubscribeToCandleUpdates(string symbol, TimeFrame interval, Action<BitfinexCandle[]> handler) => SubscribeToCandleUpdatesAsync(symbol, interval, handler).Result;
        /// <summary>
        /// Subscribes to candle updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="interval">The interval of the candles</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToCandleUpdatesAsync(string symbol, TimeFrame interval, Action<BitfinexCandle[]> handler)
        {
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
            return await Subscribe(new BitfinexKlineSubscriptionRequest(symbol, JsonConvert.SerializeObject(interval, new TimeFrameConverter(false))), internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to trading information updates
        /// </summary>
        /// <param name="orderHandler">Data handler for order updates. Can be null if not interested</param>
        /// <param name="tradeHandler">Data handler for trade execution updates. Can be null if not interested</param>
        /// <param name="positionHandler">Data handler for position updates. Can be null if not interested</param>
        /// <returns></returns>
        public CallResult<UpdateSubscription> SubscribeToTradingUpdates(
            Action<BitfinexSocketEvent<BitfinexOrder[]>> orderHandler, 
            Action<BitfinexSocketEvent<BitfinexTradeDetails[]>> tradeHandler,
            Action<BitfinexSocketEvent<BitfinexPosition[]>> positionHandler) => SubscribeToTradingUpdatesAsync(orderHandler, tradeHandler, positionHandler).Result;
        /// <summary>
        /// Subscribe to trading information updates
        /// </summary>
        /// <param name="orderHandler">Data handler for order updates. Can be null if not interested</param>
        /// <param name="tradeHandler">Data handler for trade execution updates. Can be null if not interested</param>
        /// <param name="positionHandler">Data handler for position updates. Can be null if not interested</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToTradingUpdatesAsync(
            Action<BitfinexSocketEvent<BitfinexOrder[]>> orderHandler, 
            Action<BitfinexSocketEvent<BitfinexTradeDetails[]>> tradeHandler,
             Action<BitfinexSocketEvent<BitfinexPosition[]>> positionHandler)
        {
            var tokenHandler = new Action<BitfinexSocketEvent<JToken>>(tokenData =>
            {
                HandleAuthUpdate(tokenData, orderHandler, BitfinexEvents.GetEventsForCategory("Orders"));
                HandleAuthUpdate(tokenData, tradeHandler, BitfinexEvents.GetEventsForCategory("Trades"));
                HandleAuthUpdate(tokenData, positionHandler, BitfinexEvents.GetEventsForCategory("Positions"));
            });

            return await SubscribeAuth("trading", tokenHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to wallet information updates
        /// </summary>
        /// <param name="walletHandler">Data handler for wallet updates</param>
        /// <returns></returns>
        public CallResult<UpdateSubscription> SubscribeToWalletUpdates(Action<BitfinexSocketEvent<BitfinexWallet[]>> walletHandler) => SubscribeToWalletUpdatesAsync(walletHandler).Result;
        /// <summary>
        /// Subscribe to wallet information updates
        /// </summary>
        /// <param name="walletHandler">Data handler for wallet updates</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToWalletUpdatesAsync( Action<BitfinexSocketEvent<BitfinexWallet[]>> walletHandler)
        {
            var tokenHandler = new Action<BitfinexSocketEvent<JToken>>(tokenData =>
            {
                HandleAuthUpdate(tokenData, walletHandler, BitfinexEvents.GetEventsForCategory("Wallet"));
            });

            return await SubscribeAuth("wallet", tokenHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to funding information updates
        /// </summary>
        /// <param name="fundingOfferHandler">Subscribe to funding offer updates. Can be null if not interested</param>
        /// <param name="fundingCreditHandler">Subscribe to funding credit updates. Can be null if not interested</param>
        /// <param name="fundingLoanHandler">Subscribe to funding loan updates. Can be null if not interested</param>
        /// <returns></returns>
        public CallResult<UpdateSubscription> SubscribeToFundingUpdates(
            Action<BitfinexSocketEvent<BitfinexFundingOffer[]>> fundingOfferHandler,
            Action<BitfinexSocketEvent<BitfinexFundingCredit[]>> fundingCreditHandler,
            Action<BitfinexSocketEvent<BitfinexFundingLoan[]>> fundingLoanHandler) => SubscribeToFundingUpdatesAsync(fundingOfferHandler, fundingCreditHandler, fundingLoanHandler).Result;
        /// <summary>
        /// Subscribe to funding information updates
        /// </summary>
        /// <param name="fundingOfferHandler">Subscribe to funding offer updates. Can be null if not interested</param>
        /// <param name="fundingCreditHandler">Subscribe to funding credit updates. Can be null if not interested</param>
        /// <param name="fundingLoanHandler">Subscribe to funding loan updates. Can be null if not interested</param>
        /// <returns></returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToFundingUpdatesAsync(
            Action<BitfinexSocketEvent<BitfinexFundingOffer[]>> fundingOfferHandler,
            Action<BitfinexSocketEvent<BitfinexFundingCredit[]>> fundingCreditHandler,
            Action<BitfinexSocketEvent<BitfinexFundingLoan[]>> fundingLoanHandler)
        {
            var tokenHandler = new Action<BitfinexSocketEvent<JToken>>(tokenData =>
            {
                HandleAuthUpdate(tokenData, fundingOfferHandler, BitfinexEvents.GetEventsForCategory("FundingOffers"));
                HandleAuthUpdate(tokenData, fundingCreditHandler, BitfinexEvents.GetEventsForCategory("FundingCredits"));
                HandleAuthUpdate(tokenData, fundingLoanHandler, BitfinexEvents.GetEventsForCategory("FundingLoans"));
            });

            return await SubscribeAuth("funding", tokenHandler).ConfigureAwait(false);
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
                PriceAuxiliaryLimit = priceAuxiliaryLimit.HasValue ? priceAuxiliaryLimit.Value.ToString(CultureInfo.InvariantCulture) : null,
                PriceTrailing = priceTrailing.HasValue ? priceTrailing.Value.ToString(CultureInfo.InvariantCulture): null
            });

            return await Query<BitfinexOrder>(query, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancel all open orders
        /// </summary>
        /// <returns></returns>
        public CallResult<bool> CancelAllOrders() => CancelAllOrdersAsync().Result;
        /// <summary>
        /// Cancel all open orders
        /// </summary>
        /// <returns></returns>
        public async Task<CallResult<bool>> CancelAllOrdersAsync()
        {
            // Doesn't seem to work even though it is implemented as described at https://docs.bitfinex.com/v2/reference#ws-input-order-cancel-multi 
            log.Write(LogVerbosity.Info, "Going to cancel all orders");
            var query = new BitfinexSocketQuery(null, BitfinexEventType.OrderCancelMulti, new BitfinexMultiCancel { All = true });

            return await Query<bool>(query, true).ConfigureAwait(false);
        }

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
            if (!desResult.Success)
            {
                log.Write(LogVerbosity.Warning, $"Failed to deserialize {name} object: " + desResult.Error);
                return;
            }

            handler(desResult.Data);
        }

        private void HandleSingleToArrayData<T>(string name, JArray dataArray, Action<T[]> handler)
        {
            var desResult = Deserialize<T>(dataArray);
            if (!desResult.Success)
            {
                log.Write(LogVerbosity.Warning, $"Failed to deserialize  {name} object: " + desResult.Error);
                return;
            }

            handler(new[] { desResult.Data });
        }

        private async Task<CallResult<bool>> CancelOrdersAsync(long[] orderIds = null, Dictionary<long, DateTime> clientOrderIds = null, Dictionary<long, long?> groupOrderIds = null)
        {
            if (orderIds == null && clientOrderIds == null && groupOrderIds == null)
                return new CallResult<bool>(false, new ArgumentError("Either orderIds, clientOrderIds or groupOrderIds should be provided"));

            log.Write(LogVerbosity.Info, "Going to cancel multiple orders");
            var cancelObject = new BitfinexMultiCancel {OrderIds = orderIds};
            if (clientOrderIds != null)
            {
                cancelObject.ClientIds = new object[clientOrderIds.Count][];
                for(var i = 0; i < cancelObject.ClientIds.Length; i++)
                {
                    cancelObject.ClientIds[i] = new object[]
                    {
                        clientOrderIds.ElementAt(i).Key,
                        clientOrderIds.ElementAt(i).Value.ToString("yyyy-MM-dd")
                    };
                }
            }
            if(groupOrderIds != null)
                cancelObject.GroupIds = new[] { groupOrderIds.Select(g => g.Key).ToArray() };

            var query = new BitfinexSocketQuery(null, BitfinexEventType.OrderCancelMulti, cancelObject);
            return await Query<bool>(query, true).ConfigureAwait(false);
        }

        private async Task<CallResult<T>> Query<T>(BitfinexSocketQuery request, bool signed)
        {
            CallResult<T> result = null;
            var internalHandler = new Action<BitfinexSocketEvent<JToken>>(data => result = Deserialize<T>(data.Data));

            var subscription = GetBackgroundSocket(signed);
            if (subscription == null)
            {
                // We don't have a background socket to query, create a new one
                var connectResult = await CreateAndConnectSocketAuth(internalHandler, false).ConfigureAwait(false);
                if (!connectResult.Success)
                    return new CallResult<T>(default(T), connectResult.Error);

                subscription = connectResult.Data;
                subscription.Type = signed ? SocketType.BackgroundAuthenticated : SocketType.Background;

                if (signed)
                {
                    var authResult = await Authenticate(subscription, "trading", "notify").ConfigureAwait(false);
                    if (!authResult.Success)
                    {
                        await subscription.Close().ConfigureAwait(false);
                        return new CallResult<T>(default(T), authResult.Error);
                    }
                }
            }
            else
            {
                // Use earlier created background socket to query without having to connect again
                subscription.Events.Single(s => s.Name == DataEvent).Reset();
                subscription.MessageHandlers[DataHandlerName] = (subs, data) => DataHandlerQuery((BitfinexSocketSubscription)subs, data, internalHandler);
            }

            var bitfinexSubscription = (BitfinexSocketSubscription) subscription;
            if (bitfinexSubscription.MaintenanceMode)
            {
                log.Write(LogVerbosity.Info, "Socket in maintenance mode, canceling query");
                return new CallResult<T>(default(T), new ServerError("Socket in maintenance mode, try again after the socket is reconnected"));
            }

            subscription.Request = request;
            var waitTask = subscription.WaitForEvent(DataEvent, request.Id, socketResponseTimeout);
            Send(subscription.Socket, request);
            var dataResult = await waitTask.ConfigureAwait(false);
            if(!dataResult.Success)
                subscription.SetEventById(request.Id, false, dataResult.Error);

            return !dataResult.Success ? new CallResult<T>(default(T), dataResult.Error) : result;
        }

        private async Task<CallResult<UpdateSubscription>> Subscribe(BitfinexSubscriptionRequest request, Action<JToken> onData)
        {
            var connectResult = await CreateAndConnectSocket(onData).ConfigureAwait(false);
            if (!connectResult.Success)
                return new CallResult<UpdateSubscription>(null, connectResult.Error);

            return await Subscribe(connectResult.Data, request).ConfigureAwait(false);
        }

        private async Task<CallResult<UpdateSubscription>> Subscribe(SocketSubscription subscription, BitfinexSubscriptionRequest request)
        {
            var waitTask = subscription.WaitForEvent(SubscriptionEvent, socketResponseTimeout);
            Send(subscription.Socket, request);

            subscription.Request = request;
            var subResult = await waitTask.ConfigureAwait(false);
            if (!subResult.Success)
            {
                await subscription.Close().ConfigureAwait(false);
                return new CallResult<UpdateSubscription>(null, subResult.Error);
            }

            subscription.Socket.ShouldReconnect = true;
            return new CallResult<UpdateSubscription>(new UpdateSubscription(subscription), null);
        }

        private async Task<CallResult<UpdateSubscription>> SubscribeAuth(string filter, Action<BitfinexSocketEvent<JToken>> onData)
        {
            var connectResult = await CreateAndConnectSocketAuth(onData, true).ConfigureAwait(false);
            if (!connectResult.Success)
                return new CallResult<UpdateSubscription>(null, connectResult.Error);

            var result = await Authenticate(connectResult.Data, filter).ConfigureAwait(false);
            if(!result.Success)            
                await connectResult.Data.Close().ConfigureAwait(false);
            else
                connectResult.Data.Socket.ShouldReconnect = true;
            return result;
        }

        private async Task<CallResult<UpdateSubscription>> Authenticate(SocketSubscription subscription, params string[] filter)
        {
            if(authProvider == null)
                return new CallResult<UpdateSubscription>(null, new NoApiCredentialsError());

            var authObject = GetAuthObject(filter);
            var waitTask = subscription.WaitForEvent(AuthenticationEvent, socketResponseTimeout);
            Send(subscription.Socket, authObject);

            subscription.Request = authObject;            
            var subResult = await waitTask.ConfigureAwait(false);
            if (!subResult.Success)
                return new CallResult<UpdateSubscription>(null, subResult.Error);            

            return new CallResult<UpdateSubscription>(new UpdateSubscription(subscription), null);
        }

        private void HandleAuthUpdate<T>(BitfinexSocketEvent<JToken> token, Action<BitfinexSocketEvent<T[]>> action, IEnumerable<BitfinexEvent> events)
        {
            var evnt = events.SingleOrDefault(e => e.EventType == token.EventType);
            if (evnt == null)
                return;

            if (action == null)
            {
                log.Write(LogVerbosity.Debug, $"Ignoring {token.EventType} event because not subscribed");
                return;
            }

            T[] data;
            if (evnt.Single)
            {
                var result = Deserialize<T>(token.Data);
                if (!result.Success)
                {
                    log.Write(LogVerbosity.Warning, "Failed to deserialize data: " + result.Error);
                    return;
                }
                data = new[] { result.Data };
            }
            else
            {
                var result = Deserialize<T[]>(token.Data);
                if (!result.Success)
                {
                    log.Write(LogVerbosity.Warning, "Failed to deserialize data: " + result.Error);
                    return;
                }
                data = result.Data;
            }

            action(new BitfinexSocketEvent<T[]>(token.EventType, data));
        }

        private async Task<CallResult<BitfinexSocketSubscription>> CreateAndConnectSocket(Action<JToken> onMessage)
        {
            var socket = CreateSocket(BaseAddress);
            socket.Timeout = socketNoDataTimeout;
            var subscription = new BitfinexSocketSubscription(socket);
            subscription.MessageHandlers.Add(HeartbeatHandlerName, HeartbeatHandler);
            subscription.MessageHandlers.Add(DataHandlerName, (subs, data) => DataHandler(subs, data, onMessage));

            subscription.MessageHandlers.Add(SubscriptionHandlerName, SubscriptionHandler);
            subscription.MessageHandlers.Add(InfoHandlerName, InfoHandler);
            subscription.AddEvent(SubscriptionEvent);

            var connectResult = await ConnectSocket(subscription).ConfigureAwait(false);
            if (!connectResult.Success)
                return new CallResult<BitfinexSocketSubscription>(null, connectResult.Error);

            return new CallResult<BitfinexSocketSubscription>(subscription, null);
        }

        private async Task<CallResult<BitfinexSocketSubscription>> CreateAndConnectSocketAuth(Action<BitfinexSocketEvent<JToken>> onMessage, bool subscribing)
        {
            var socket = CreateSocket(BaseAddress);
            socket.Timeout = socketNoDataTimeout;
            var subscription = new BitfinexSocketSubscription(socket);
            subscription.MessageHandlers.Add(HeartbeatHandlerName, HeartbeatHandler);
            if (subscribing)
                subscription.MessageHandlers.Add(DataHandlerName, (subs, data) => DataHandlerAuth(data, onMessage));
            else
            {
                subscription.MessageHandlers.Add(DataHandlerName, (subs, data) => DataHandlerQuery((BitfinexSocketSubscription)subs, data, onMessage));
                subscription.AddEvent(DataEvent);
            }

            subscription.MessageHandlers.Add(AuthenticationHandlerName, AuthenticationHandler);
            subscription.AddEvent(AuthenticationEvent);
            subscription.MessageHandlers.Add(InfoHandlerName, InfoHandler);

            var connectResult = await ConnectSocket(subscription).ConfigureAwait(false);
            if (!connectResult.Success)
                return new CallResult<BitfinexSocketSubscription>(null, connectResult.Error);

            return new CallResult<BitfinexSocketSubscription>(subscription, null);
        }


        private bool InfoHandler(SocketSubscription subscription, JToken data)
        {
            var infoEvent = data.Type == JTokenType.Object && (string)data["event"] == "info";
            if (!infoEvent)
                return false;

            log.Write(LogVerbosity.Debug, $"Info event received: {data}");
            if (data["code"] == null)
                return true;

            var code = (int) data["code"];
            switch (code)
            {
                case 20051:
                    log.Write(LogVerbosity.Info, $"Code {code} received, reconnecting socket");
                    subscription.Socket.Close(); // Closing it via socket will automatically reconnect
                    break;
                case 20060:
                    log.Write(LogVerbosity.Info, $"Code {code} received, entering maintenance mode");
                    ((BitfinexSocketSubscription) subscription).MaintenanceMode = true;
                    break;
                case 20061:
                    log.Write(LogVerbosity.Info, $"Code {code} received, leaving maintenance mode. Reconnecting/Resubscribing socket.");
                    ((BitfinexSocketSubscription)subscription).MaintenanceMode = false;
                    subscription.Socket.Close(); // Closing it via socket will automatically reconnect
                    break;
                default:
                    log.Write(LogVerbosity.Warning, $"Unknown info code received: {code}");
                    break;
            }

            return true;
        }

        private bool HeartbeatHandler(SocketSubscription subscription, JToken data)
        {
            return data.Type == JTokenType.Array && data[1].Type == JTokenType.String && (string)data[1] == "hb";
        }

        private bool AuthenticationHandler(SocketSubscription subscription, JToken data)
        {
            if (data.Type != JTokenType.Object)
                return false;

            var authEvent = (string)data["event"] == "auth";
            if (!authEvent)
                return false;

            var authResponse = Deserialize<BitfinexAuthenticationResponse>(data, false);
            if (!authResponse.Success)
            {
                log.Write(LogVerbosity.Warning, $"Socket {subscription.Socket.Id} authentication failed: " + authResponse.Error);
                subscription.SetEventByName(AuthenticationEvent, false, authResponse.Error);
                return true;
            }

            if (authResponse.Data.Status != "OK")
            {
                var error = new ServerError(authResponse.Data.ErrorCode, authResponse.Data.ErrorMessage);
                log.Write(LogVerbosity.Debug, $"Socket {subscription.Socket.Id} authentication failed: " + error);
                subscription.SetEventByName(AuthenticationEvent, false, error);
                return true;
            }

            log.Write(LogVerbosity.Debug, $"Socket {subscription.Socket.Id} authentication completed");
            subscription.SetEventByName(AuthenticationEvent, true, null);
            return true;
        }

        private bool SubscriptionHandler(SocketSubscription subscription, JToken data)
        {
            if (data.Type != JTokenType.Object)
                return false;

            var infoEvent = (string)data["event"] == "subscribed";
            var errorEvent = (string)data["event"] == "error";
            if (!infoEvent && !errorEvent)
                return false;

            var evnt = subscription.GetWaitingEvent(SubscriptionEvent);
            if (evnt == null)
                return false;

            if (infoEvent)
            {
                var subResponse = Deserialize<BitfinexSubscribeResponse>(data, false);
                if (!subResponse.Success)
                {
                    log.Write(LogVerbosity.Warning, $"Socket {subscription.Socket.Id} subscription failed: " + subResponse.Error);
                    subscription.SetEventByName(SubscriptionEvent, false, subResponse.Error);
                    return true;
                }

                log.Write(LogVerbosity.Debug, $"Socket {subscription.Socket.Id} subscription completed");
                ((BitfinexSubscriptionRequest)subscription.Request).ChannelId = subResponse.Data.ChannelId;
                subscription.SetEventByName(SubscriptionEvent, true, null);
                return true;
            }
            else
            {
                var subResponse = Deserialize<BitfinexErrorResponse>(data, false);
                if (!subResponse.Success)
                {
                    log.Write(LogVerbosity.Warning, $"Socket {subscription.Socket.Id} subscription failed: " + subResponse.Error);
                    subscription.SetEventByName(SubscriptionEvent, false, subResponse.Error);
                    return true;
                }

                var error = new ServerError(subResponse.Data.Code, subResponse.Data.Message);
                log.Write(LogVerbosity.Debug, $"Socket {subscription.Socket.Id} subscription failed: " + error);
                subscription.SetEventByName(SubscriptionEvent, false, error);
                return true;
            }
        }

        private bool DataHandler(SocketSubscription subscription, JToken data, Action<JToken> handler)
        {
            var dataObject = data.Type == JTokenType.Array;
            if (!dataObject)
                return false;

            var channelId = (int)((JArray)data)[0];
            var subId = ((BitfinexSubscriptionRequest)subscription.Request).ChannelId;
            if (channelId != subId)
                return false;

            handler(data);
            return true;          
        }

        private bool DataHandlerAuth(JToken data, Action<BitfinexSocketEvent<JToken>> handler)
        {
            var dataObject = data.Type == JTokenType.Array;
            if (!dataObject)
                return false;

            var eventTypeString = (string)data[1];
            var eventType = BitfinexEvents.EventMapping[eventTypeString];
            var result = new BitfinexSocketEvent<JToken> {EventType = eventType, Data = data[2]};
            handler(result);
            return true;
        }

        private bool DataHandlerQuery(BitfinexSocketSubscription subscription, JToken data, Action<BitfinexSocketEvent<JToken>> handler)
        {
            var dataObject = data.Type == JTokenType.Array;
            if (!dataObject)
                return false;

            var evnt = subscription.GetWaitingEvent(DataEvent);
            if (evnt == null)
                return false;

            var request = (BitfinexSocketQuery)subscription.Request;
            var eventType = BitfinexEvents.EventMapping[(string)data[1]];
            if (eventType == BitfinexEventType.Notification)
            {
                var notificationData = (JArray)data[2];
                var notificationType = BitfinexEvents.EventMapping[(string)notificationData[1]];
                if (notificationType != BitfinexEventType.OrderNewRequest 
                 && notificationType != BitfinexEventType.OrderCancelRequest
                 && notificationType != BitfinexEventType.OrderUpdateRequest
                 && notificationType != BitfinexEventType.OrderCancelMultiRequest)
                    return false;

                if (((string)notificationData[6]).ToLower() == "error")
                {
                    if(request.QueryType == BitfinexEventType.OrderNew && notificationType == BitfinexEventType.OrderNewRequest)
                    {
                        var orderData = notificationData[4];
                        if ((string)orderData[2] != request.Id)
                            return false;

                        subscription.SetEventByName(DataEvent, false, new ServerError((string)notificationData[7]));
                        return true;
                    }
                    if(request.QueryType == BitfinexEventType.OrderCancel && notificationType == BitfinexEventType.OrderCancelRequest)
                    {
                        var orderData = notificationData[4];
                        if ((string)orderData[0] != request.Id)
                            return false;

                        subscription.SetEventByName(DataEvent, false, new ServerError((string)notificationData[7]));
                        return true;
                    }                    
                    if(request.QueryType == BitfinexEventType.OrderUpdate && notificationType == BitfinexEventType.OrderUpdateRequest)
                    {
                        // OrderUpdateRequest not found notification doesn't carry the order id, where as OrderCancelRequest not found notification does..
                        // Anyway, can't check for ids, so just assume its for this one

                        subscription.SetEventByName(DataEvent, false, new ServerError((string)notificationData[7]));
                        return true;
                    }
                    if(request.QueryType == BitfinexEventType.OrderCancelMulti && notificationType == BitfinexEventType.OrderCancelMultiRequest)
                    {
                        subscription.SetEventByName(DataEvent, false, new ServerError((string)notificationData[7]));
                        return true;
                    }
                }

                if(notificationType == BitfinexEventType.OrderCancelMultiRequest)
                {
                    handler(new BitfinexSocketEvent<JToken>
                    {
                        EventType = BitfinexEventType.OrderCancelMulti,
                        Data = JToken.Parse("true")
                    });
                    subscription.SetEventByName(DataEvent, true, null);
                    return true;
                }

                return false;
            }

            if (eventType == BitfinexEventType.OrderNew
                || eventType == BitfinexEventType.OrderUpdate
                || eventType == BitfinexEventType.OrderCancel)
            {
                var orderData = (JArray)data[2];
                if ((string)orderData[2] != request.Id && (string)orderData[0] != request.Id)
                    return false;

                var resultEvnt = new BitfinexSocketEvent<JToken> { EventType = eventType, Data = orderData };
                handler(resultEvnt);
                subscription.SetEventByName(DataEvent, true, null);
            }

            return true;
        }

        private BitfinexAuthentication GetAuthObject(params string[] filter)
        {
            var n = Nonce;
            var authentication = new BitfinexAuthentication
            {
                Event = "auth",
                ApiKey = authProvider.Credentials.Key.GetString(),
                Nonce = n,
                Payload = "AUTH" + n
            };
            if (filter.Any())
                authentication.Filter = filter;
            authentication.Signature = authProvider.Sign(authentication.Payload).ToLower();
            return authentication;
        }

        protected override bool SocketReconnect(SocketSubscription subscription, TimeSpan disconnectedTime)
        {
            if (subscription.Request is BitfinexAuthentication request)
            {
                var resubResult = Authenticate(subscription, request.Filter).Result;
                if (!resubResult.Success)
                    return false;
            }
            else
            {
                var resubResult = Subscribe(subscription, (BitfinexSubscriptionRequest)subscription.Request).Result;
                if (!resubResult.Success)
                    return false;
            }
            return true;
        }

        private long GenerateClientOrderId()
        {
            var buffer = new byte[8];
            random.NextBytes(buffer);
            return (long)Math.Round(Math.Abs(BitConverter.ToInt32(buffer, 0)) / 1000m);
        }

        private void Configure(BitfinexSocketClientOptions options)
        {
            socketResponseTimeout = options.SocketResponseTimeout;
            socketNoDataTimeout = options.SocketNoDataTimeout;
        }
        #endregion
    }
}
