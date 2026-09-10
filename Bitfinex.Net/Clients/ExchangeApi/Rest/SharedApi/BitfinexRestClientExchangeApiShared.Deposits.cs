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
        #region Get Deposit Addresses

        public GetDepositAddressesOptions GetDepositAddressesOptions { get; } = new GetDepositAddressesOptions(_exchangeName, true)
        {
            ParameterRuleOverrides = [
                RequestParameterRuleOverride<GetDepositAddressesRequest>.Required(x => x.Network)
            ]
        };

        async Task<ICallResult<SharedDepositAddress[]>> IGetDepositAddresses.GetDepositAddressesAsync(GetDepositAddressesRequest request, CancellationToken ct)
            => await GetDepositAddressesAsync(request, ct).ConfigureAwait(false);

        public async Task<HttpResult<SharedDepositAddress[]>> GetDepositAddressesAsync(GetDepositAddressesRequest request, CancellationToken ct)
        {

            var validationError = GetDepositAddressesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedDepositAddress[]>(Exchange, validationError);

            var depositAddresses = await _api.Account.GetDepositAddressAsync(request.Network!, WithdrawWallet.Deposit, false, ct).ConfigureAwait(false);
            if (!depositAddresses.Success)
                return HttpResult.Fail<SharedDepositAddress[]>(depositAddresses);

            return HttpResult.Ok(depositAddresses, new[] {
                new SharedDepositAddress(BitfinexExchange.AssetAliases.ExchangeToCommonName(depositAddresses.Data.Data!.Asset), !string.IsNullOrEmpty(depositAddresses.Data.Data.PoolAddress) ? depositAddresses.Data.Data.PoolAddress : depositAddresses.Data.Data.Address)
                {
                    Network = request.Network,
                    TagOrMemo = !string.IsNullOrEmpty(depositAddresses.Data.Data.PoolAddress) ? depositAddresses.Data.Data.Address : null,
                }
            });
        }

        #endregion

        #region Get Deposit History

        Task<HttpResult<SharedDeposit[]>> IDepositRestClient.GetDepositsAsync(GetDepositsRequest request, PageRequest? nextPageToken, CancellationToken ct)
            => GetDepositHistoryAsync(request, nextPageToken, ct);
        GetDepositHistoryOptions IDepositRestClient.GetDepositsOptions => GetDepositHistoryOptions;

        public GetDepositHistoryOptions GetDepositHistoryOptions { get; } = new GetDepositHistoryOptions(_exchangeName, false, true, true, 1000);
        async Task<ICallResult<SharedDeposit[]>> IGetDepositHistory.GetDepositHistoryAsync(GetDepositsRequest request, PageRequest? pageRequest, CancellationToken ct)
            => await GetDepositHistoryAsync(request, pageRequest, ct).ConfigureAwait(false);

        public async Task<HttpResult<SharedDeposit[]>> GetDepositHistoryAsync(GetDepositsRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetDepositHistoryOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedDeposit[]>(Exchange, validationError);

            var direction = DataDirection.Descending;
            var limit = request.Limit ?? 1000;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

            // Get data
            var result = await _api.Account.GetMovementsAsync(
                request.Asset,
                startTime: pageParams.StartTime,
                endTime: pageParams.EndTime,
                limit: limit,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedDeposit[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                    () => Pagination.NextPageFromTime(pageParams, result.Data.Min(x => x.StartTime)),
                    result.Data.Length,
                    result.Data.Select(x => x.StartTime),
                    request.StartTime,
                    request.EndTime ?? DateTime.UtcNow,
                    pageParams);

            var data = result.Data.Where(x => x.Quantity > 0);

            // Return
            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(data, x => x.StartTime, request.StartTime, request.EndTime, direction)
                .Select(x =>
                    new SharedDeposit(
                        BitfinexExchange.AssetAliases.ExchangeToCommonName(x.Asset),
                        x.Quantity,
                        x.Status == "COMPLETED",
                        x.StartTime,
                        x.Status == "COMPLETED" ? SharedTransferStatus.Completed :
                        x.Status == "CANCELED" ? SharedTransferStatus.Failed :
                        SharedTransferStatus.Unknown)
                    {
                        Id = x.Id,
                        TransactionId = x.TransactionId
                    })
                .ToArray(), nextPageRequest);
        }

        #endregion

    }
}
