using Bitfinex.Net;
using Bitfinex.Net.Interfaces.Clients;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.SharedApis.Interfaces;
using CryptoExchange.Net.SharedApis.RequestModels;
using CryptoExchange.Net.SharedApis.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients.SpotApi
{
    internal partial class BitfinexRestClientSpotApi : IBitfinexRestClientSpotApiShared
    {
        public string Exchange => BitfinexExchange.ExchangeName;

        async Task<WebCallResult<IEnumerable<SharedKline>>> IKlineRestClient.GetKlinesAsync(GetKlinesRequest request, CancellationToken ct)
        {
            var interval = (Enums.KlineInterval)request.Interval.TotalSeconds;
            if (!Enum.IsDefined(typeof(Enums.KlineInterval), interval))
                return new WebCallResult<IEnumerable<SharedKline>>(new ArgumentError("Interval not supported"));

            var baseAsset = request.BaseAsset == "USDT" ? "USD" : request.BaseAsset;
            var quoteAsset = request.QuoteAsset == "USDT" ? "USD" : request.QuoteAsset;

            var result = await ExchangeData.GetKlinesAsync(
                FormatSymbol(baseAsset, quoteAsset, request.ApiType),
                interval,
                startTime: request.StartTime,
                endTime: request.EndTime,
                limit: request.Limit,
                ct: ct
                ).ConfigureAwait(false);

            if (!result)
                return result.As<IEnumerable<SharedKline>>(default);

            // Reverse as data is returned in desc order instead of standard asc
            return result.As(result.Data.Select(x => new SharedKline
            {
                BaseVolume = x.Volume,
                ClosePrice = x.ClosePrice,
                HighPrice = x.HighPrice,
                LowPrice = x.LowPrice,
                OpenPrice = x.OpenPrice,
                OpenTime = x.OpenTime
            }));
        }

        async Task<WebCallResult<IEnumerable<SharedSpotSymbol>>> ISpotSymbolRestClient.GetSymbolsAsync(SharedRequest request, CancellationToken ct)
        {
            var result = await ExchangeData.GetSymbolsAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<SharedSpotSymbol>>(default);

            return result.As(result.Data.Select(s => new SharedSpotSymbol
            {
                BaseAsset = s.Key.Split(new[] { ':' })[0],
                QuoteAsset = s.Key.Split(new[] { ':' })[1],
                Name = s.Key,
                MinTradeQuantity = s.Value.MinOrderQuantity,
                MaxTradeQuantity = s.Value.MaxOrderQuantity
            }));
        }

        async Task<WebCallResult<SharedTicker>> ITickerRestClient.GetTickerAsync(GetTickerRequest request, CancellationToken ct)
        {
            var baseAsset = request.BaseAsset == "USDT" ? "UST" : request.BaseAsset;
            var quoteAsset = request.QuoteAsset == "USDT" ? "UST" : request.QuoteAsset;

            var result = await ExchangeData.GetTickerAsync(FormatSymbol(baseAsset, quoteAsset, request.ApiType), ct).ConfigureAwait(false);
            if (!result)
                return result.As<SharedTicker>(default);

            return result.As(new SharedTicker
            {
                HighPrice = result.Data.HighPrice,
                LastPrice = result.Data.LastPrice,
                LowPrice = result.Data.LowPrice,
            });
        }

        async Task<WebCallResult<IEnumerable<SharedTicker>>> ITickerRestClient.GetTickersAsync(SharedRequest request, CancellationToken ct)
        {
            var result = await ExchangeData.GetTickersAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<SharedTicker>>(default);

            return result.As<IEnumerable<SharedTicker>>(result.Data.Select(x => new SharedTicker
            {
                Symbol = x.Symbol,
                HighPrice = x.HighPrice,
                LastPrice = x.LastPrice,
                LowPrice = x.LowPrice,
            }));
        }

        async Task<WebCallResult<IEnumerable<SharedTrade>>> ITradeRestClient.GetTradesAsync(GetTradesRequest request, CancellationToken ct)
        {
            var baseAsset = request.BaseAsset == "USDT" ? "UST" : request.BaseAsset;
            var quoteAsset = request.QuoteAsset == "USDT" ? "UST" : request.QuoteAsset;

            var result = await ExchangeData.GetTradeHistoryAsync(
                FormatSymbol(baseAsset, quoteAsset, request.ApiType),
                startTime: request.StartTime,
                endTime: request.EndTime,
                limit: request.Limit,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<SharedTrade>>(default);

            return result.As(result.Data.Select(x => new SharedTrade
            {
                Price = x.Price,
                Quantity = x.Quantity,
                Timestamp = x.Timestamp
            }));
        }
    }
}
