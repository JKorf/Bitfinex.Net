using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;
using CryptoExchange.Net;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.V1;
using Bitfinex.Net.Interfaces.Clients.SpotApi;

namespace Bitfinex.Net.Clients.SpotApi
{
    /// <inheritdoc />
    public class BitfinexClientSpotApiExchangeData : IBitfinexClientSpotApiExchangeData
    {
        private const string StatusEndpoint = "platform/status";
        private const string SymbolsEndpoint = "symbols";
        private const string CurrenciesEndpoint = "conf/pub:map:currency:label";
        private const string LendsEndpoint = "lends/{}";
        private const string ForeignExchangeEndpoint = "calc/fx";
        private const string StatsEndpoint = "stats1/{}:1m:{}:{}/{}";

        private const string FundingBookEndpoint = "lendbook/{}";
        private const string SymbolDetailsEndpoint = "symbols_details";
        private const string TickersEndpoint = "tickers";
        private const string TradesEndpoint = "trades/{}/hist";
        private const string OrderBookEndpoint = "book/{}/{}";
        private const string LastCandleEndpoint = "candles/trade:{}:{}/last";
        private const string LastFundingCandleEndpoint = "candles/trade:{}:{}:{}/last";
        private const string CandlesEndpoint = "candles/trade:{}:{}/hist";
        private const string FundingCandlesEndpoint = "candles/trade:{}:{}:{}/hist";
        private const string MarketAverageEndpoint = "calc/trade/avg";

        private readonly BitfinexClientSpotApi _baseClient;

