using Bitfinex.Net.Enums;
using Bitfinex.Net.Interfaces.Clients.ExchangeApi;
using Bitfinex.Net.Objects.Models;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients.ExchangeApi
{
    internal partial class BitfinexRestClientExchangeSharedApi
    {
        #region Place Spot Order

        public SharedFeeDeductionType SpotFeeDeductionType => SharedFeeDeductionType.DeductFromOutput;
        public SharedFeeAssetType SpotFeeAssetType => SharedFeeAssetType.OutputAsset;
        public SharedOrderType[] SpotSupportedOrderTypes { get; } = new[] { SharedOrderType.Limit, SharedOrderType.Market, SharedOrderType.LimitMaker };
        public SharedTimeInForce[] SpotSupportedTimeInForce { get; } = new[] { SharedTimeInForce.GoodTillCanceled, SharedTimeInForce.ImmediateOrCancel, SharedTimeInForce.FillOrKill };
        public SharedQuantitySupport SpotSupportedOrderQuantity { get; } = new SharedQuantitySupport(
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset);

        public string GenerateClientOrderId() => ExchangeHelpers.RandomLong(10).ToString();

        async Task<ICallResult<SharedId>> IPlaceSpotOrder.PlaceSpotOrderAsync(PlaceSpotOrderRequest request, CancellationToken ct)
            => await PlaceSpotOrderAsync(request, ct).ConfigureAwait(false);

        public PlaceSpotOrderOptions PlaceSpotOrderOptions { get; } = new PlaceSpotOrderOptions(_exchangeName);
        public async Task<HttpResult<SharedId>> PlaceSpotOrderAsync(PlaceSpotOrderRequest request, CancellationToken ct)
        {
            var validationError = PlaceSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            int clientOrderId = 0;
            if (request.ClientOrderId != null && !int.TryParse(request.ClientOrderId, out clientOrderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(PlaceSpotOrderRequest.ClientOrderId), "ClientOrderId needs to be parsable to `int` for `Bitfinex`"));

            var result = await _api.Trading.PlaceOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                request.Side == SharedOrderSide.Buy ? Enums.OrderSide.Buy : Enums.OrderSide.Sell,
                GetPlaceOrderType(request.OrderType, request.TimeInForce),
                quantity: request.Quantity?.QuantityInBaseAsset ?? 0,
                flags: request.OrderType == SharedOrderType.LimitMaker ? Enums.OrderFlags.PostOnly : null,
                price: request.Price ?? 0,
                clientOrderId: request.ClientOrderId != null ? clientOrderId : null,
                ct: ct).ConfigureAwait(false);

            if (!result.Success)
                return HttpResult.Fail<SharedId>(result);

            return HttpResult.Ok(result, new SharedId(result.Data.Data!.Id.ToString()));
        }

        #endregion

        #region Get Spot Order

        public GetSpotOrderOptions GetSpotOrderOptions { get; } = new GetSpotOrderOptions(_exchangeName, true);
        async Task<ICallResult<SharedSpotOrder>> IGetSpotOrder.GetSpotOrderAsync(GetOrderRequest request, CancellationToken ct)
            => await GetSpotOrderAsync(request, ct).ConfigureAwait(false);

        public async Task<HttpResult<SharedSpotOrder>> GetSpotOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = GetSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedSpotOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderRequest.OrderId), "Invalid order id"));

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await _api.Trading.GetOpenOrdersAsync(symbol, new[] { orderId }, ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotOrder>(result);

            if (!result.Data.Any())
                result = await _api.Trading.GetClosedOrdersAsync(symbol, new[] { orderId }, ct: ct).ConfigureAwait(false);
            
            if (!result.Success)
                return HttpResult.Fail<SharedSpotOrder>(result);

            if (!result.Data.Any())
                return HttpResult.Fail<SharedSpotOrder>(result, new ServerError(new ErrorInfo(ErrorType.UnknownOrder, $"Order with id {orderId} not found")));

            var order = result.Data.Single();
            return HttpResult.Ok(result, new SharedSpotOrder(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, order.Symbol),
                order.Symbol,
                order.Id.ToString(),
                ParseOrderType(order.Type, order.Flags),
                order.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                ParseOrderStatus(order.Status),
                order.CreateTime)
            {
                ClientOrderId = order.ClientOrderId?.ToString(),
                AveragePrice = order.PriceAverage == 0 ? null : order.PriceAverage,
                OrderQuantity = new SharedOrderQuantity(order.Quantity),
                QuantityFilled = new SharedOrderQuantity(order.Quantity - order.QuantityRemaining),
                TimeInForce = ParseTimeInForce(order.Type, order.Flags),
                UpdateTime = order.UpdateTime,
                IsTriggerOrder = order.Type == OrderType.ExchangeStop || order.Type == OrderType.ExchangeStopLimit,
                OrderPrice = order.Type == OrderType.ExchangeStop || order.Type == OrderType.ExchangeStopLimit ? order.PriceAuxilliaryLimit : order.Price,
                TriggerPrice = order.Type == OrderType.ExchangeStop || order.Type == OrderType.ExchangeStopLimit ? order.Price : null
            });
        }

        #endregion

        #region Get Open Spot Orders

        public GetOpenSpotOrdersOptions GetOpenSpotOrdersOptions { get; } = new GetOpenSpotOrdersOptions(_exchangeName, true);
        async Task<ICallResult<SharedSpotOrder[]>> IGetOpenSpotOrders.GetOpenSpotOrdersAsync(GetOpenOrdersRequest request, CancellationToken ct)
            => await GetOpenSpotOrdersAsync(request, ct).ConfigureAwait(false);

        public async Task<HttpResult<SharedSpotOrder[]>> GetOpenSpotOrdersAsync(GetOpenOrdersRequest request, CancellationToken ct)
        {
            var validationError = GetOpenSpotOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder[]>(Exchange, validationError);

            var symbol = request.Symbol?.GetSymbol(FormatSymbol);
            var result = await _api.Trading.GetOpenOrdersAsync(symbol, ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotOrder[]>(result);

            return HttpResult.Ok(result, result.Data.Select(x => new SharedSpotOrder(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, x.Symbol),
                x.Symbol,
                x.Id.ToString(),
                ParseOrderType(x.Type, x.Flags),
                x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                ParseOrderStatus(x.Status),
                x.CreateTime)
            {
                ClientOrderId = x.ClientOrderId?.ToString(),
                AveragePrice = x.PriceAverage == 0 ? null : x.PriceAverage,
                OrderQuantity = new SharedOrderQuantity(x.Quantity),
                QuantityFilled = new SharedOrderQuantity(x.Quantity - x.QuantityRemaining),
                TimeInForce = ParseTimeInForce(x.Type, x.Flags),
                UpdateTime = x.UpdateTime,
                IsTriggerOrder = x.Type == OrderType.ExchangeStop || x.Type == OrderType.ExchangeStopLimit,
                OrderPrice = x.Type == OrderType.ExchangeStop || x.Type == OrderType.ExchangeStopLimit ? x.PriceAuxilliaryLimit : x.Price,
                TriggerPrice = x.Type == OrderType.ExchangeStop || x.Type == OrderType.ExchangeStopLimit ? x.Price : null
            }).ToArray());
        }

        #endregion

        #region Get Closed Spot Orders

        public GetSpotClosedOrdersOptions GetClosedSpotOrdersOptions { get; } = new GetSpotClosedOrdersOptions(_exchangeName, false, true, true, 2500);
        async Task<ICallResult<SharedSpotOrder[]>> IGetClosedSpotOrders.GetClosedSpotOrdersAsync(GetClosedOrdersRequest request, PageRequest? pageRequest, CancellationToken ct)
            => await GetClosedSpotOrdersAsync(request, pageRequest, ct).ConfigureAwait(false);

        public async Task<HttpResult<SharedSpotOrder[]>> GetClosedSpotOrdersAsync(GetClosedOrdersRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetClosedSpotOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder[]>(Exchange, validationError);

            var direction = DataDirection.Descending;
            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var limit = request.Limit ?? 2500;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

            // Get data
            var result = await _api.Trading.GetClosedOrdersAsync(
                symbol,
                startTime: pageParams.StartTime,
                endTime: pageParams.EndTime,
                limit: limit,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotOrder[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                    () => Pagination.NextPageFromTime(pageParams, result.Data.Min(x => x.CreateTime)),
                    result.Data.Length,
                    result.Data.Select(x => x.CreateTime),
                    request.StartTime,
                    request.EndTime ?? DateTime.UtcNow,
                    pageParams);

            // Return
            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data, x => x.CreateTime, request.StartTime, request.EndTime, direction)
                .Select(x =>
                    new SharedSpotOrder(
                        ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, x.Symbol),
                        x.Symbol,
                        x.Id.ToString(),
                        ParseOrderType(x.Type, x.Flags),
                        x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                        ParseOrderStatus(x.Status),
                        x.CreateTime)
                    {
                        ClientOrderId = x.ClientOrderId?.ToString(),
                        AveragePrice = x.PriceAverage == 0 ? null : x.PriceAverage,
                        OrderQuantity = new SharedOrderQuantity(x.Quantity),
                        QuantityFilled = new SharedOrderQuantity(x.Quantity - x.QuantityRemaining),
                        TimeInForce = ParseTimeInForce(x.Type, x.Flags),
                        UpdateTime = x.UpdateTime,
                        IsTriggerOrder = x.Type == OrderType.ExchangeStop || x.Type == OrderType.ExchangeStopLimit,
                        OrderPrice = x.Type == OrderType.ExchangeStop || x.Type == OrderType.ExchangeStopLimit ? x.PriceAuxilliaryLimit : x.Price,
                        TriggerPrice = x.Type == OrderType.ExchangeStop || x.Type == OrderType.ExchangeStopLimit ? x.Price : null
                    }).ToArray(), nextPageRequest);
        }

        #endregion

        #region Get Spot Order Trades

        public GetSpotOrderTradesOptions GetSpotOrderTradesOptions { get; } = new GetSpotOrderTradesOptions(_exchangeName, true);
        async Task<ICallResult<SharedUserTrade[]>> IGetSpotOrderTrades.GetSpotOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
            => await GetSpotOrderTradesAsync(request, ct).ConfigureAwait(false);

        public async Task<HttpResult<SharedUserTrade[]>> GetSpotOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
        {
            var validationError = GetSpotOrderTradesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, ArgumentError.Invalid(nameof(GetOrderTradesRequest.OrderId), "Invalid order id"));

            var order = await _api.Trading.GetOrderTradesAsync(
                request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedUserTrade[]>(order);

            return HttpResult.Ok(order, order.Data.Select(x => new SharedUserTrade(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, x.Symbol),
                x.Symbol,
                x.OrderId.ToString(),
                x.Id.ToString(),
                x.QuantityRaw > 0 ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                new SharedOrderQuantity(x.Quantity),
                x.Price,
                x.Timestamp)
            {
                Fee = Math.Abs(x.Fee),
                FeeAsset = BitfinexExchange.AssetAliases.ExchangeToCommonName(x.FeeAsset),
                Role = x.Maker == true ? SharedRole.Maker : x.Maker == false ? SharedRole.Taker : null,
                ClientOrderId = x.ClientOrderId?.ToString()
            }).ToArray());
        }

        #endregion

        #region Get Spot User Trade History

        Task<HttpResult<SharedUserTrade[]>> ISpotOrderRestClient.GetSpotUserTradesAsync(GetUserTradesRequest request, PageRequest? nextPageToken, CancellationToken ct)
            => GetSpotUserTradeHistoryAsync(request, nextPageToken, ct);
        GetSpotUserTradeHistoryOptions ISpotOrderRestClient.GetSpotUserTradesOptions => GetSpotUserTradeHistoryOptions;

        public GetSpotUserTradeHistoryOptions GetSpotUserTradeHistoryOptions { get; } = new GetSpotUserTradeHistoryOptions(_exchangeName, false, true, true, 2500);
        async Task<ICallResult<SharedUserTrade[]>> IGetSpotUserTradeHistory.GetSpotUserTradeHistoryAsync(GetUserTradesRequest request, PageRequest? pageRequest, CancellationToken ct)
            => await GetSpotUserTradeHistoryAsync(request, pageRequest, ct).ConfigureAwait(false);

        public async Task<HttpResult<SharedUserTrade[]>> GetSpotUserTradeHistoryAsync(GetUserTradesRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetSpotUserTradeHistoryOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, validationError);

            var direction = DataDirection.Descending;
            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var limit = request.Limit ?? 1000;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

            // Get data
            var result = await _api.Trading.GetUserTradesAsync(
                symbol,
                startTime: pageParams.StartTime,
                endTime: pageParams.EndTime,
                limit: limit,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedUserTrade[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                    () => Pagination.NextPageFromTime(pageParams, result.Data.Min(x => x.Timestamp)),
                    result.Data.Length,
                    result.Data.Select(x => x.Timestamp),
                    request.StartTime,
                    request.EndTime ?? DateTime.UtcNow,
                    pageParams);

            // Return
            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data, x => x.Timestamp, request.StartTime, request.EndTime, direction)
                .Select(x => new SharedUserTrade(
                    ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, x.Symbol),
                    x.Symbol,
                    x.OrderId.ToString(),
                    x.Id.ToString(),
                    x.QuantityRaw > 0 ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                    new SharedOrderQuantity(x.Quantity),
                    x.Price,
                    x.Timestamp)
                {
                    Fee = Math.Abs(x.Fee),
                    FeeAsset = BitfinexExchange.AssetAliases.ExchangeToCommonName(x.FeeAsset),
                    Role = x.Maker == true ? SharedRole.Maker : x.Maker == false ? SharedRole.Taker : null,
                    ClientOrderId = x.ClientOrderId?.ToString()
                })
                .ToArray(), nextPageRequest);
        }

        #endregion

        #region Cancel Spot Order

        public CancelSpotOrderOptions CancelSpotOrderOptions { get; } = new CancelSpotOrderOptions(_exchangeName, true);
        async Task<ICallResult<SharedId>> ICancelSpotOrder.CancelSpotOrderAsync(CancelOrderRequest request, CancellationToken ct)
            => await CancelSpotOrderAsync(request, ct).ConfigureAwait(false);

        public async Task<HttpResult<SharedId>> CancelSpotOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = CancelSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await _api.Trading.CancelOrderAsync(orderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(order.Data.ToString()));
        }

        #endregion

        private SharedOrderStatus ParseOrderStatus(OrderStatus status)
        {
            if (status == Enums.OrderStatus.Canceled)
                return SharedOrderStatus.Canceled;
            if (status == Enums.OrderStatus.Active || status == Enums.OrderStatus.PartiallyFilled)
                return SharedOrderStatus.Open;
            if (status == OrderStatus.Executed || status == OrderStatus.ForcefullyExecuted)
                return SharedOrderStatus.Filled;

            return SharedOrderStatus.Unknown;
        }

        private SharedOrderType ParseOrderType(Enums.OrderType type, OrderFlags? flags)
        {
            if (type == Enums.OrderType.ExchangeMarket) return SharedOrderType.Market;
            if (type == Enums.OrderType.ExchangeLimit && (flags != null && flags.Value.HasFlag(OrderFlags.PostOnly))) return SharedOrderType.LimitMaker;
            if (type == Enums.OrderType.ExchangeLimit) return SharedOrderType.Limit;
            if (type == Enums.OrderType.ExchangeFillOrKill) return SharedOrderType.Limit;
            if (type == Enums.OrderType.ExchangeImmediateOrCancel) return SharedOrderType.Limit;

            return SharedOrderType.Other;
        }

        private SharedTimeInForce? ParseTimeInForce(Enums.OrderType type, OrderFlags? flags)
        {
            if (type == OrderType.ExchangeFillOrKill) return SharedTimeInForce.FillOrKill;
            if (type == OrderType.ExchangeImmediateOrCancel) return SharedTimeInForce.ImmediateOrCancel;

            return null;
        }

        private Enums.OrderType GetPlaceOrderType(SharedOrderType type, SharedTimeInForce? tif)
        {
            if ((type == SharedOrderType.Limit || type == SharedOrderType.LimitMaker) && (tif == null || tif == SharedTimeInForce.GoodTillCanceled)) return Enums.OrderType.ExchangeLimit;
            if (type == SharedOrderType.Limit && tif == SharedTimeInForce.ImmediateOrCancel) return Enums.OrderType.ExchangeImmediateOrCancel;
            if (type == SharedOrderType.Limit && tif == SharedTimeInForce.FillOrKill) return Enums.OrderType.ExchangeFillOrKill;
            if (type == SharedOrderType.Market) return Enums.OrderType.ExchangeMarket;

            throw new ArgumentException($"The combination of order type `{type}` and time in force `{tif}` in invalid");
        }

    }
}
