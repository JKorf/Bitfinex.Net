using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.SocketObjects;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;

namespace Bitfinex.Net.Interfaces
{
    /// <summary>
    /// Interface for the Bitfinex socket client
    /// </summary>
    public interface IBitfinexSocketClient: ISocketClient
    {
        /// <summary>
        /// Subscribes to ticker updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        CallResult<UpdateSubscription> SubscribeToTickerUpdates(string symbol, Action<BitfinexStreamSymbolOverview> handler);

        /// <summary>
        /// Subscribes to ticker updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<BitfinexStreamSymbolOverview> handler);

        /// <summary>
        /// Subscribes to order book updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="precision">The precision of the updates</param>
        /// <param name="frequency">The frequency of updates</param>
        /// <param name="length">The range for the order book updates</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        CallResult<UpdateSubscription> SubscribeToBookUpdates(string symbol, Precision precision, Frequency frequency, int length, Action<IEnumerable<BitfinexOrderBookEntry>> handler);

        /// <summary>
        /// Subscribes to order book updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="precision">The precision of the updates</param>
        /// <param name="frequency">The frequency of updates</param>
        /// <param name="length">The range for the order book updates, either 25 or 100</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToBookUpdatesAsync(string symbol, Precision precision, Frequency frequency, int length, Action<IEnumerable<BitfinexOrderBookEntry>> handler);

        /// <summary>
        /// Subscribes to raw order book updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="limit">The range for the order book updates</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        CallResult<UpdateSubscription> SubscribeToRawBookUpdates(string symbol, int limit, Action<IEnumerable<BitfinexRawOrderBookEntry>> handler);

        /// <summary>
        /// Subscribes to raw order book updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="limit">The range for the order book updates</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToRawBookUpdatesAsync(string symbol, int limit, Action<IEnumerable<BitfinexRawOrderBookEntry>> handler);

        /// <summary>
        /// Subscribes to public trade updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        CallResult<UpdateSubscription> SubscribeToTradeUpdates(string symbol, Action<IEnumerable<BitfinexTradeSimple>> handler);

        /// <summary>
        /// Subscribes to public trade updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<IEnumerable<BitfinexTradeSimple>> handler);

        /// <summary>
        /// Subscribes to kline updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="interval">The interval of the klines</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        CallResult<UpdateSubscription> SubscribeToKlineUpdates(string symbol, TimeFrame interval, Action<IEnumerable<BitfinexKline>> handler);

        /// <summary>
        /// Subscribes to kline updates for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="interval">The interval of the klines</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, TimeFrame interval, Action<IEnumerable<BitfinexKline>> handler);

        /// <summary>
        /// Subscribe to trading information updates
        /// </summary>
        /// <param name="orderHandler">Data handler for order updates. Can be null if not interested</param>
        /// <param name="tradeHandler">Data handler for trade execution updates. Can be null if not interested</param>
        /// <param name="positionHandler">Data handler for position updates. Can be null if not interested</param>
        /// <returns></returns>
        CallResult<UpdateSubscription> SubscribeToTradingUpdates(
            Action<BitfinexSocketEvent<IEnumerable<BitfinexOrder>>> orderHandler,
            Action<BitfinexSocketEvent<IEnumerable<BitfinexTradeDetails>>> tradeHandler,
            Action<BitfinexSocketEvent<IEnumerable<BitfinexPosition>>> positionHandler);

        /// <summary>
        /// Subscribe to trading information updates
        /// </summary>
        /// <param name="orderHandler">Data handler for order updates. Can be null if not interested</param>
        /// <param name="tradeHandler">Data handler for trade execution updates. Can be null if not interested</param>
        /// <param name="positionHandler">Data handler for position updates. Can be null if not interested</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTradingUpdatesAsync(
            Action<BitfinexSocketEvent<IEnumerable<BitfinexOrder>>> orderHandler,
            Action<BitfinexSocketEvent<IEnumerable<BitfinexTradeDetails>>> tradeHandler,
            Action<BitfinexSocketEvent<IEnumerable<BitfinexPosition>>> positionHandler);

        /// <summary>
        /// Subscribe to wallet information updates
        /// </summary>
        /// <param name="walletHandler">Data handler for wallet updates</param>
        /// <returns></returns>
        CallResult<UpdateSubscription> SubscribeToWalletUpdates(Action<BitfinexSocketEvent<IEnumerable<BitfinexWallet>>> walletHandler);

        /// <summary>
        /// Subscribe to wallet information updates
        /// </summary>
        /// <param name="walletHandler">Data handler for wallet updates</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToWalletUpdatesAsync(Action<BitfinexSocketEvent<IEnumerable<BitfinexWallet>>> walletHandler);

        /// <summary>
        /// Subscribe to funding information updates
        /// </summary>
        /// <param name="fundingOfferHandler">Subscribe to funding offer updates. Can be null if not interested</param>
        /// <param name="fundingCreditHandler">Subscribe to funding credit updates. Can be null if not interested</param>
        /// <param name="fundingLoanHandler">Subscribe to funding loan updates. Can be null if not interested</param>
        /// <returns></returns>
        CallResult<UpdateSubscription> SubscribeToFundingUpdates(
            Action<BitfinexSocketEvent<IEnumerable<BitfinexFundingOffer>>> fundingOfferHandler,
            Action<BitfinexSocketEvent<IEnumerable<BitfinexFundingCredit>>> fundingCreditHandler,
            Action<BitfinexSocketEvent<IEnumerable<BitfinexFunding>>> fundingLoanHandler);

