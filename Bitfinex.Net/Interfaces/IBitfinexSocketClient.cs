using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.SocketObjects;
using CryptoExchange.Net;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.RateLimiter;

namespace Bitfinex.Net.Interfaces
{
    public interface IBitfinexSocketClient
    {
        SocketState State { get; }
        IWebsocketFactory SocketFactory { get; set; }
        IRequestFactory RequestFactory { get; set; }

        /// <summary>
        /// Happens when the server signals the client to pause activity
        /// </summary>
        event Action SocketPaused;

        /// <summary>
        /// Hapens when the server signals the client it can resume activity
        /// </summary>
        event Action SocketResumed;

        /// <summary>
        /// Happens when the socket loses connection to the server
        /// </summary>
        event Action ConnectionLost;

        /// <summary>
        /// Happens when connection to the server is restored after it was lost
        /// </summary>
        event Action ConnectionRestored;

        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        void SetApiCredentials(string apiKey, string apiSecret);

        /// <summary>
        /// Connect to the websocket and start processing data
        /// </summary>
        /// <returns></returns>
        bool Start();

        /// <summary>
        /// Disconnect from the socket and clear all subscriptions
        /// </summary>
        void Stop();

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexSocketClient.PlaceOrderAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexOrder> PlaceOrder(OrderType type, string symbol, decimal amount, long? groupId = null, long? clientOrderId = null, decimal? price = null, decimal? priceTrailing = null, decimal? priceAuxiliaryLimit = null, decimal? priceOcoStop = null, OrderFlags? flags = null);

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
        /// <param name="priceOcoStop">Oco stop price of ther order</param>
        /// <param name="flags">Additional flags</param>
        /// <returns></returns>
        Task<CallResult<BitfinexOrder>> PlaceOrderAsync(OrderType type, string symbol, decimal amount, long? groupId = null, long? clientOrderId = null, decimal? price = null, decimal? priceTrailing = null, decimal? priceAuxiliaryLimit = null, decimal? priceOcoStop = null, OrderFlags? flags = null);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexSocketClient.CancelOrderAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<bool> CancelOrder(long orderId);

        /// <summary>
        /// Cancels an order
        /// </summary>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <returns></returns>
        Task<CallResult<bool>> CancelOrderAsync(long orderId);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexSocketClient.CancelOrdersByGroupIdAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<bool> CancelOrdersByGroupId(long groupOrderId);

        /// <summary>
        /// Cancels multiple orders based on their groupId
        /// </summary>
        /// <param name="groupOrderId">The group id to cancel</param>
        /// <returns>True if successfully committed on server</returns>
        Task<CallResult<bool>> CancelOrdersByGroupIdAsync(long groupOrderId);

        /// <summary>
        /// Synchronized version of the <see cref="CancelOrdersByGroupIdsAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<bool> CancelOrdersByGroupIds(long[] groupOrderIds);

        /// <summary>
        /// Cancels multiple orders based on their groupIds
        /// </summary>
        /// <param name="groupOrderIds">The group ids to cancel</param>
        /// <returns>True if successfully committed on server</returns>
        Task<CallResult<bool>> CancelOrdersByGroupIdsAsync(long[] groupOrderIds);

        /// <summary>
        /// Synchronized version of the <see cref="CancelOrdersByGroupIds"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<bool> CancelOrdersByGroupIds(Dictionary<long, long?> groupOrderIds);

        /// <summary>
        /// Cancels multiple orders based on their groupIds
        /// </summary>
        /// <param name="groupOrderIds">The group ids to cancel, listed as (GroupId, MaxOrderId) pair. If MaxOrderId is provided all order ids which are in the group id with higher orderIds than the MaxOrderId won't get canceled</param>
        /// <returns>True if successfully committed on server</returns>
        Task<CallResult<bool>> CancelOrdersByGroupIdsAsync(Dictionary<long, long?> groupOrderIds);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexSocketClient.CancelOrdersByOrderIdsAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<bool> CancelOrdersByOrderIds(long[] orderIds);

        /// <summary>
        /// Cancels multiple orders based on their order ids
        /// </summary>
        /// <param name="orderIds">The order ids to cancel</param>
        /// <returns>True if successfully committed on server</returns>
        Task<CallResult<bool>> CancelOrdersByOrderIdsAsync(long[] orderIds);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexSocketClient.CancelOrdersByClientOrderIdsAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<bool> CancelOrdersByClientOrderIds(Dictionary<long, DateTime> clientOrderIds);

