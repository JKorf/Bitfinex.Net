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
        #region Trade client

        public SubscribeTradeOptions SubscribeTradeOptions { get; } = new SubscribeTradeOptions(_exchangeName, false);
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(SubscribeTradeRequest request, Action<DataEvent<SharedTrade[]>> handler, CancellationToken ct)
        {
            var validationError = SubscribeTradeOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await _api.SubscribeToTradeUpdatesAsync(symbol, update =>
            {
                if (update.UpdateType == SocketUpdateType.Snapshot || update.StreamId!.EndsWith(".tu"))
                    return;

                handler(update.ToType<SharedTrade[]>(update.Data.Select(x => 
                new SharedTrade(
                    request.Symbol, 
                    symbol,
                    new SharedOrderQuantity(x.QuantityAbs),
                    x.Price,
                    x.Timestamp)
                {
                    Side = x.Quantity > 0 ? SharedOrderSide.Buy : SharedOrderSide.Sell
                }).ToArray()));
            }, ct).ConfigureAwait(false);

            return result;
        }
        #endregion
    }
}
