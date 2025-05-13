using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.Socket;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;

namespace Bitfinex.Net.Interfaces.Clients.SpotApi
{
    /// <summary>
    /// Bitfinex spot streams
    /// </summary>
    public interface IBitfinexSocketClientSpotApi : ISocketApiClient, IDisposable
    {
        /// <summary>
        /// Get the shared socket subscription client. This interface is shared with other exchanges to allow for a common implementation for different exchanges.
        /// </summary>
        public IBitfinexSocketClientSpotApiShared SharedClient { get; }

        /// <summary>
        /// Subscribes to ticker updates for a symbol. Use SubscribeToFundingTickerUpdatesAsync for funding symbol ticker updates
        /// <para><a href="https://docs.bitfinex.com/reference#ws-public-ticker" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to, for example `tETHUSD`</param>
        /// <param name="handler">The handler for the data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<DataEvent<BitfinexStreamTicker>> handler, CancellationToken ct = default);

        /// <summary>
        /// Subscribes to funding ticker updates a symbol. Use SubscribeToTickerUpdatesAsync for trade symbol ticker updates
        /// <para><a href="https://docs.bitfinex.com/reference#ws-public-ticker" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to, for example `tETHUSD`</param>
        /// <param name="handler">The handler for the data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToFundingTickerUpdatesAsync(string symbol, Action<DataEvent<BitfinexStreamFundingTicker>> handler, CancellationToken ct = default);

        /// <summary>
        /// Subscribes to order book updates for a symbol. Use SubscribeToFundingOrderBookUpdatesAsync for funding symbol ticker updates
        /// <para><a href="https://docs.bitfinex.com/reference#ws-public-books" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to, for example `tETHUSD`</param>
        /// <param name="precision">The precision of the updates</param>
        /// <param name="frequency">The frequency of updates</param>
        /// <param name="length">The range for the order book updates, either 25 or 100</param>
        /// <param name="handler">The handler for the data</param>
        /// <param name="checksumHandler">The handler for the checksum, can be used to validate a order book implementation</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, Precision precision, Frequency frequency, int length, Action<DataEvent<BitfinexOrderBookEntry[]>> handler, Action<DataEvent<int>>? checksumHandler = null, CancellationToken ct = default);

        /// <summary>
        /// Subscribes to funding order book updates for a symbol. Use SubscribeToOrderBookUpdatesAsync for trade symbol ticker updates
        /// <para><a href="https://docs.bitfinex.com/reference#ws-public-books" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to, for example `tETHUSD`</param>
        /// <param name="precision">The precision of the updates</param>
        /// <param name="frequency">The frequency of updates</param>
        /// <param name="length">The range for the order book updates, either 25 or 100</param>
        /// <param name="handler">The handler for the data</param>
        /// <param name="checksumHandler">The handler for the checksum, can be used to validate a order book implementation</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToFundingOrderBookUpdatesAsync(string symbol, Precision precision, Frequency frequency, int length, Action<DataEvent<BitfinexOrderBookFundingEntry[]>> handler, Action<DataEvent<int>>? checksumHandler = null, CancellationToken ct = default);

        /// <summary>
        /// Subscribes to raw order book updates for a symbol. Use SubscribeToRawFundingOrderBookUpdatesAsync for funding symbol ticker updates
        /// <para><a href="https://docs.bitfinex.com/reference#ws-public-raw-books" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to, for example `tETHUSD`</param>
        /// <param name="limit">The range for the order book updates</param>
        /// <param name="handler">The handler for the data</param>
        /// <param name="checksumHandler">The handler for the checksum, can be used to validate a order book implementation</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToRawOrderBookUpdatesAsync(string symbol, int limit, Action<DataEvent<BitfinexRawOrderBookEntry[]>> handler, Action<DataEvent<int>>? checksumHandler = null, CancellationToken ct = default);

        /// <summary>
        /// Subscribes to raw order book updates for a symbol. Use SubscribeToRawOrderBookUpdatesAsync for trade symbol ticker updates
        /// <para><a href="https://docs.bitfinex.com/reference#ws-public-raw-books" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to, for example `tETHUSD`</param>
        /// <param name="limit">The range for the order book updates</param>
        /// <param name="handler">The handler for the data</param>
        /// <param name="checksumHandler">The handler for the checksum, can be used to validate a order book implementation</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToRawFundingOrderBookUpdatesAsync(string symbol, int limit, Action<DataEvent<BitfinexRawOrderBookFundingEntry[]>> handler, Action<DataEvent<int>>? checksumHandler = null, CancellationToken ct = default);

