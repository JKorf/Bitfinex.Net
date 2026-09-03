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
        #region Trigger Order Client
        public PlaceSpotTriggerOrderOptions PlaceSpotTriggerOrderOptions { get; } = new PlaceSpotTriggerOrderOptions(_exchangeName, true);
        public async Task<HttpResult<SharedId>> PlaceSpotTriggerOrderAsync(PlaceSpotTriggerOrderRequest request, CancellationToken ct)
        {
            var validationError = PlaceSpotTriggerOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            int clientOrderId = 0;
            if (request.ClientOrderId != null && !int.TryParse(request.ClientOrderId, out clientOrderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(PlaceSpotTriggerOrderRequest.ClientOrderId), "ClientOrderId needs to be parsable to `int` for `Bitfinex`"));

            var result = await _api.Trading.PlaceOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                request.OrderSide == SharedOrderSide.Buy ? OrderSide.Buy : OrderSide.Sell,
                request.OrderPrice == null ? OrderType.ExchangeStop : OrderType.ExchangeStopLimit,
                quantity: request.Quantity?.QuantityInBaseAsset ?? 0,
                price: request.TriggerPrice,
                priceAuxLimit: request.OrderPrice,
                clientOrderId: request.ClientOrderId != null ? clientOrderId : null,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedId>(result);

            // Return
            return HttpResult.Ok(result, new SharedId(result.Data.Id.ToString()));
        }

        public GetSpotTriggerOrderOptions GetSpotTriggerOrderOptions { get; } = new GetSpotTriggerOrderOptions(_exchangeName, true);
        public async Task<HttpResult<SharedSpotTriggerOrder>> GetSpotTriggerOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = GetSpotTriggerOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotTriggerOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var id))
                throw new ArgumentException($"Invalid order id");

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await _api.Trading.GetOpenOrdersAsync(symbol, new[] { id }, ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotTriggerOrder>(result);

            if (!result.Data.Any())
                result = await _api.Trading.GetClosedOrdersAsync(symbol, new[] { id }, ct: ct).ConfigureAwait(false);

            if (!result.Success)
                return HttpResult.Fail<SharedSpotTriggerOrder>(result);

            if (!result.Data.Any())
                return HttpResult.Fail<SharedSpotTriggerOrder>(result, new ServerError(new ErrorInfo(ErrorType.UnknownOrder, $"Order with id {id} not found")));

            var order = result.Data.Single();
            return HttpResult.Ok(result, new SharedSpotTriggerOrder(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, order.Symbol),
                order.Symbol,
                order.Id.ToString(),
                order.Type == OrderType.ExchangeStop ? SharedOrderType.Market : SharedOrderType.Limit,
                order.Side == OrderSide.Buy ? SharedTriggerOrderDirection.Enter : SharedTriggerOrderDirection.Exit,
                ParseTriggerOrderStatus(order),
                order.Price,
                order.CreateTime)
            {
                PlacedOrderId = order.Id.ToString(),
                AveragePrice = order.PriceAverage == 0 ? null : order.PriceAverage,
                OrderPrice = order.PriceAuxilliaryLimit,
                OrderQuantity = new SharedOrderQuantity(order.Quantity),
                QuantityFilled = new SharedOrderQuantity(order.Quantity - order.QuantityRemaining),
                TimeInForce = ParseTimeInForce(order.Type, order.Flags),
                UpdateTime = order.UpdateTime,
                ClientOrderId = order.ClientOrderId?.ToString()
            });
        }

        private SharedTriggerOrderStatus ParseTriggerOrderStatus(BitfinexOrder order)
        {
            if (order.Status == OrderStatus.Executed || order.Status == OrderStatus.ForcefullyExecuted)
                return SharedTriggerOrderStatus.Filled;

            if (order.Status == OrderStatus.Canceled)
                return SharedTriggerOrderStatus.CanceledOrRejected;

            if (order.Status == OrderStatus.Active || order.Status == OrderStatus.PartiallyFilled)
                return SharedTriggerOrderStatus.Active;

            return SharedTriggerOrderStatus.Unknown;
        }

        public CancelSpotTriggerOrderOptions CancelSpotTriggerOrderOptions { get; } = new CancelSpotTriggerOrderOptions(_exchangeName, true);
        public async Task<HttpResult<SharedId>> CancelSpotTriggerOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = CancelSpotTriggerOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await _api.Trading.CancelOrderAsync(orderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(order.Data.Id.ToString()));
        }
        #endregion
    }
}