        internal BitfinexClientSpotApiExchangeData(BitfinexClientSpotApi baseClient)
        {
            _baseClient = baseClient;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexPlatformStatus>> GetPlatformStatusAsync(CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<BitfinexPlatformStatus>(_baseClient.GetUrl(StatusEndpoint, "2"), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexAsset>>> GetAssetsAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.SendRequestAsync<IEnumerable<IEnumerable<BitfinexAsset>>>(_baseClient.GetUrl(CurrenciesEndpoint, "2"), HttpMethod.Get, ct).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<BitfinexAsset>>(default);
            return result.As(result.Data.First());
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<string>>> GetSymbolsAsync(CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<IEnumerable<string>>(_baseClient.GetUrl(SymbolsEndpoint, "1"), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexSymbolOverview>> GetTickerAsync(string symbol, CancellationToken ct = default)
        { 
            var ticker = await GetTickersAsync(new[] { symbol }, ct).ConfigureAwait(false);
            if(!ticker)
                return ticker.As<BitfinexSymbolOverview>(null);

            if (!ticker.Data.Any())
                return ticker.AsError<BitfinexSymbolOverview>(new ServerError("Symbol not found"));

            return ticker.As(ticker.Data.First());
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexSymbolOverview>>> GetTickersAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                {"symbols", symbols?.Any() == true ? string.Join(",", symbols): "ALL"}
            };

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexSymbolOverview>>(_baseClient.GetUrl(TickersEndpoint, "2"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexTradeSimple>>> GetTradeHistoryAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 5000);
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexTradeSimple>>(_baseClient.GetUrl(TradesEndpoint.FillPathParameters(symbol), "2"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexOrderBook>> GetOrderBookAsync(string symbol, Precision precision, int? limit = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            limit?.ValidateIntValues("limit", 25, 100);
            if (precision == Precision.R0)
                throw new ArgumentException("Precision can not be R0. Use PrecisionLevel0 to get aggregated trades for each price point or GetRawOrderBook to get the raw order book instead");

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("len", limit?.ToString(CultureInfo.InvariantCulture));
            var prec = JsonConvert.SerializeObject(precision, new PrecisionConverter(false));

            var result = await _baseClient.SendRequestAsync<IEnumerable<BitfinexOrderBookEntry>>(_baseClient.GetUrl(OrderBookEndpoint.FillPathParameters(symbol, prec), "2"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
            if (!result)
                return result.As<BitfinexOrderBook>(default);

            var isFunding = symbol.StartsWith("f");
            var book = new BitfinexOrderBook()
            {
                Asks = result.Data.Where(d => isFunding ? d.Quantity < 0 : d.Quantity > 0),
                Bids = result.Data.Where(d => isFunding ? d.Quantity > 0 : d.Quantity < 0)
            };
            return result.As(book);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexOrderBook>> GetRawOrderBookAsync(string symbol, int? limit = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            limit?.ValidateIntValues("limit", 25, 100);

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("len", limit?.ToString(CultureInfo.InvariantCulture));
            var prec = JsonConvert.SerializeObject(Precision.R0, new PrecisionConverter(false));

            var result = await _baseClient.SendRequestAsync<IEnumerable<BitfinexRawOrderBookEntry>>(_baseClient.GetUrl(OrderBookEndpoint.FillPathParameters(symbol, prec), "2"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
            if (!result)
                return result.As<BitfinexOrderBook>(default);

            var isFunding = symbol.StartsWith("f");
            var book = new BitfinexOrderBook()
            {
                Asks = result.Data.Where(d => isFunding ? d.Quantity < 0 : d.Quantity > 0),
                Bids = result.Data.Where(d => isFunding ? d.Quantity > 0 : d.Quantity < 0)
            };
            return result.As(book);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexKline>> GetLastKlineAsync(string symbol, KlineInterval interval, string? fundingPeriod = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();


            string endpoint;
            if (fundingPeriod != null)
            {
                endpoint = LastFundingCandleEndpoint.FillPathParameters(
                    JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false)),
                    symbol,
                    fundingPeriod);
            }
            else
            {
                endpoint = LastCandleEndpoint.FillPathParameters(
                    JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false)),
                    symbol);
            }

            return await _baseClient.SendRequestAsync<BitfinexKline>(_baseClient.GetUrl(endpoint, "2"), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexKline>>> GetKlinesAsync(string symbol, KlineInterval interval, string? fundingPeriod = null, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 5000);

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            string endpoint;
            if (fundingPeriod != null)
            {
                endpoint = FundingCandlesEndpoint.FillPathParameters(
                    JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false)),
                    symbol,
                    fundingPeriod);
            }
            else
            {
                endpoint = CandlesEndpoint.FillPathParameters(
                    JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false)),
                    symbol);
            }

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexKline>>(_baseClient.GetUrl(endpoint, "2"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexAveragePrice>> GetAveragePriceAsync(string symbol, decimal quantity, decimal? rateLimit = null, int? period = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("period", period?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("rate_limit", rateLimit?.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestAsync<BitfinexAveragePrice>(_baseClient.GetUrl(MarketAverageEndpoint, "2"), HttpMethod.Post, ct, parameters).ConfigureAwait(false);
        }


        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexFundingBook>> GetFundingBookAsync(string asset, int? limit = null, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit_bids", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("limit_asks", limit?.ToString(CultureInfo.InvariantCulture));
            return await _baseClient.SendRequestAsync<BitfinexFundingBook>(_baseClient.GetUrl(FundingBookEndpoint.FillPathParameters(asset), "1"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }


        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexSymbolDetails>>> GetSymbolDetailsAsync(CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexSymbolDetails>>(_baseClient.GetUrl(SymbolDetailsEndpoint, "1"), HttpMethod.Get, ct).ConfigureAwait(false);
        }


        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexForeignExchangeRate>> GetForeignExchangeRateAsync(string asset1, string asset2, CancellationToken ct = default)
        {
            asset1.ValidateNotNull(nameof(asset1));
            asset2.ValidateNotNull(nameof(asset2));

            var parameters = new Dictionary<string, object>
            {
                { "ccy1", asset1 },
                { "ccy2", asset2 }
            };

            return await _baseClient.SendRequestAsync<BitfinexForeignExchangeRate>(_baseClient.GetUrl(ForeignExchangeEndpoint, "2"), HttpMethod.Post, ct, parameters).ConfigureAwait(false);
        }


        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexLend>>> GetLendsAsync(string asset, DateTime? startTime = null, int? limit = null, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit_lends", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("timestamp", DateTimeConverter.ConvertToSeconds(startTime));
            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexLend>>(_baseClient.GetUrl(LendsEndpoint.FillPathParameters(asset), "1"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }


        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexStats>>> GetStatsAsync(string symbol, StatKey key, StatSide side, StatSection section, Sorting? sorting = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            var endpoint = StatsEndpoint.FillPathParameters(
                JsonConvert.SerializeObject(key, new StatKeyConverter(false)),
                symbol,
                JsonConvert.SerializeObject(side, new StatSideConverter(false)),
                JsonConvert.SerializeObject(section, new StatSectionConverter(false)));

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexStats>>(_baseClient.GetUrl(endpoint, "2"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }
    }
}