        /// <summary>
        /// Subscribe to funding information updates
        /// </summary>
        /// <param name="fundingOfferHandler">Subscribe to funding offer updates. Can be null if not interested</param>
        /// <param name="fundingCreditHandler">Subscribe to funding credit updates. Can be null if not interested</param>
        /// <param name="fundingLoanHandler">Subscribe to funding loan updates. Can be null if not interested</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToFundingUpdatesAsync(
            Action<BitfinexSocketEvent<IEnumerable<BitfinexFundingOffer>>> fundingOfferHandler,
            Action<BitfinexSocketEvent<IEnumerable<BitfinexFundingCredit>>> fundingCreditHandler,
            Action<BitfinexSocketEvent<IEnumerable<BitfinexFunding>>> fundingLoanHandler);

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
        /// <param name="priceOcoStop">Oco stop price of the order</param>
        /// <param name="flags">Additional flags</param>
        /// <returns></returns>
        Task<CallResult<BitfinexOrder>> PlaceOrderAsync(OrderType type, string symbol, decimal amount, long? groupId = null, long? clientOrderId = null, decimal? price = null, decimal? priceTrailing = null, decimal? priceAuxiliaryLimit = null, decimal? priceOcoStop = null, OrderFlags? flags = null);

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
        CallResult<BitfinexOrder> UpdateOrder(long orderId, decimal? price = null, decimal? amount = null, decimal? delta = null, decimal? priceAuxiliaryLimit = null, decimal? priceTrailing = null, OrderFlags? flags = null);

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
        Task<CallResult<BitfinexOrder>> UpdateOrderAsync(long orderId, decimal? price = null, decimal? amount = null, decimal? delta = null, decimal? priceAuxiliaryLimit = null, decimal? priceTrailing = null, OrderFlags? flags = null);

        /// <summary>
        /// Cancels an order
        /// </summary>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <returns></returns>
        CallResult<BitfinexOrder> CancelOrder(long orderId);

        /// <summary>
        /// Cancels an order
        /// </summary>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <returns></returns>
        Task<CallResult<BitfinexOrder>> CancelOrderAsync(long orderId);

        /// <summary>
        /// Cancels multiple orders based on their groupId
        /// </summary>
        /// <param name="groupOrderId">The group id to cancel</param>
        /// <returns>True if successfully committed on server</returns>
        CallResult<bool> CancelOrdersByGroupId(long groupOrderId);

        /// <summary>
        /// Cancels multiple orders based on their groupId
        /// </summary>
        /// <param name="groupOrderId">The group id to cancel</param>
        /// <returns>True if successfully committed on server</returns>
        Task<CallResult<bool>> CancelOrdersByGroupIdAsync(long groupOrderId);

        /// <summary>
        /// Cancels multiple orders based on their groupIds
        /// </summary>
        /// <param name="groupOrderIds">The group ids to cancel</param>
        /// <returns>True if successfully committed on server</returns>
        CallResult<bool> CancelOrdersByGroupIds(IEnumerable<long> groupOrderIds);

        /// <summary>
        /// Cancels multiple orders based on their groupIds
        /// </summary>
        /// <param name="groupOrderIds">The group ids to cancel</param>
        /// <returns>True if successfully committed on server</returns>
        Task<CallResult<bool>> CancelOrdersByGroupIdsAsync(IEnumerable<long> groupOrderIds);

        /// <summary>
        /// Cancels multiple orders based on their order ids
        /// </summary>
        /// <param name="orderIds">The order ids to cancel</param>
        /// <returns>True if successfully committed on server</returns>
        CallResult<bool> CancelOrders(IEnumerable<long> orderIds);

        /// <summary>
        /// Cancels multiple orders based on their order ids
        /// </summary>
        /// <param name="orderIds">The order ids to cancel</param>
        /// <returns>True if successfully committed on server</returns>
        Task<CallResult<bool>> CancelOrdersAsync(IEnumerable<long> orderIds);

        /// <summary>
        /// Cancels multiple orders based on their clientOrderIds
        /// </summary>
        /// <param name="clientOrderIds">The client order ids to cancel, listed as (clientOrderId, Day) pair. ClientOrderIds are unique per day, so timestamp should be provided</param>
        /// <returns>True if successfully committed on server</returns>
        CallResult<bool> CancelOrdersByClientOrderIds(Dictionary<long, DateTime> clientOrderIds);

        /// <summary>
        /// Cancels multiple orders based on their clientOrderIds
        /// </summary>
        /// <param name="clientOrderIds">The client order ids to cancel, listed as (clientOrderId, Day) pair. ClientOrderIds are unique per day, so timestamp should be provided</param>
        /// <returns>True if successfully committed on server</returns>
        Task<CallResult<bool>> CancelOrdersByClientOrderIdsAsync(Dictionary<long, DateTime> clientOrderIds);
    }
}