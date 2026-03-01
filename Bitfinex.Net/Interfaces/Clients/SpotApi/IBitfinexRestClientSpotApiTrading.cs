using Bitfinex.Net.Enums;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Objects.Models;

namespace Bitfinex.Net.Interfaces.Clients.SpotApi
{
    /// <summary>
    /// Bitfinex trading endpoints, placing and managing orders.
    /// </summary>
    public interface IBitfinexRestClientSpotApiTrading
    {
        /// <summary>
        /// Get the active orders
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-auth-orders" /><br />
        /// Endpoint:<br />
        /// POST /v2/auth/r/orders
        /// </para>
        /// </summary>
        /// <param name="symbol">Filter by symbol, for example `tETHUSD`</param>
        /// <param name="orderIds">Filter by specific order ids</param>
        /// <param name="clientOrderId">Filter by client order id. clientOrderId should also be provided</param>
        /// <param name="clientOrderIdDate">The date of the clientOrderId</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexOrder[]>> GetOpenOrdersAsync(
            string? symbol = null,
            IEnumerable<long>? orderIds = null,
            string? clientOrderId = null,
            DateTime? clientOrderIdDate = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get the order history for a symbol for this account
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-auth-orders-history" /><br />
        /// Endpoint:<br />
        /// POST /v2/auth/r/orders/{symbol}/hist
        /// </para>
        /// </summary>
        /// <param name="symbol">The symbol to get the history for, for example `tETHUSD`</param>
        /// <param name="orderIds">Filter by specific order ids</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexOrder[]>> GetClosedOrdersAsync(string? symbol = null, IEnumerable<long>? orderIds = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get the individual trades for an order
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-auth-order-trades" /><br />
        /// Endpoint:<br />
        /// POST /v2/auth/r/order/{symbol}:{orderId}/trades
        /// </para>
        /// </summary>
        /// <param name="symbol">The symbol of the order, for example `tETHUSD`</param>
        /// <param name="orderId">The order Id</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexTradeDetails[]>> GetOrderTradesAsync(string symbol, long orderId, CancellationToken ct = default);

        /// <summary>
        /// Get the trade history for a symbol
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-auth-trades" /><br />
        /// Endpoint:<br />
        /// POST /v2/auth/r/trades/{symbol}/hist
        /// </para>
        /// </summary>
        /// <param name="symbol">The symbol to get history for, for example `tETHUSD`</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexTradeDetails[]>> GetUserTradesAsync(string? symbol = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);


        /// <summary>
        /// Get a list of historical positions
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-auth-positions-hist" /><br />
        /// Endpoint:<br />
        /// POST /v2/auth/r/positions/hist
        /// </para>
        /// </summary>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexPosition[]>> GetPositionHistoryAsync(DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Place a new order
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-auth-submit-order" /><br />
        /// Endpoint:<br />
        /// POST /v2/auth/w/order/submit
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol to place order for, for example `tETHUSD`</param>
        /// <param name="side">Side of the order</param>
        /// <param name="type">Type of the order</param>
        /// <param name="price">The price for the order</param>
        /// <param name="quantity">The quantity of the order</param>
        /// <param name="ct">Cancellation token</param>
        /// <param name="flags"></param>
        /// <param name="leverage">Set the leverage for a derivative order, supported by derivative symbol orders only. The value should be between 1 and 100 inclusive. The field is optional, if omitted the default leverage value of 10 will be used.</param>
        /// <param name="groupId">Group id</param>
        /// <param name="clientOrderId">Client order id</param>
        /// <param name="priceTrailing">The trailing price for a trailing stop order</param>
        /// <param name="priceAuxLimit">Auxiliary Limit price (for STOP LIMIT)</param>
        /// <param name="priceOcoStop">OCO stop price</param>
        /// <param name="cancelTime">datetime for automatic order cancelation</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWriteResultOrder>> PlaceOrderAsync(
            string symbol,
            OrderSide side,
            OrderType type,
            decimal quantity,
            decimal price,
            OrderFlags? flags = null,
            int? leverage = null,
            int? groupId = null,
            int? clientOrderId = null,
            decimal? priceTrailing = null,
            decimal? priceAuxLimit = null,
            decimal? priceOcoStop = null,
            DateTime? cancelTime = null,
            CancellationToken ct = default);

        /// <summary>
        /// Cancel a specific order
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-auth-cancel-order" /><br />
        /// Endpoint:<br />
        /// POST /v2/auth/w/order/cancel
        /// </para>
        /// </summary>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <param name="clientOrderId">The client order id of the order to cancel</param>
        /// <param name="clientOrderIdDate">The date of the client order (year month and day)</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWriteResultOrder>> CancelOrderAsync(long? orderId = null, long? clientOrderId = null, DateTime? clientOrderIdDate = null, CancellationToken ct = default);

        /// <summary>
        /// Cancels open orders
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-auth-cancel-orders-multiple" /><br />
        /// Endpoint:<br />
        /// POST /v2/auth/w/order/cancel/multi
        /// </para>
        /// </summary>
        /// <param name="orderIds">Ids of orders to cancel</param>
        /// <param name="groupIds">Group ids to cancel</param>
        /// <param name="clientOrderIds">Client order ids to cancel</param>
        /// <param name="all">Cancel all orders</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWriteResultOrders>> CancelOrdersAsync(IEnumerable<long>? orderIds = null, IEnumerable<long>? groupIds = null, Dictionary<long, DateTime>? clientOrderIds = null, bool? all = null, CancellationToken ct = default);

        /// <summary>
        /// The claim feature allows the use of funds you have in your Margin Wallet to settle a leveraged position as an exchange buy or sale
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-auth-position-claim" /><br />
        /// Endpoint:<br />
        /// POST /v2/auth/w/position/claim
        /// </para>
        /// </summary>
        /// <param name="id">The id of the position to claim</param>
        /// <param name="quantity">The (partial) quantity to be claimed</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWriteResultPosition>> ClaimPositionAsync(long id, decimal quantity, CancellationToken ct = default);

        /// <summary>
        /// Essentially a reverse of the Claim Position feature, the Increase Position feature allows you to create a new position using the funds in your margin wallet
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-auth-position-increase" /><br />
        /// Endpoint:<br />
        /// POST /v2/auth/w/position/increase
        /// </para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `tETHUSD`</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWriteResultPositionBasic>> IncreasePositionAsync(string symbol, decimal quantity, CancellationToken ct = default);

        /// <summary>
        /// Returns information relevant to the increase position endpoint.
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-auth-increase-position-info" /><br />
        /// Endpoint:<br />
        /// POST /v2/auth/r/position/increase/info
        /// </para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `tETHUSD`</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexIncreasePositionInfo>> GetIncreasePositionInfoAsync(string symbol, decimal quantity, CancellationToken ct = default);

        /// <summary>
        /// Get the active positions
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-auth-positions" /><br />
        /// Endpoint:<br />
        /// POST /v2/auth/r/positions
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexPosition[]>> GetPositionsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get positions by id
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-auth-positions-audit" /><br />
        /// Endpoint:<br />
        /// POST /v2/auth/r/positions/audit
        /// </para>
        /// </summary>
        /// <param name="ids">The id's of positions to return</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexPosition[]>> GetPositionsByIdAsync(IEnumerable<string> ids, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Returns position snapshots of user positions between the specified start and end perimiters. Snapshots are taken daily.
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-auth-positions-snap" /><br />
        /// Endpoint:<br />
        /// POST /v2/auth/r/positions/snap
        /// </para>
        /// </summary>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexPosition[]>> GetPositionSnapshotsAsync(DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);
    }
}
