using Bitfinex.Net.Enums;
using Bitfinex.Net.Interfaces.Clients.ExchangeApi;
using Bitfinex.Net.Objects.Models;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.SharedApis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients.ExchangeApi
{
    internal partial class BitfinexSocketClientExchangeSharedApi
    {
        #region Spot Order client
        async Task<WebSocketResult<UpdateSubscription>> ISpotOrderSocketClient.SubscribeToSpotOrderUpdatesAsync(SubscribeSpotOrderRequest request, Action<DataEvent<SharedSpotOrder[]>> handler, CancellationToken ct)
            => await SubscribeToSpotOrderUpdatesAsync(request, x => handler(x.ToType<SharedSpotOrder[]>(x.Data)), ct).ConfigureAwait(false);

        public SubscribeSpotOrderOptions SubscribeSpotOrderOptions { get; } = new SubscribeSpotOrderOptions(_exchangeName, false);
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToSpotOrderUpdatesAsync(SubscribeSpotOrderRequest request, Action<DataEvent<SharedSpotOrderUpdate[]>> handler, CancellationToken ct)
        {
            var validationError = SubscribeSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var result = await _api.SubscribeToUserUpdatesAsync(
                orderHandler: update =>
                {
                    if (update.UpdateType == SocketUpdateType.Snapshot)
                        return;

                    var data = update.Data.Where(x => !x.Symbol.Contains("F0"));
                    if (!data.Any())
                        return;

                    handler(update.ToType<SharedSpotOrderUpdate[]>(data.Select(x =>
                        new SharedSpotOrderUpdate(
                            ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, x.Symbol),
                            x.Symbol,
                            x.Id.ToString(),
                            ParseOrderType(x.Type),
                            x.Side == Enums.OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                            ParseOrderStatus(x.Status),
                            x.CreateTime)
                        {
                            ClientOrderId = x.ClientOrderId.ToString(),
                            OrderQuantity = new SharedOrderQuantity(x.Quantity),
                            QuantityFilled = new SharedOrderQuantity(x.Quantity - x.QuantityRemaining),
                            AveragePrice = x.PriceAverage == 0 ? null : x.PriceAverage,
                            UpdateTime = x.UpdateTime,
                            IsTriggerOrder = x.Type == OrderType.ExchangeStop || x.Type == OrderType.ExchangeStopLimit,
                            OrderPrice = x.Type == OrderType.ExchangeStop || x.Type == OrderType.ExchangeStopLimit ? x.PriceAuxilliaryLimit : x.Price,
                            TriggerPrice = x.Type == OrderType.ExchangeStop || x.Type == OrderType.ExchangeStopLimit ? x.Price : null
                        }
                    ).ToArray()));
                },
                ct: ct).ConfigureAwait(false);

            return result;
        }

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

        private SharedOrderType ParseOrderType(OrderType type)
        {
            if (type == OrderType.ExchangeMarket || type == OrderType.ExchangeStop)
                return SharedOrderType.Market;

            if (type == OrderType.ExchangeLimit || type == OrderType.ExchangeStopLimit)
                return SharedOrderType.Limit;

            return SharedOrderType.Other;
        }
        #endregion

        #region Spot Order client

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

        PlaceSpotOrderOptions IPlaceSpotOrder.PlaceSpotOrderOptions
            => PlaceSpotOrderOptions;

        public PlaceSpotOrderSocketOptions PlaceSpotOrderOptions { get; } = new PlaceSpotOrderSocketOptions(_exchangeName);
        public async Task<QueryResult<SharedId>> PlaceSpotOrderAsync(PlaceSpotOrderRequest request, CancellationToken ct)
        {
            var validationError = PlaceSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return QueryResult.Fail<SharedId>(Exchange, validationError);

            int clientOrderId = 0;
            if (request.ClientOrderId != null && !int.TryParse(request.ClientOrderId, out clientOrderId))
                return QueryResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(PlaceSpotOrderRequest.ClientOrderId), "ClientOrderId needs to be parsable to `int` for `Bitfinex`"));

            var result = await _api.PlaceOrderAsync(
                request.Side == SharedOrderSide.Buy ? Enums.OrderSide.Buy : Enums.OrderSide.Sell,
                GetPlaceOrderType(request.OrderType, request.TimeInForce),
                request.Symbol!.GetSymbol(FormatSymbol),
                quantity: request.Quantity?.QuantityInBaseAsset ?? 0,
                flags: request.OrderType == SharedOrderType.LimitMaker ? Enums.OrderFlags.PostOnly : null,
                price: request.Price ?? 0,
                clientOrderId: request.ClientOrderId != null ? clientOrderId : null,
                ct: ct).ConfigureAwait(false);

            if (!result.Success)
                return QueryResult.Fail<SharedId>(result);

            return QueryResult.Ok(result, new SharedId(result.Data.Id.ToString()));
        }

        public CancelSpotOrderSocketOptions CancelSpotOrderOptions { get; } = new CancelSpotOrderSocketOptions(_exchangeName, true);
        public async Task<QueryResult<SharedId>> CancelSpotOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = CancelSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return QueryResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return QueryResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await _api.CancelOrderAsync(orderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return QueryResult.Fail<SharedId>(order);

            return QueryResult.Ok(order, new SharedId(order.Data.ToString()));
        }

        private Enums.OrderType GetPlaceOrderType(SharedOrderType type, SharedTimeInForce? tif)
        {
            if ((type == SharedOrderType.Limit || type == SharedOrderType.LimitMaker) && (tif == null || tif == SharedTimeInForce.GoodTillCanceled)) return Enums.OrderType.ExchangeLimit;
            if (type == SharedOrderType.Limit && tif == SharedTimeInForce.ImmediateOrCancel) return Enums.OrderType.ExchangeImmediateOrCancel;
            if (type == SharedOrderType.Limit && tif == SharedTimeInForce.FillOrKill) return Enums.OrderType.ExchangeFillOrKill;
            if (type == SharedOrderType.Market) return Enums.OrderType.ExchangeMarket;

            throw new ArgumentException($"The combination of order type `{type}` and time in force `{tif}` in invalid");
        }
        #endregion
    }
}
