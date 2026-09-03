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
        #region Recent Trade client

        public GetRecentTradesOptions GetRecentTradesOptions { get; } = new GetRecentTradesOptions(_exchangeName, 10000, false);
        public async Task<HttpResult<SharedTrade[]>> GetRecentTradesAsync(GetRecentTradesRequest request, CancellationToken ct)
        {
            var validationError = GetRecentTradesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedTrade[]>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await _api.ExchangeData.GetTradeHistoryAsync(
                symbol,
                limit: request.Limit,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedTrade[]>(result);

            return HttpResult.Ok(result, result.Data.Select(x =>
            new SharedTrade(request.Symbol, symbol, new SharedOrderQuantity(Math.Abs(x.Quantity)), x.Price, x.Timestamp)
            {
                Side = x.Quantity > 0 ? SharedOrderSide.Buy : SharedOrderSide.Sell
            }).ToArray());
        }

        #endregion

        #region Trade History client

        public GetTradeHistoryOptions GetTradeHistoryOptions { get; } = new GetTradeHistoryOptions(_exchangeName, true, true, true, 10000, false);
        public async Task<HttpResult<SharedTrade[]>> GetTradeHistoryAsync(GetTradeHistoryRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetTradeHistoryOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedTrade[]>(Exchange, validationError);

            var direction = request.Direction ?? DataDirection.Ascending;
            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var limit = request.Limit ?? 10000;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

            var result = await _api.ExchangeData.GetTradeHistoryAsync(
                symbol,
                startTime: pageParams.StartTime,
                endTime: pageParams.EndTime,
                limit: limit,
                sorting: direction == DataDirection.Ascending ? Sorting.OldFirst : Sorting.NewFirst,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedTrade[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                    () => direction == DataDirection.Ascending
                        ? Pagination.NextPageFromTime(pageParams, result.Data.Max(x => x.Timestamp))
                        : Pagination.NextPageFromTime(pageParams, result.Data.Min(x => x.Timestamp)),
                    result.Data.Length,
                    result.Data.Select(x => x.Timestamp),
                    request.StartTime,
                    request.EndTime ?? DateTime.UtcNow,
                    pageParams,
                    TimeSpan.FromDays(90));

            // Return
            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data, x => x.Timestamp, request.StartTime, request.EndTime, direction)
                    .Select(x =>
                        new SharedTrade(request.Symbol, symbol, new SharedOrderQuantity(Math.Abs(x.Quantity)), x.Price, x.Timestamp)
                        {
                            Side = x.Quantity > 0 ? SharedOrderSide.Buy : SharedOrderSide.Sell
                        }).ToArray(), nextPageRequest);
        }
        #endregion
    }
}
