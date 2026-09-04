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
        #region Get Futures Ticker

        public GetFuturesTickerOptions GetFuturesTickerOptions { get; } = new GetFuturesTickerOptions(_exchangeName);
        async Task<ICallResult<SharedFuturesTicker>> IGetFuturesTicker.GetFuturesTickerAsync(GetTickerRequest request, CancellationToken ct)
            => await GetFuturesTickerAsync(request, ct).ConfigureAwait(false);

        public async Task<HttpResult<SharedFuturesTicker>> GetFuturesTickerAsync(GetTickerRequest request, CancellationToken ct)
        {
            var validationError = GetFuturesTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesTicker>(Exchange, validationError);

            var result = await _api.ExchangeData.GetTickerAsync(request.Symbol!.GetSymbol(FormatSymbol), ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFuturesTicker>(result);

            return HttpResult.Ok(result, 
                new SharedFuturesTicker(
                    ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, result.Data.Symbol),
                    result.Data.Symbol,
                    result.Data.LastPrice,
                    result.Data.HighPrice,
                    result.Data.LowPrice,
                    new SharedOrderQuantity(result.Data.Volume),
                    Math.Round(result.Data.DailyChangePercentage * 100, 2)));
        }

        #endregion

        #region Get All Futures Tickers

        Task<HttpResult<SharedFuturesTicker[]>> IFuturesTickerRestClient.GetFuturesTickersAsync(GetTickersRequest request, CancellationToken ct)
            => GetAllFuturesTickersAsync(request, ct);
        GetAllFuturesTickersOptions IFuturesTickerRestClient.GetFuturesTickersOptions => GetAllFuturesTickersOptions;

        public GetAllFuturesTickersOptions GetAllFuturesTickersOptions { get; } = new GetAllFuturesTickersOptions(_exchangeName);
        async Task<ICallResult<SharedFuturesTicker[]>> IGetAllFuturesTickers.GetAllFuturesTickersAsync(GetTickersRequest request, CancellationToken ct)
            => await GetAllFuturesTickersAsync(request, ct).ConfigureAwait(false);

        public async Task<HttpResult<SharedFuturesTicker[]>> GetAllFuturesTickersAsync(GetTickersRequest request, CancellationToken ct)
        {
            var validationError = GetAllSpotTickersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesTicker[]>(Exchange, validationError);

            var result = await _api.ExchangeData.GetTickersAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFuturesTicker[]>(result);

            return HttpResult.Ok(result, result.Data.Where(x => x.Symbol.Contains("F0")).Select(x => 
                new SharedFuturesTicker(
                    ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, x.Symbol), 
                    x.Symbol, 
                    x.LastPrice,
                    x.HighPrice,
                    x.LowPrice,
                    new SharedOrderQuantity(x.Volume),
                    Math.Round(x.DailyChangePercentage * 100, 2))).ToArray());
        }

        #endregion

    }
}
