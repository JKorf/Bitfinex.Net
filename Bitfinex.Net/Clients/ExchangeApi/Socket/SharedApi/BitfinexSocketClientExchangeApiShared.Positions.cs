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
        #region Position client
        public SubscribePositionOptions SubscribePositionOptions { get; }
            = new SubscribePositionOptions(_exchangeName, true);
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToPositionUpdatesAsync(SubscribePositionRequest request, Action<DataEvent<SharedPosition[]>> handler, CancellationToken ct)
        {
            var validationError = SubscribePositionOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var result = await _api.SubscribeToUserUpdatesAsync(
                positionHandler: update => handler(update.ToType(update.Data.Select(x => 
                    new SharedPosition(
                        ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, x.Symbol),
                        x.Symbol,
                        new SharedOrderQuantity(Math.Abs(x.Quantity)),
                        x.UpdateTime)
                    {
                        AverageOpenPrice = x.BasePrice,
                        LiquidationPrice = x.LiquidationPrice,
                        Leverage = x.Leverage,
                        PositionMode = SharedPositionMode.OneWay,
                        PositionSide = x.Quantity >= 0 ? SharedPositionSide.Long : SharedPositionSide.Short,
                        UnrealizedPnl = x.ProfitLoss
                    }).ToArray())),
                ct: ct).ConfigureAwait(false);

            return result;
        }

        #endregion
    }
}