        /// <summary>
        /// Cancels multiple orders based on their clientOrderIds
        /// </summary>
        /// <param name="clientOrderIds">The client order ids to cancel, listed as (clientOrderId, Day) pair. ClientOrderIds are unique per day, so timestamp should be provided</param>
        /// <returns>True if successfully committed on server</returns>
        Task<CallResult<bool>> CancelOrdersByClientOrderIdsAsync(Dictionary<long, DateTime> clientOrderIds);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexSocketClient.CancelOrderAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<bool> UpdateOrder(long orderId, decimal? price = null, decimal? amount = null, decimal? delta = null, decimal? priceAuxiliaryLimit = null, decimal? priceTrailing = null, OrderFlags? flags = null);

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
        Task<CallResult<bool>> UpdateOrderAsync(long orderId, decimal? price = null, decimal? amount = null, decimal? delta = null, decimal? priceAuxiliaryLimit = null, decimal? priceTrailing = null, OrderFlags? flags = null);

        /// <summary>
        /// Subscribes to wallet updates. A snapshot will be send when opening the socket so consider subscribing to this before opening the socket.
        /// </summary>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        CallResult<int> SubscribeToWalletUpdates(Action<BitfinexSocketEvent<BitfinexWallet[]>> handler);

        /// <summary>
        /// Subscribes to order updates. A snapshot will be send when opening the socket so consider subscribing to this before opening the socket.
        /// </summary>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        CallResult<int> SubscribeToOrderUpdates(Action<BitfinexSocketEvent<BitfinexOrder[]>> handler);

        /// <summary>
        /// Subscribes to position updates. A snapshot will be send when opening the socket so consider subscribing to this before opening the socket.
        /// </summary>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        CallResult<int> SubscribeToPositionUpdates(Action<BitfinexSocketEvent<BitfinexPosition[]>> handler);

        /// <summary>
        /// Subscribes to trade updates. A snapshot will be send when opening the socket so consider subscribing to this before opening the socket.
        /// </summary>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        CallResult<int> SubscribeToTradeUpdates(Action<BitfinexSocketEvent<BitfinexTradeDetails[]>> handler);

        /// <summary>
        /// Subscribes to funding offer updates. A snapshot will be send when opening the socket so consider subscribing to this before opening the socket.
        /// </summary>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        CallResult<int> SubscribeToFundingOfferUpdates(Action<BitfinexSocketEvent<BitfinexFundingOffer[]>> handler);

        /// <summary>
        /// Subscribes to funding credits updates. A snapshot will be send when opening the socket so consider subscribing to this before opening the socket.
        /// </summary>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        CallResult<int> SubscribeToFundingCreditsUpdates(Action<BitfinexSocketEvent<BitfinexFundingCredit[]>> handler);

        /// <summary>
        /// Subscribes to funding loans updates. A snapshot will be send when opening the socket so consider subscribing to this before opening the socket.
        /// </summary>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        CallResult<int> SubscribeToFundingLoansUpdates(Action<BitfinexSocketEvent<BitfinexFundingLoan[]>> handler);

        /// <summary>
        /// Subscribes to ticker updates for a symbol. Requires socket to be connected
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        Task<CallResult<int>> SubscribeToTickerUpdates(string symbol, Action<BitfinexMarketOverview[]> handler);

        /// <summary>
        /// Subscribes to trade updates for a symbol. Requires socket to be connected
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        Task<CallResult<int>> SubscribeToTradeUpdates(string symbol, Action<BitfinexTradeSimple[]> handler);

        /// <summary>
        /// Subscribes to orderbook update for a symbol. Requires socket to be connected
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="precision">The pricision of the udpates</param>
        /// <param name="frequency">The frequency of updates</param>
        /// <param name="length">The amount of data to receive in the initial snapshot</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        Task<CallResult<int>> SubscribeToBookUpdates(string symbol, Precision precision, Frequency frequency, int length, Action<BitfinexOrderBookEntry[]> handler);

        /// <summary>
        /// Subscribes to raw orderbook update for a symbol. Requires socket to be connected
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="length">The amount of data to recive in the initial snapshot</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        Task<CallResult<int>> SubscribeToRawBookUpdates(string symbol, int length, Action<BitfinexRawOrderBookEntry[]> handler);

        /// <summary>
        /// Subscribes to candle updates for a symbol. Requires socket to be connected
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="interval">The interval of the candles</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        Task<CallResult<int>> SubscribeToCandleUpdates(string symbol, TimeFrame interval, Action<BitfinexCandle[]> handler);

        /// <summary>
        /// Unsubscribe from a specific channel using the id acquired when subscribing
        /// </summary>
        /// <param name="streamId">The channel id to unsubscribe from</param>
        /// <returns></returns>
        Task<CallResult<bool>> UnsubscribeFromChannel(int streamId);

        void Dispose();
        void AddRateLimiter(IRateLimiter limiter);
        void RemoveRateLimiters();
        CallResult<long> Ping();
        Task<CallResult<long>> PingAsync();
    }
}