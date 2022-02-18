using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.Socket;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;

namespace Bitfinex.Net.Interfaces.Clients.SpotApi
{
    /// <summary>
    /// Bitfinex spot streams
    /// </summary>
    public interface IBitfinexSocketClientSpotStreams : IDisposable
    {
        /// <summary>
        /// Subscribes to ticker updates for a symbol
        /// <para><a href="https://docs.bitfinex.com/reference#ws-public-ticker" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="handler">The handler for the data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<DataEvent<BitfinexStreamSymbolOverview>> handler, CancellationToken ct = default);

        /// <summary>
        /// Subscribes to order book updates for a symbol
        /// <para><a href="https://docs.bitfinex.com/reference#ws-public-books" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="precision">The precision of the updates</param>
        /// <param name="frequency">The frequency of updates</param>
        /// <param name="length">The range for the order book updates, either 25 or 100</param>
        /// <param name="handler">The handler for the data</param>
        /// <param name="checksumHandler">The handler for the checksum, can be used to validate a order book implementation</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, Precision precision, Frequency frequency, int length, Action<DataEvent<IEnumerable<BitfinexOrderBookEntry>>> handler, Action<DataEvent<int>>? checksumHandler = null, CancellationToken ct = default);

        /// <summary>
        /// Subscribes to raw order book updates for a symbol
        /// <para><a href="https://docs.bitfinex.com/reference#ws-public-raw-books" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="limit">The range for the order book updates</param>
        /// <param name="handler">The handler for the data</param>
        /// <param name="checksumHandler">The handler for the checksum, can be used to validate a order book implementation</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToRawOrderBookUpdatesAsync(string symbol, int limit, Action<DataEvent<IEnumerable<BitfinexRawOrderBookEntry>>> handler, Action<DataEvent<int>>? checksumHandler = null, CancellationToken ct = default);

        /// <summary>
        /// Subscribes to public trade updates for a symbol
        /// <para><a href="https://docs.bitfinex.com/reference#ws-public-trades" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="handler">The handler for the data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<DataEvent<IEnumerable<BitfinexTradeSimple>>> handler, CancellationToken ct = default);

        /// <summary>
        /// Subscribes to kline updates for a symbol
        /// <para><a href="https://docs.bitfinex.com/reference#ws-public-candles" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="interval">The interval of the klines</param>
        /// <param name="handler">The handler for the data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, KlineInterval interval, Action<DataEvent<IEnumerable<BitfinexKline>>> handler, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to trading information updates
        /// <para><a href="https://docs.bitfinex.com/reference#ws-auth-trades" /></para>
        /// </summary>
        /// <param name="orderHandler">Data handler for order updates. Can be null if not interested</param>
        /// <param name="tradeHandler">Data handler for trade execution updates. Can be null if not interested</param>
        /// <param name="positionHandler">Data handler for position updates. Can be null if not interested</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToUserTradeUpdatesAsync(
            Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexOrder>>>> orderHandler,
            Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexTradeDetails>>>> tradeHandler,
            Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexPosition>>>> positionHandler,
            CancellationToken ct = default);

