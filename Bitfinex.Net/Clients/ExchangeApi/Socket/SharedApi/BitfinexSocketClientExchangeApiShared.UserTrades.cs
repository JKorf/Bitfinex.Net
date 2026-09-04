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
        #region Subscribe To User Trade Updates

        public SubscribeUserTradeOptions SubscribeUserTradeOptions { get; } = new SubscribeUserTradeOptions(_exchangeName, false);
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToUserTradeUpdatesAsync(SubscribeUserTradeRequest request, Action<DataEvent<SharedUserTrade[]>> handler, CancellationToken ct)
        {
            var validationError = SubscribeUserTradeOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var result = await _api.SubscribeToUserUpdatesAsync(
                tradeHandler: update =>
                {
                    if (request.TradingMode == TradingMode.Spot && update.Data.Symbol.Contains("F0"))
                        return;
                    else if (request.TradingMode == TradingMode.PerpetualLinear && !update.Data.Symbol.Contains("F0"))
                        return;

                    handler(update.ToType<SharedUserTrade[]>(new[] {
                        new SharedUserTrade(
                            ExchangeSymbolCache.ParseSymbol(update.Data.Symbol.Contains("F0") ? _topicFuturesId : _topicSpotId, _api.EnvironmentName, null, update.Data.Symbol),
                            update.Data.Symbol,
                            update.Data.OrderId.ToString(),
                            update.Data.Id.ToString(),
                            update.Data.QuantityRaw > 0 ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                            new SharedOrderQuantity(update.Data.Quantity),
                            update.Data.Price,
                            update.Data.Timestamp)
                        {
                            Fee = Math.Abs(update.Data.Fee),
                            FeeAsset = BitfinexExchange.AssetAliases.ExchangeToCommonName(update.Data.FeeAsset),
                            Role = update.Data.Maker == true ? SharedRole.Maker : SharedRole.Taker,
                            ClientOrderId = update.Data.ClientOrderId?.ToString()
                        }
                    }));
                },
                ct: ct).ConfigureAwait(false);

            return result;
        }

        #endregion
    }
}
