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
        #region Get Balances

        public GetBalancesOptions GetBalancesOptions { get; } = new GetBalancesOptions(_exchangeName, AccountTypeFilter.Funding, AccountTypeFilter.Spot, AccountTypeFilter.Margin);

        async Task<ICallResult<SharedBalance[]>> IGetBalances.GetBalancesAsync(GetBalancesRequest request, CancellationToken ct)
            => await GetBalancesAsync(request, ct).ConfigureAwait(false);

        public async Task<HttpResult<SharedBalance[]>> GetBalancesAsync(GetBalancesRequest request, CancellationToken ct)
        {
            var validationError = GetBalancesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedBalance[]>(Exchange, validationError);

            var result = await _api.Account.GetBalancesAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedBalance[]>(result);

            var filterType = request.AccountType?.IsMarginAccount() == true ? WalletType.Margin : request.AccountType == SharedAccountType.Funding ? WalletType.Funding : WalletType.Exchange;
            return HttpResult.Ok(result, result.Data.Where(x => x.Type == filterType).Select(x =>
                new SharedBalance(
                        SupportedTradingModes, 
                        BitfinexExchange.AssetAliases.ExchangeToCommonName(x.Asset),
                        x.Available ?? 0, 
                        x.Total)).ToArray());
        }

        #endregion

    }
}