        /// <summary>
        /// Subscribe to wallet information updates
        /// <para><a href="https://docs.bitfinex.com/reference#ws-auth-wallets" /></para>
        /// </summary>
        /// <param name="walletHandler">Data handler for wallet updates</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToBalanceUpdatesAsync(Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexWallet>>>> walletHandler, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to funding information updates
        /// <para><a href="https://docs.bitfinex.com/reference#ws-auth-funding-offers" /></para>
        /// <para><a href="https://docs.bitfinex.com/reference#ws-auth-funding-credits" /></para>
        /// <para><a href="https://docs.bitfinex.com/reference#ws-auth-funding-loans" /></para>
        /// </summary>
        /// <param name="fundingOfferHandler">Subscribe to funding offer updates. Can be null if not interested</param>
        /// <param name="fundingCreditHandler">Subscribe to funding credit updates. Can be null if not interested</param>
        /// <param name="fundingLoanHandler">Subscribe to funding loan updates. Can be null if not interested</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToFundingUpdatesAsync(
            Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexFundingOffer>>>> fundingOfferHandler,
            Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexFundingCredit>>>> fundingCreditHandler,
            Action<DataEvent<BitfinexSocketEvent<IEnumerable<BitfinexFunding>>>> fundingLoanHandler,
            CancellationToken ct = default);

        /// <summary>
        /// Places a new order
        /// <para><a href="https://docs.bitfinex.com/reference#ws-auth-input-order-new" /></para>
        /// </summary>
        /// <param name="type">The type of the order</param>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="quantity">The quantity of the order, positive for buying, negative for selling</param>
        /// <param name="groupId">Group id to assign to the order</param>
        /// <param name="clientOrderId">Client order id to assign to the order</param>
        /// <param name="price">Price of the order</param>
        /// <param name="priceTrailing">Trailing price of the order</param>
        /// <param name="priceAuxiliaryLimit">Auxiliary limit price of the order</param>
        /// <param name="priceOcoStop">Oco stop price of the order</param>
        /// <param name="flags">Additional flags</param>
        /// <param name="affiliateCode">Affiliate code for the order</param>
        /// <returns></returns>
        Task<CallResult<BitfinexOrder>> PlaceOrderAsync(OrderType type, string symbol, decimal quantity, long? groupId = null, long? clientOrderId = null, decimal? price = null, decimal? priceTrailing = null, decimal? priceAuxiliaryLimit = null, decimal? priceOcoStop = null, OrderFlags? flags = null, string? affiliateCode = null);

        /// <summary>
        /// Updates an order
        /// <para><a href="https://docs.bitfinex.com/reference#ws-auth-input-order-update" /></para>
        /// </summary>
        /// <param name="orderId">The id of the order to update</param>
        /// <param name="price">The new price of the order</param>
        /// <param name="quantity">The new quantity of the order</param>
        /// <param name="delta">The delta to change</param>
        /// <param name="priceAuxiliaryLimit">the new aux limit price</param>
        /// <param name="priceTrailing">The new trailing price</param>
        /// <param name="flags">The new flags</param>
        /// <returns></returns>
        Task<CallResult<BitfinexOrder>> UpdateOrderAsync(long orderId, decimal? price = null, decimal? quantity = null, decimal? delta = null, decimal? priceAuxiliaryLimit = null, decimal? priceTrailing = null, OrderFlags? flags = null);

        /// <summary>
        /// Cancels an order
        /// <para><a href="https://docs.bitfinex.com/reference#ws-auth-input-order-cancel" /></para>
        /// </summary>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <returns></returns>
        Task<CallResult<BitfinexOrder>> CancelOrderAsync(long orderId);

        /// <summary>
        /// Cancels multiple orders based on their groupId
        /// <para><a href="https://docs.bitfinex.com/reference#ws-auth-input-order-cancel" /></para>
        /// </summary>
        /// <param name="groupOrderId">The group id to cancel</param>
        /// <returns>True if successfully committed on server</returns>
        Task<CallResult<bool>> CancelOrdersByGroupIdAsync(long groupOrderId);

        /// <summary>
        /// Cancels multiple orders based on their groupIds
        /// <para><a href="https://docs.bitfinex.com/reference#ws-auth-input-order-cancel-multi" /></para>
        /// </summary>
        /// <param name="groupOrderIds">The group ids to cancel</param>
        /// <returns>True if successfully committed on server</returns>
        Task<CallResult<bool>> CancelOrdersByGroupIdsAsync(IEnumerable<long> groupOrderIds);

        /// <summary>
        /// Cancels multiple orders based on their order ids
        /// <para><a href="https://docs.bitfinex.com/reference#ws-auth-input-order-cancel-multi" /></para>
        /// </summary>
        /// <param name="orderIds">The order ids to cancel</param>
        /// <returns>True if successfully committed on server</returns>
        Task<CallResult<bool>> CancelOrdersAsync(IEnumerable<long> orderIds);

        /// <summary>
        /// Cancels multiple orders based on their clientOrderIds
        /// <para><a href="https://docs.bitfinex.com/reference#ws-auth-input-order-cancel-multi" /></para>
        /// </summary>
        /// <param name="clientOrderIds">The client order ids to cancel, listed as (clientOrderId, Day) pair. ClientOrderIds are unique per day, so timestamp should be provided</param>
        /// <returns>True if successfully committed on server</returns>
        Task<CallResult<bool>> CancelOrdersByClientOrderIdsAsync(Dictionary<long, DateTime> clientOrderIds);
    }
}