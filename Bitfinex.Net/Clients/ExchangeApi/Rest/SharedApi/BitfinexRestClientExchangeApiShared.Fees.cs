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
        #region Get Fees

        public GetFeeOptions GetFeeOptions { get; } = new GetFeeOptions(_exchangeName, true);

        async Task<ICallResult<SharedFee>> IGetFees.GetFeesAsync(GetFeeRequest request, CancellationToken ct)
            => await GetFeesAsync(request, ct).ConfigureAwait(false);

        public async Task<HttpResult<SharedFee>> GetFeesAsync(GetFeeRequest request, CancellationToken ct)
        {
            var validationError = GetFeeOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFee>(Exchange, validationError);

            // Get data
            var result = await _api.Account.Get30DaySummaryAndFeesAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFee>(result);

            // Return
            return HttpResult.Ok(result, new SharedFee(result.Data.Fees.MakerFees.MakerFee * 100, result.Data.Fees.TakerFees.TakerFeeCrypto * 100));
        }

        #endregion
    }
}
