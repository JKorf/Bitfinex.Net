using Bitfinex.Net.Enums;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.RateLimiting.Guards;
using CryptoExchange.Net.Objects.Errors;

namespace Bitfinex.Net.Clients.SpotApi
{
    /// <inheritdoc />
    internal class BitfinexRestClientSpotApiExchangeData : IBitfinexRestClientSpotApiExchangeData
    {
        private readonly BitfinexRestClientSpotApi _baseClient;
        private static readonly RequestDefinitionCache _definitions = new();

        internal BitfinexRestClientSpotApiExchangeData(BitfinexRestClientSpotApi baseClient)
        {
            _baseClient = baseClient;
        }

        #region Get Platform Status
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexPlatformStatus>> GetPlatformStatusAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, "/v2/platform/status", BitfinexExchange.RateLimiter.Overall, 1, false,
                limitGuard: new SingleLimitGuard(30, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexPlatformStatus>(request, null, ct).ConfigureAwait(false);
        }
        #endregion

        #region Conf endpoints

        #region Get Assets List
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexAsset[]>> GetAssetsListAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, "/v2/conf/pub:map:currency:label", BitfinexExchange.RateLimiter.RestConf, 1);
            var result = await _baseClient.SendAsync<BitfinexAsset[][]>(request, null, ct).ConfigureAwait(false);
            if (!result)
                return result.As<BitfinexAsset[]>(default);
            return result.As(result.Data.First());
        }
        #endregion

        #region Get Symbol Names
        /// <inheritdoc />
        public async Task<WebCallResult<string[]>> GetSymbolNamesAsync(SymbolType type, CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/conf/pub:list:pair:{EnumConverter.GetString(type)}", BitfinexExchange.RateLimiter.RestConf, 1);
            var result = await _baseClient.SendAsync<string[][]>(request, null, ct).ConfigureAwait(false);
            if (!result)
                return result.As<string[]>(default);
            return result.As(result.Data.First());
        }
        #endregion

        #region Get Asset Names
        /// <inheritdoc />
        public async Task<WebCallResult<string[]>> GetAssetNamesAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/conf/pub:list:currency", BitfinexExchange.RateLimiter.RestConf, 1);
            var result = await _baseClient.SendAsync<string[][]>(request, null, ct).ConfigureAwait(false);
            if (!result)
                return result.As<string[]>(default);
            return result.As(result.Data.First());
        }
        #endregion

        #region Get Asset Symbols
        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, string>>> GetAssetSymbolsAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/conf/pub:map:currency:sym", BitfinexExchange.RateLimiter.RestConf, 1);
            var result = await _baseClient.SendAsync<BitfinexStringKeyValue[][]>(request, null, ct).ConfigureAwait(false);
            if (!result)
                return result.As<Dictionary<string, string>>(default);
            return result.As(result.Data.First().ToDictionary(k => k.Key, k => k.Value));
        }
        #endregion

        #region Get Asset Full Names
        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, string>>> GetAssetFullNamesAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/conf/pub:map:currency:label", BitfinexExchange.RateLimiter.RestConf, 1);
            var result = await _baseClient.SendAsync<BitfinexStringKeyValue[][]>(request, null, ct).ConfigureAwait(false);
            if (!result)
                return result.As<Dictionary<string, string>>(default);
            return result.As(result.Data.First().ToDictionary(k => k.Key, k => k.Value));
        }
        #endregion

        #region Get Asset Units
        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, string>>> GetAssetUnitsAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/conf/pub:map:currency:unit", BitfinexExchange.RateLimiter.RestConf, 1);
            var result = await _baseClient.SendAsync<BitfinexStringKeyValue[][]>(request, null, ct).ConfigureAwait(false);
            if (!result)
                return result.As<Dictionary<string, string>>(default);
            return result.As(result.Data.First().ToDictionary(k => k.Key, k => k.Value));
        }
        #endregion

        #region Get Asset Underlyings
        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, string>>> GetAssetUnderlyingsAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/conf/pub:map:currency:undl", BitfinexExchange.RateLimiter.RestConf, 1);
            var result = await _baseClient.SendAsync<BitfinexStringKeyValue[][]>(request, null, ct).ConfigureAwait(false);
            if (!result)
                return result.As<Dictionary<string, string>>(default);
            return result.As(result.Data.First().ToDictionary(k => k.Key, k => k.Value));
        }
        #endregion

        #region Get Asset Networks
        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, string>>> GetAssetNetworksAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/conf/pub:map:currency:pool", BitfinexExchange.RateLimiter.RestConf, 1);
            var result = await _baseClient.SendAsync<BitfinexStringKeyValue[][]>(request, null, ct).ConfigureAwait(false);
            if (!result)
                return result.As<Dictionary<string, string>>(default);
            return result.As(result.Data.First().ToDictionary(k => k.Key, k => k.Value));
        }
        #endregion

        #region Get Asset Block Explorer Urls
        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, string[]>>> GetAssetBlockExplorerUrlsAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/conf/pub:map:currency:explorer", BitfinexExchange.RateLimiter.RestConf, 1);
            var result = await _baseClient.SendAsync<BitfinexStringArrayKeyValue[][]>(request, null, ct).ConfigureAwait(false);
            if (!result)
                return result.As<Dictionary<string, string[]>>(default);
            return result.As(result.Data.First().ToDictionary(k => k.Key, k => k.Value));
        }
        #endregion

        #region Get Asset Withdrawal Fees
        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, decimal[]>>> GetAssetWithdrawalFeesAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/conf/pub:map:currency:tx:fee", BitfinexExchange.RateLimiter.RestConf, 1);
            var result = await _baseClient.SendAsync<BitfinexDecimalArrayKeyValue[][]>(request, null, ct).ConfigureAwait(false);
            if (!result)
                return result.As< Dictionary<string, decimal[]>>(default);
            return result.As(result.Data.First().ToDictionary(k => k.Key, k => k.Value));
        }
        #endregion

        #region Get Asset Deposit Withdrawal Methods
        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, string[]>>> GetAssetDepositWithdrawalMethodsAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/conf/pub:map:tx:method", BitfinexExchange.RateLimiter.RestConf, 1);
            var result = await _baseClient.SendAsync<BitfinexStringArrayKeyValue[][]>(request, null, ct).ConfigureAwait(false);
            if (!result)
                return result.As<Dictionary<string, string[]>>(default);
            return result.As(result.Data.First().ToDictionary(k => k.Key, k => k.Value));
        }
        #endregion

        #region Get Symbols
        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, BitfinexSymbolInfo>>> GetSymbolsAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/conf/pub:info:pair", BitfinexExchange.RateLimiter.RestConf, 1);
            var result = await _baseClient.SendAsync<BitfinexSymbolInfoKeyValue[][]>(request, null, ct).ConfigureAwait(false);
            if (!result)
                return result.As<Dictionary<string, BitfinexSymbolInfo>>(default);
            return result.As(result.Data.First().ToDictionary(k => k.Key, k => k.Value));
        }
        #endregion

        #region Get Futures Symbols
        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, BitfinexSymbolInfo>>> GetFuturesSymbolsAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/conf/pub:info:pair:futures", BitfinexExchange.RateLimiter.RestConf, 1);
            var result = await _baseClient.SendAsync<BitfinexSymbolInfoKeyValue[][]>(request, null, ct).ConfigureAwait(false);
            if (!result)
                return result.As<Dictionary<string, BitfinexSymbolInfo>>(default);
            return result.As(result.Data.First().ToDictionary(k => k.Key, k => k.Value));
        }
        #endregion

        #region Get Deposit Withdrawal Status
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexAssetInfo[]>> GetDepositWithdrawalStatusAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/conf/pub:info:tx:status", BitfinexExchange.RateLimiter.RestConf, 1);
            var result = await _baseClient.SendAsync<BitfinexAssetInfo[][]>(request, null, ct).ConfigureAwait(false);
            if (!result)
                return result.As<BitfinexAssetInfo[]>(default);
            return result.As(result.Data.First());
        }
        #endregion

        #region Get Margin Info
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexMarginInfo>> GetMarginInfoAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/conf/pub:spec:margin", BitfinexExchange.RateLimiter.RestConf, 1);
            var result = await _baseClient.SendAsync<BitfinexMarginInfo[]>(request, null, ct).ConfigureAwait(false);
            if (!result)
                return result.As<BitfinexMarginInfo>(default);
            return result.As(result.Data.First());
        }
        #endregion

        #region Get Derivatives Fees
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexDerivativesFees>> GetDerivativesFeesAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/conf/pub:fees", BitfinexExchange.RateLimiter.RestConf, 1);
            var result = await _baseClient.SendAsync<BitfinexDerivativesFees[]>(request, null, ct).ConfigureAwait(false);
            if (!result)
                return result.As<BitfinexDerivativesFees>(default);
            return result.As(result.Data.First());
        }
        #endregion

        #endregion

        #region Get Ticker
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexTicker>> GetTickerAsync(string symbol, CancellationToken ct = default)
        {
            var ticker = await GetTickersAsync(new[] { symbol }, ct).ConfigureAwait(false);
            if(!ticker)
                return ticker.As<BitfinexTicker>(null);

            if (!ticker.Data.Any())
                return ticker.AsError<BitfinexTicker>(new ServerError(new ErrorInfo(ErrorType.UnknownSymbol, "Symbol not found")));

            return ticker.As(ticker.Data.First());
        }
        #endregion

        #region Get Funding Tickers
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexFundingTicker>> GetFundingTickerAsync(string symbol, CancellationToken ct = default)
        {
            var ticker = await GetFundingTickersAsync(new[] { symbol }, ct).ConfigureAwait(false);
            if (!ticker)
                return ticker.As<BitfinexFundingTicker>(null);

            if (!ticker.Data.Any())
                return ticker.AsError<BitfinexFundingTicker>(new ServerError(new ErrorInfo(ErrorType.UnknownSymbol, "Symbol not found")));

            return ticker.As(ticker.Data.First());
        }
        #endregion

        #region Get Tickers
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexTicker[]>> GetTickersAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection
            {
                {"symbols", symbols?.Any() == true ? string.Join(",", symbols): "ALL"}
            };

            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/tickers", BitfinexExchange.RateLimiter.Overall, 1, false,
                new SingleLimitGuard(30, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexTicker[]>(request, parameters, ct).ConfigureAwait(false);
        }
        #endregion

        #region Get Funding Tickers
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexFundingTicker[]>> GetFundingTickersAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection
            {
                {"symbols", symbols?.Any() == true ? string.Join(",", symbols): "ALL"}
            };

            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/tickers", BitfinexExchange.RateLimiter.Overall, 1, false,
                new SingleLimitGuard(30, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexFundingTicker[]>(request, parameters, ct).ConfigureAwait(false);
        }
        #endregion

        #region Get Ticker History
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexTickerHistory[]>> GetTickerHistoryAsync(IEnumerable<string>? symbols = null, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection
            {
                {"symbols", symbols?.Any() == true ? string.Join(",", symbols): "ALL"}
            };
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/tickers/hist", BitfinexExchange.RateLimiter.Overall, 1);
            return await _baseClient.SendAsync<BitfinexTickerHistory[]>(request, parameters, ct).ConfigureAwait(false);
        }
        #endregion

        #region Get Trade History
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexTradeSimple[]>> GetTradeHistoryAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 10000);
            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalEnum("sort", sorting);

            var request = _definitions.GetOrCreate(HttpMethod.Get, $"v2/trades/{symbol}/hist", BitfinexExchange.RateLimiter.Overall, 1, false,
                new SingleLimitGuard(15, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexTradeSimple[]>(request, parameters, ct).ConfigureAwait(false);
        }
        #endregion

        #region Get Order Book
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexOrderBook>> GetOrderBookAsync(string symbol, Precision precision, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntValues("limit", 1, 25, 100);
            if (precision == Precision.R0)
                throw new ArgumentException("Precision can not be R0. Use PrecisionLevel0 to get aggregated trades for each price point or GetRawOrderBook to get the raw order book instead");

            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("len", limit?.ToString(CultureInfo.InvariantCulture));

            var request = _definitions.GetOrCreate(HttpMethod.Get, $"v2/book/{symbol}/{EnumConverter.GetString(precision)}", BitfinexExchange.RateLimiter.Overall, 1, false,
                new SingleLimitGuard(240, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<BitfinexOrderBookEntry[]>(request, parameters, ct).ConfigureAwait(false);
            if (!result)
                return result.As<BitfinexOrderBook>(default);

            var book = new BitfinexOrderBook()
            {
                Asks = result.Data.Where(d => d.Quantity < 0).ToArray(),
                Bids = result.Data.Where(d => d.Quantity > 0).ToArray()
            };

            foreach (var item in book.Asks)
                item.Quantity = Math.Abs(item.Quantity);

            return result.As(book);
        }
        #endregion

        #region Get Funding Order Book
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexFundingOrderBook>> GetFundingOrderBookAsync(string symbol, Precision precision, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntValues("limit", 1, 25, 100);
            if (precision == Precision.R0)
                throw new ArgumentException("Precision can not be R0. Use PrecisionLevel0 to get aggregated trades for each price point or GetRawOrderBook to get the raw order book instead");

            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("len", limit?.ToString(CultureInfo.InvariantCulture));
            
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"v2/book/{symbol}/{EnumConverter.GetString(precision)}", BitfinexExchange.RateLimiter.Overall, 1, false,
                new SingleLimitGuard(240, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<BitfinexOrderBookFundingEntry[]>(request, parameters, ct).ConfigureAwait(false);
            if (!result)
                return result.As<BitfinexFundingOrderBook>(default);

            var book = new BitfinexFundingOrderBook()
            {
                Asks = result.Data.Where(d => d.Quantity > 0).ToArray(),
                Bids = result.Data.Where(d => d.Quantity < 0).ToArray()
            };

            foreach (var item in book.Bids)
                item.Quantity = Math.Abs(item.Quantity);

            return result.As(book);
        }
        #endregion

        #region Get Raw Order Book
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexRawOrderBook>> GetRawOrderBookAsync(string symbol, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntValues("limit", 25, 100);

            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("len", limit?.ToString(CultureInfo.InvariantCulture));

            var request = _definitions.GetOrCreate(HttpMethod.Get, $"v2/book/{symbol}/{EnumConverter.GetString(Precision.R0)}", BitfinexExchange.RateLimiter.Overall, 1, false,
                new SingleLimitGuard(240, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<BitfinexRawOrderBookEntry[]>(request, parameters, ct).ConfigureAwait(false);
            if (!result)
                return result.As<BitfinexRawOrderBook>(default);

            var book = new BitfinexRawOrderBook()
            {
                Asks = result.Data.Where(d => d.Quantity < 0).ToArray(),
                Bids = result.Data.Where(d => d.Quantity > 0).ToArray()
            };

            foreach (var item in book.Asks)
                item.Quantity = Math.Abs(item.Quantity);

            return result.As(book);
        }
        #endregion

        #region Get Raw Funding Order Book
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexRawFundingOrderBook>> GetRawFundingOrderBookAsync(string symbol, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntValues("limit", 25, 100);

            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("len", limit?.ToString(CultureInfo.InvariantCulture));

            var request = _definitions.GetOrCreate(HttpMethod.Get, $"v2/book/{symbol}/{EnumConverter.GetString(Precision.R0)}", BitfinexExchange.RateLimiter.Overall, 1, false,
                new SingleLimitGuard(240, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<BitfinexRawOrderBookFundingEntry[]>(request, parameters, ct).ConfigureAwait(false);
            if (!result)
                return result.As<BitfinexRawFundingOrderBook>(default);

            var book = new BitfinexRawFundingOrderBook()
            {
                Asks = result.Data.Where(d => d.Quantity > 0).ToArray(),
                Bids = result.Data.Where(d => d.Quantity < 0).ToArray()
            };

            foreach(var item in book.Bids)
                item.Quantity = Math.Abs(item.Quantity);

            return result.As(book);
        }
        #endregion

        #region Get Last Kline
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexKline>> GetLastKlineAsync(string symbol, KlineInterval interval, string? fundingPeriod = null, CancellationToken ct = default)
        {
            string endpoint;
            if (fundingPeriod != null)
            {
                endpoint = $"v2/candles/trade:{EnumConverter.GetString(interval)}:{symbol}:{fundingPeriod}/last";
            }
            else
            {
                endpoint = $"v2/candles/trade:{EnumConverter.GetString(interval)}:{symbol}/last";
            }

            var request = _definitions.GetOrCreate(HttpMethod.Get, endpoint, BitfinexExchange.RateLimiter.Overall, 1, false,
                new SingleLimitGuard(30, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexKline>(request, null, ct).ConfigureAwait(false);
        }
        #endregion

        #region Get Klines
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexKline[]>> GetKlinesAsync(string symbol, KlineInterval interval, string? fundingPeriod = null, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 10000);

            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalEnum("sort", sorting); 

            string endpoint;
            if (fundingPeriod != null)
            {
                endpoint = $"v2/candles/trade:{EnumConverter.GetString(interval)}:{symbol}:{fundingPeriod}/hist";
            }
            else
            {
                endpoint = $"v2/candles/trade:{EnumConverter.GetString(interval)}:{symbol}/hist";
            }

            var request = _definitions.GetOrCreate(HttpMethod.Get, endpoint, BitfinexExchange.RateLimiter.Overall, 1, false,
                new SingleLimitGuard(30, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexKline[]>(request, parameters, ct).ConfigureAwait(false);
        }
        #endregion

        #region Get Average Price
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexAveragePrice>> GetAveragePriceAsync(string symbol, decimal quantity, decimal? rateLimit = null, int? period = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection
            {
                { "symbol", symbol },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("period", period?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("rate_limit", rateLimit?.ToString(CultureInfo.InvariantCulture));

            var request = _definitions.GetOrCreate(HttpMethod.Post, "/v2/calc/trade/avg", BitfinexExchange.RateLimiter.Overall, 1, false,
                new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexAveragePrice>(request, parameters, ct).ConfigureAwait(false);
        }
        #endregion

        #region Get Foreign Exchange Rate
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexForeignExchangeRate>> GetForeignExchangeRateAsync(string asset1, string asset2, CancellationToken ct = default)
        {
            asset1.ValidateNotNull(nameof(asset1));
            asset2.ValidateNotNull(nameof(asset2));

            var parameters = new ParameterCollection
            {
                { "ccy1", asset1 },
                { "ccy2", asset2 }
            };

            var request = _definitions.GetOrCreate(HttpMethod.Post, "/v2/calc/fx", BitfinexExchange.RateLimiter.Overall, 1, false,
                new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexForeignExchangeRate>(request, parameters, ct).ConfigureAwait(false);
        }
        #endregion

        #region Get Derivatives Status
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexDerivativesStatus[]>> GetDerivativesStatusAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection
            {
                {"keys", symbols?.Any() == true ? string.Join(",", symbols): "ALL"}
            };

            var request = _definitions.GetOrCreate(HttpMethod.Get, "/v2/status/deriv", BitfinexExchange.RateLimiter.Overall, 1, false,
                new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexDerivativesStatus[]>(request, parameters, ct).ConfigureAwait(false);
        }
        #endregion

        #region Get Derivatives Status History
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexDerivativesStatus[]>> GetDerivativesStatusHistoryAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalEnum("sort", sorting);

            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/status/deriv/{symbol}/hist", BitfinexExchange.RateLimiter.Overall, 1, false,
                new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexDerivativesStatus[]>(request, parameters, ct).ConfigureAwait(false);
        }
        #endregion

        #region Get Liquidations
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexLiquidation[]>> GetLiquidationsAsync(int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalEnum("sort", sorting);

            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/liquidations/hist", BitfinexExchange.RateLimiter.Overall, 1, false,
                new SingleLimitGuard(3, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            var result = await _baseClient.SendAsync<BitfinexLiquidation[][]>(request, parameters, ct).ConfigureAwait(false);
            if (!result)
                return result.As<BitfinexLiquidation[]>(default);

            return result.As(result.Data.SelectMany(d => d).ToArray());
        }
        #endregion

        #region Get Funding Statistics
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexFundingStats[]>> GetFundingStatisticsAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/funding/stats/{symbol}/hist", BitfinexExchange.RateLimiter.Overall, 1);
            return await _baseClient.SendAsync<BitfinexFundingStats[]>(request, parameters, ct).ConfigureAwait(false);
        }
        #endregion

        #region Get Funding Size History
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexStats[]>> GetFundingSizeHistoryAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalEnum("sort", sorting);

            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/stats1/funding.size:1m:{symbol}/{EnumConverter.GetString(StatSection.History)}", BitfinexExchange.RateLimiter.RestStats, 1);
            return await _baseClient.SendAsync<BitfinexStats[]>(request, parameters, ct).ConfigureAwait(false);
        }
        #endregion

        #region Get Last Funding Size
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexStats>> GetLastFundingSizeAsync(string symbol, CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/stats1/funding.size:1m:{symbol}/{EnumConverter.GetString(StatSection.Last)}", BitfinexExchange.RateLimiter.RestStats, 1);
            return await _baseClient.SendAsync<BitfinexStats>(request, null, ct).ConfigureAwait(false);
        }
        #endregion

        #region Get Last Credit Size
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexStats>> GetLastCreditSizeAsync(string symbol, CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/stats1/credits.size:1m:{symbol}/{EnumConverter.GetString(StatSection.Last)}", BitfinexExchange.RateLimiter.RestStats, 1);
            return await _baseClient.SendAsync<BitfinexStats>(request, null, ct).ConfigureAwait(false);
        }
        #endregion

        #region Get Credit Size History
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexStats[]>> GetCreditSizeHistoryAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalEnum("sort", sorting);

            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/stats1/credits.size:1m:{symbol}/{EnumConverter.GetString(StatSection.History)}", BitfinexExchange.RateLimiter.RestStats, 1);
            return await _baseClient.SendAsync<BitfinexStats[]>(request, parameters, ct).ConfigureAwait(false);
        }
        #endregion

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexStats>> GetLastCreditSizeAsync(string asset, string symbol, CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/stats1/credits.size.sym:1m:{asset}:{symbol}/{EnumConverter.GetString(StatSection.Last)}", BitfinexExchange.RateLimiter.RestStats, 1);
            return await _baseClient.SendAsync<BitfinexStats>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexStats[]>> GetCreditSizeHistoryAsync(string asset, string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalEnum("sort", sorting);

            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/stats1/credits.size.sym:1m:{asset}:{symbol}/{EnumConverter.GetString(StatSection.History)}", BitfinexExchange.RateLimiter.RestStats, 1);
            return await _baseClient.SendAsync<BitfinexStats[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexStats>> GetLastLongsShortsTotalsAsync(string symbol, StatSide side, CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/stats1/pos.size:1m:{symbol}:{EnumConverter.GetString(side)}/{EnumConverter.GetString(StatSection.Last)}", BitfinexExchange.RateLimiter.RestStats, 1);
            return await _baseClient.SendAsync<BitfinexStats>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexStats[]>> GetLongsShortsTotalsHistoryAsync(string symbol, StatSide side, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalEnum("sort", sorting);

            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/stats1/pos.size:1m:{symbol}:{EnumConverter.GetString(side)}/{EnumConverter.GetString(StatSection.History)}", BitfinexExchange.RateLimiter.RestStats, 1);
            return await _baseClient.SendAsync<BitfinexStats[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexStats>> GetLastTradingVolumeAsync(int period, CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/stats1/vol.{period}d:30m:BFX/{EnumConverter.GetString(StatSection.Last)}", BitfinexExchange.RateLimiter.RestStats, 1);
            return await _baseClient.SendAsync<BitfinexStats>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexStats[]>> GetTradingVolumeHistoryAsync(int period, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalEnum("sort", sorting);

            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/stats1/vol.{period}d:30m:BFX/{EnumConverter.GetString(StatSection.History)}", BitfinexExchange.RateLimiter.RestStats, 1);
            return await _baseClient.SendAsync<BitfinexStats[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexStats>> GetLastVolumeWeightedAveragePriceAsync(string symbol, CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/stats1/vwap:1d:{symbol}/{EnumConverter.GetString(StatSection.Last)}", BitfinexExchange.RateLimiter.RestStats, 1);
            return await _baseClient.SendAsync<BitfinexStats>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexStats[]>> GetVolumeWeightedAveragePriceHistoryAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalEnum("sort", sorting);

            var request = _definitions.GetOrCreate(HttpMethod.Get, $"/v2/stats1/vwap:1d:{symbol}/{EnumConverter.GetString(StatSection.History)}", BitfinexExchange.RateLimiter.RestStats, 1);
            return await _baseClient.SendAsync<BitfinexStats[]>(request, parameters, ct).ConfigureAwait(false);
        }
    }
}