        /// <summary>
        /// Subscribes to public trade updates for a symbol
        /// <para><a href="https://docs.bitfinex.com/reference#ws-public-trades" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to, for example `tETHUSD`</param>
        /// <param name="handler">The handler for the data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<DataEvent<BitfinexTradeSimple[]>> handler, CancellationToken ct = default);

        /// <summary>
        /// Subscribes to kline updates for a symbol
        /// <para><a href="https://docs.bitfinex.com/reference#ws-public-candles" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to, for example `tETHUSD`. For funding klines use {symbol}:p{period}, for example fUSD:p30</param>
        /// <param name="interval">The interval of the klines</param>
        /// <param name="handler">The handler for the data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, KlineInterval interval, Action<DataEvent<BitfinexKline[]>> handler, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to liquidation updates
        /// <para><a href="https://docs.bitfinex.com/reference/ws-public-status" /></para>
        /// </summary>
        /// <param name="handler">The handler for the data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToLiquidationUpdatesAsync(Action<DataEvent<BitfinexLiquidation[]>> handler, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to derivatives status updates
        /// <para><a href="https://docs.bitfinex.com/reference/ws-public-status" /></para>
        /// </summary>
        /// <param name="symbol">The derivatives symbol, tBTCF0:USTF0 for example</param>
        /// <param name="handler">The handler for the data</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToDerivativesUpdatesAsync(string symbol, Action<DataEvent<BitfinexDerivativesStatusUpdate>> handler, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to trading information updates
        /// <para><a href="https://docs.bitfinex.com/reference#ws-auth-trades" /></para>
        /// </summary>
        /// <param name="orderHandler">Data handler for order updates. Can be null if not interested</param>
        /// <param name="tradeHandler">Data handler for trade execution updates. Can be null if not interested</param>
        /// <param name="positionHandler">Data handler for position updates. Can be null if not interested</param>
        /// <param name="fundingOfferHandler">Data handler for funding offer updates. Can be null if not interested</param>
        /// <param name="fundingCreditHandler">Data handler for funding credit updates. Can be null if not interested</param>
        /// <param name="fundingLoanHandler">Data handler for funding loan updates. Can be null if not interested</param>
        /// <param name="walletHandler">Data handler for wallet updates. Can be null if not interested</param>
        /// <param name="balanceHandler">Data handler for balance updates. Can be null if not interested</param>
        /// <param name="fundingTradeHandler">Data handler for funding trade updates. Can be null if not interested</param>
        /// <param name="fundingInfoHandler">Data handler for funding info updates. Can be null if not interested</param>
        /// <param name="marginBaseHandler">Data handler for margin base updates. Can be null if not interested</param>
        /// <param name="marginSymbolHandler">Data handler for margin symbol updates. Can be null if not interested</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToUserUpdatesAsync(
             Action<DataEvent<BitfinexOrder[]>>? orderHandler = null,
             Action<DataEvent<BitfinexPosition[]>>? positionHandler = null,
             Action<DataEvent<BitfinexFundingOffer[]>>? fundingOfferHandler = null,
             Action<DataEvent<BitfinexFundingCredit[]>>? fundingCreditHandler = null,
             Action<DataEvent<BitfinexFunding[]>>? fundingLoanHandler = null,
             Action<DataEvent<BitfinexWallet[]>>? walletHandler = null,
             Action<DataEvent<BitfinexBalance>>? balanceHandler = null,
             Action<DataEvent<BitfinexTradeDetails>>? tradeHandler = null,
             Action<DataEvent<BitfinexFundingTrade>>? fundingTradeHandler = null,
             Action<DataEvent<BitfinexFundingInfo>>? fundingInfoHandler = null,
             Action<DataEvent<BitfinexMarginBase>>? marginBaseHandler = null,
             Action<DataEvent<BitfinexMarginSymbol>>? marginSymbolHandler = null,
             CancellationToken ct = default);

