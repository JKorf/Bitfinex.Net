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
        #region Balance client
        public SubscribeBalanceOptions SubscribeBalanceOptions { get; } = new SubscribeBalanceOptions(_exchangeName, false);
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToBalanceUpdatesAsync(SubscribeBalancesRequest request, Action<DataEvent<SharedBalance[]>> handler, CancellationToken ct)
        {
            var validationError = SubscribeBalanceOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var result = await _api.SubscribeToUserUpdatesAsync(
                walletHandler: update => {
                    if (update.UpdateType == SocketUpdateType.Snapshot)
                        return;

                    var updateData = update.Data.Where(x => x.Type == Enums.WalletType.Exchange);
                    if (!updateData.Any())
                        return;

                    handler(update.ToType<SharedBalance[]>(updateData.Select(x => 
                        new SharedBalance(
                            SupportedTradingModes,
                            BitfinexExchange.AssetAliases.ExchangeToCommonName(x.Asset),
                            x.Available ?? x.Total,
                            x.Total)).ToArray()));
                },
                ct: ct).ConfigureAwait(false);

            return result;
        }
        #endregion
    }
}
