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
        #region Get Withdrawal History

        Task<HttpResult<SharedWithdrawal[]>> IWithdrawalRestClient.GetWithdrawalsAsync(GetWithdrawalsRequest request, PageRequest? nextPageToken, CancellationToken ct)
            => GetWithdrawalHistoryAsync(request, nextPageToken, ct);
        GetWithdrawalHistoryOptions IWithdrawalRestClient.GetWithdrawalsOptions => GetWithdrawalHistoryOptions;

        public GetWithdrawalHistoryOptions GetWithdrawalHistoryOptions { get; } = new GetWithdrawalHistoryOptions(_exchangeName, false, true, true, 1000);
        async Task<ICallResult<SharedWithdrawal[]>> IGetWithdrawalHistory.GetWithdrawalHistoryAsync(GetWithdrawalsRequest request, PageRequest? pageRequest, CancellationToken ct)
            => await GetWithdrawalHistoryAsync(request, pageRequest, ct).ConfigureAwait(false);

        public async Task<HttpResult<SharedWithdrawal[]>> GetWithdrawalHistoryAsync(GetWithdrawalsRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetWithdrawalHistoryOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedWithdrawal[]>(Exchange, validationError);

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
                return HttpResult.Fail<SharedWithdrawal[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                    () => direction == DataDirection.Ascending
                        ? Pagination.NextPageFromTime(pageParams, result.Data.Max(x => x.StartTime))
                        : Pagination.NextPageFromTime(pageParams, result.Data.Min(x => x.StartTime)),
                    result.Data.Length,
                    result.Data.Select(x => x.StartTime),
                    request.StartTime,
                    request.EndTime ?? DateTime.UtcNow,
                    pageParams);

            var data = result.Data.Where(x => x.Quantity < 0);

            // Return
            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(data, x => x.StartTime, request.StartTime, request.EndTime, direction)
                .Select(x =>
                    new SharedWithdrawal(
                        BitfinexExchange.AssetAliases.ExchangeToCommonName(x.Asset), 
                        x.Address, 
                        Math.Abs(x.Quantity),
                        x.Status == "COMPLETED",
                        x.StartTime,
                        GetWithdrawalStatus(x))
                    {
                        Id = x.Id,
                        TransactionId = x.TransactionId
                    })
                .ToArray(), nextPageRequest);
        }

        #endregion

        private SharedTransferStatus GetWithdrawalStatus(BitfinexMovement x)
        {
            if (x.Status == "COMPLETED")
                return SharedTransferStatus.Completed;

            return SharedTransferStatus.Unknown;
        }

        #region Withdraw

        public WithdrawOptions WithdrawOptions { get; } = new WithdrawOptions(_exchangeName)
        {
            RequiredRequestParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(WithdrawRequest.Network), typeof(string), "Network to use", "tetheruse")
            }
        };

        async Task<ICallResult<SharedId>> IWithdraw.WithdrawAsync(WithdrawRequest request, CancellationToken ct)
            => await WithdrawAsync(request, ct).ConfigureAwait(false);

        public async Task<HttpResult<SharedId>> WithdrawAsync(WithdrawRequest request, CancellationToken ct)
        {
            var validationError = WithdrawOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            // Get data
            var withdrawal = await _api.Account.WithdrawV2Async(
                request.Network!,
                WithdrawWallet.Deposit,
                request.Quantity,
                address: request.Address,
                paymentId: request.AddressTag,
                ct: ct).ConfigureAwait(false);
            if (!withdrawal.Success)
                return HttpResult.Fail<SharedId>(withdrawal);

            return HttpResult.Ok(withdrawal, new SharedId(withdrawal.Data.Data.WithdrawalId.ToString()));
        }

        #endregion

    }
}