        /// <summary>
        /// Places a new order
        /// <para><a href="https://docs.bitfinex.com/reference#ws-auth-input-order-new" /></para>
        /// </summary>
        /// <param name="side">The order side</param>
        /// <param name="type">The type of the order</param>
        /// <param name="symbol">The symbol the order is for, for example `tETHUSD`</param>
        /// <param name="quantity">The quantity of the order</param>
        /// <param name="groupId">Group id to assign to the order</param>
        /// <param name="clientOrderId">Client order id to assign to the order</param>
        /// <param name="price">Price of the order</param>
        /// <param name="priceTrailing">Trailing price of the order</param>
        /// <param name="priceAuxiliaryLimit">Auxiliary limit price of the order</param>
        /// <param name="priceOcoStop">Oco stop price of the order</param>
        /// <param name="flags">Additional flags</param>
        /// <param name="leverage">Leverage</param>
        /// <param name="cancelTime">Automatically cancel the order after this time</param>
        /// <param name="affiliateCode">Affiliate code for the order</param>
        /// <returns></returns>
        Task<CallResult<BitfinexOrder>> PlaceOrderAsync(OrderSide side, OrderType type, string symbol, decimal quantity, long? groupId = null, long? clientOrderId = null, decimal? price = null, decimal? priceTrailing = null, decimal? priceAuxiliaryLimit = null, decimal? priceOcoStop = null, OrderFlags? flags = null, int? leverage = null, DateTime? cancelTime = null, string? affiliateCode = null);

        /// <summary>
        /// Cancel all orders
        /// <para><a href="https://docs.bitfinex.com/reference/ws-auth-input-order-cancel-multi" /></para>
        /// </summary>
        /// <returns></returns>
        Task<CallResult<BitfinexOrder[]>> CancelAllOrdersAsync();

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
        Task<CallResult<BitfinexOrder[]>> CancelOrdersByGroupIdAsync(long groupOrderId);

        /// <summary>
        /// Cancels multiple orders based on their groupIds
        /// <para><a href="https://docs.bitfinex.com/reference#ws-auth-input-order-cancel-multi" /></para>
        /// </summary>
        /// <param name="groupOrderIds">The group ids to cancel</param>
        /// <returns>True if successfully committed on server</returns>
        Task<CallResult<BitfinexOrder[]>> CancelOrdersByGroupIdsAsync(IEnumerable<long> groupOrderIds);

        /// <summary>
        /// Cancels multiple orders based on their order ids
        /// <para><a href="https://docs.bitfinex.com/reference#ws-auth-input-order-cancel-multi" /></para>
        /// </summary>
        /// <param name="orderIds">The order ids to cancel</param>
        /// <returns>True if successfully committed on server</returns>
        Task<CallResult<BitfinexOrder[]>> CancelOrdersAsync(IEnumerable<long> orderIds);

        /// <summary>
        /// Cancels multiple orders based on their clientOrderIds
        /// <para><a href="https://docs.bitfinex.com/reference#ws-auth-input-order-cancel-multi" /></para>
        /// </summary>
        /// <param name="clientOrderIds">The client order ids to cancel, listed as (clientOrderId, Day) pair. ClientOrderIds are unique per day, so timestamp should be provided</param>
        /// <returns>True if successfully committed on server</returns>
        Task<CallResult<BitfinexOrder[]>> CancelOrdersByClientOrderIdsAsync(Dictionary<long, DateTime> clientOrderIds);

        /// <summary>
        /// Submit a new funding offer
        /// <para><a href="https://docs.bitfinex.com/reference/ws-auth-input-offer-new" /></para>
        /// </summary>
        /// <param name="type">Offer type</param>
        /// <param name="symbol">Symbol, for example `tETHUSD`</param>
        /// <param name="quantity">Amount (more than 0 for offer, less than 0 for bid)</param>
        /// <param name="price">Rate (or offset for FRRDELTA offers)</param>
        /// <param name="period">Time period of offer. Minimum 2 days. Maximum 120 days.</param>
        /// <param name="flags">Flags</param>
        /// <returns></returns>
        Task<CallResult<BitfinexFundingOffer>> SubmitFundingOfferAsync(FundingOfferType type, string symbol, decimal quantity, decimal price, int period, int? flags = null);

        /// <summary>
        /// Cancel a funding offer
        /// <para><a href="https://docs.bitfinex.com/reference/ws-auth-input-offer-cancel" /></para>
        /// </summary>
        /// <param name="id">Id of the offer to cancel</param>
        /// <returns></returns>
        Task<CallResult<BitfinexFundingOffer>> CancelFundingOfferAsync(long id);
    }
}
