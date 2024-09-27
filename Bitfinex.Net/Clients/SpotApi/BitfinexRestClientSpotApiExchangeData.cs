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
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using Bitfinex.Net.ExtensionMethods;

namespace Bitfinex.Net.Clients.SpotApi
{
    /// <inheritdoc />
    internal class BitfinexRestClientSpotApiExchangeData : IBitfinexRestClientSpotApiExchangeData
    {
        private readonly BitfinexRestClientSpotApi _baseClient;

        internal BitfinexRestClientSpotApiExchangeData(BitfinexRestClientSpotApi baseClient)
        {
            _baseClient = baseClient;
        }

        #region Get Platform Status
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexPlatformStatus>> GetPlatformStatusAsync(CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<BitfinexPlatformStatus>(_baseClient.GetUrl("platform/status", "2"), HttpMethod.Get, ct).ConfigureAwait(false);
        }
        #endregion

        #region Conf endpoints

        #region Get Assets List
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexAsset>>> GetAssetsListAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.SendRequestAsync<IEnumerable<IEnumerable<BitfinexAsset>>>(_baseClient.GetUrl("conf/pub:map:currency:label", "2"), HttpMethod.Get, ct).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<BitfinexAsset>>(default);
            return result.As(result.Data.First());
        }
        #endregion

        #region Get Symbol Names
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<string>>> GetSymbolNamesAsync(SymbolType type, CancellationToken ct = default)
        {
            var result = await _baseClient.SendRequestAsync<IEnumerable<IEnumerable<string>>>(_baseClient.GetUrl($"conf/pub:list:pair:{EnumConverter.GetString(type)}", "2"), HttpMethod.Get, ct).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<string>>(default);
            return result.As(result.Data.First());
        }
        #endregion

        #region Get Asset Names
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<string>>> GetAssetNamesAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.SendRequestAsync<IEnumerable<IEnumerable<string>>>(_baseClient.GetUrl($"conf/pub:list:currency", "2"), HttpMethod.Get, ct).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<string>>(default);
            return result.As(result.Data.First());
        }
        #endregion

        #region Get Asset Symbols
        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, string>>> GetAssetSymbolsAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.SendRequestAsync<IEnumerable<IEnumerable<BitfinexKeyValue<string>>>>(_baseClient.GetUrl($"conf/pub:map:currency:sym", "2"), HttpMethod.Get, ct).ConfigureAwait(false);
            if (!result)
                return result.As<Dictionary<string, string>>(default);
            return result.As(result.Data.First().ToDictionary(k => k.Key, k => k.Value));
        }
        #endregion

        #region Get Asset Full Names
        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, string>>> GetAssetFullNamesAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.SendRequestAsync<IEnumerable<IEnumerable<BitfinexKeyValue<string>>>>(_baseClient.GetUrl($"conf/pub:map:currency:label", "2"), HttpMethod.Get, ct).ConfigureAwait(false);
            if (!result)
                return result.As<Dictionary<string, string>>(default);
            return result.As(result.Data.First().ToDictionary(k => k.Key, k => k.Value));
        }
        #endregion

        #region Get Asset Units
        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, string>>> GetAssetUnitsAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.SendRequestAsync<IEnumerable<IEnumerable<BitfinexKeyValue<string>>>>(_baseClient.GetUrl($"conf/pub:map:currency:unit", "2"), HttpMethod.Get, ct).ConfigureAwait(false);
            if (!result)
                return result.As<Dictionary<string, string>>(default);
            return result.As(result.Data.First().ToDictionary(k => k.Key, k => k.Value));
        }
        #endregion

        #region Get Asset Underlyings
        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, string>>> GetAssetUnderlyingsAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.SendRequestAsync<IEnumerable<IEnumerable<BitfinexKeyValue<string>>>>(_baseClient.GetUrl($"conf/pub:map:currency:undl", "2"), HttpMethod.Get, ct).ConfigureAwait(false);
            if (!result)
                return result.As<Dictionary<string, string>>(default);
            return result.As(result.Data.First().ToDictionary(k => k.Key, k => k.Value));
        }
        #endregion

        #region Get Asset Networks
        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, string>>> GetAssetNetworksAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.SendRequestAsync<IEnumerable<IEnumerable<BitfinexKeyValue<string>>>>(_baseClient.GetUrl($"conf/pub:map:currency:pool", "2"), HttpMethod.Get, ct).ConfigureAwait(false);
            if (!result)
                return result.As<Dictionary<string, string>>(default);
            return result.As(result.Data.First().ToDictionary(k => k.Key, k => k.Value));
        }
        #endregion

        #region Get Asset Block Explorer Urls
        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, IEnumerable<string>>>> GetAssetBlockExplorerUrlsAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.SendRequestAsync<IEnumerable<IEnumerable<BitfinexKeyValue<IEnumerable<string>>>>>(_baseClient.GetUrl($"conf/pub:map:currency:explorer", "2"), HttpMethod.Get, ct).ConfigureAwait(false);
            if (!result)
                return result.As<Dictionary<string, IEnumerable<string>>>(default);
            return result.As(result.Data.First().ToDictionary(k => k.Key, k => k.Value));
        }
        #endregion

        #region Get Asset Withdrawal Fees
        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, IEnumerable<decimal>>>> GetAssetWithdrawalFeesAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.SendRequestAsync<IEnumerable<IEnumerable<BitfinexKeyValue<IEnumerable<decimal>>>>>(_baseClient.GetUrl($"conf/pub:map:currency:tx:fee", "2"), HttpMethod.Get, ct).ConfigureAwait(false);
            if (!result)
                return result.As< Dictionary<string, IEnumerable<decimal>>>(default);
            return result.As(result.Data.First().ToDictionary(k => k.Key, k => k.Value));
        }
        #endregion

        #region Get Asset Deposit Withdrawal Methods
        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, IEnumerable<string>>>> GetAssetDepositWithdrawalMethodsAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.SendRequestAsync<IEnumerable<IEnumerable<BitfinexKeyValue<IEnumerable<string>>>>>(_baseClient.GetUrl($"conf/pub:map:tx:method", "2"), HttpMethod.Get, ct).ConfigureAwait(false);
            if (!result)
                return result.As<Dictionary<string, IEnumerable<string>>>(default);
            return result.As(result.Data.First().ToDictionary(k => k.Key, k => k.Value));
        }
        #endregion

        #region Get Symbols
        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, BitfinexSymbolInfo>>> GetSymbolsAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.SendRequestAsync<IEnumerable<IEnumerable<BitfinexKeyValue<BitfinexSymbolInfo>>>>(_baseClient.GetUrl($"conf/pub:info:pair", "2"), HttpMethod.Get, ct).ConfigureAwait(false);
            if (!result)
                return result.As<Dictionary<string, BitfinexSymbolInfo>>(default);
            return result.As(result.Data.First().ToDictionary(k => k.Key, k => k.Value));
        }
        #endregion

        #region Get Futures Symbols
        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, BitfinexSymbolInfo>>> GetFuturesSymbolsAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.SendRequestAsync<IEnumerable<IEnumerable<BitfinexKeyValue<BitfinexSymbolInfo>>>>(_baseClient.GetUrl($"conf/pub:info:pair:futures", "2"), HttpMethod.Get, ct).ConfigureAwait(false);
            if (!result)
                return result.As<Dictionary<string, BitfinexSymbolInfo>>(default);
            return result.As(result.Data.First().ToDictionary(k => k.Key, k => k.Value));
        }
        #endregion

        #region Get Deposit Withdrawal Status
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexAssetInfo>>> GetDepositWithdrawalStatusAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.SendRequestAsync<IEnumerable<IEnumerable<BitfinexAssetInfo>>>(_baseClient.GetUrl($"conf/pub:info:tx:status", "2"), HttpMethod.Get, ct).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<BitfinexAssetInfo>>(default);
            return result.As(result.Data.First());
        }
        #endregion

        #region Get Margin Info
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexMarginInfo>> GetMarginInfoAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.SendRequestAsync<IEnumerable<BitfinexMarginInfo>> (_baseClient.GetUrl($"conf/pub:spec:margin", "2"), HttpMethod.Get, ct).ConfigureAwait(false);
            if (!result)
                return result.As<BitfinexMarginInfo>(default);
            return result.As(result.Data.First());
        }
        #endregion

        #region Get Derivatives Fees
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexDerivativesFees>> GetDerivativesFeesAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.SendRequestAsync<IEnumerable<BitfinexDerivativesFees>>(_baseClient.GetUrl($"conf/pub:fees", "2"), HttpMethod.Get, ct).ConfigureAwait(false);
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
                return ticker.AsError<BitfinexTicker>(new ServerError("Symbol not found"));

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
                return ticker.AsError<BitfinexFundingTicker>(new ServerError("Symbol not found"));

            return ticker.As(ticker.Data.First());
        }
        #endregion

        #region Get Tickers
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexTicker>>> GetTickersAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                {"symbols", symbols?.Any() == true ? string.Join(",", symbols): "ALL"}
            };

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexTicker>>(_baseClient.GetUrl("tickers", "2"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }
        #endregion

        #region Get Funding Tickers
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexFundingTicker>>> GetFundingTickersAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                {"symbols", symbols?.Any() == true ? string.Join(",", symbols): "ALL"}
            };

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexFundingTicker>>(_baseClient.GetUrl("tickers", "2"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }
        #endregion

        #region Get Ticker History
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexTickerHistory>>> GetTickerHistoryAsync(IEnumerable<string>? symbols = null, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                {"symbols", symbols?.Any() == true ? string.Join(",", symbols): "ALL"}
            };
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexTickerHistory>>(_baseClient.GetUrl("tickers/hist", "2"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }
        #endregion

        #region Get Trade History
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexTradeSimple>>> GetTradeHistoryAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 10000);
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexTradeSimple>>(_baseClient.GetUrl($"trades/{symbol}/hist", "2"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }
        #endregion

        #region Get Order Book
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexOrderBook>> GetOrderBookAsync(string symbol, Precision precision, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntValues("limit", 1, 25, 100);
            if (precision == Precision.R0)
                throw new ArgumentException("Precision can not be R0. Use PrecisionLevel0 to get aggregated trades for each price point or GetRawOrderBook to get the raw order book instead");

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("len", limit?.ToString(CultureInfo.InvariantCulture));
            var prec = JsonConvert.SerializeObject(precision, new PrecisionConverter(false));

            var result = await _baseClient.SendRequestAsync<IEnumerable<BitfinexOrderBookEntry>>(_baseClient.GetUrl($"book/{symbol}/{prec}", "2"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
            if (!result)
                return result.As<BitfinexOrderBook>(default);

            var book = new BitfinexOrderBook()
            {
                Asks = result.Data.Where(d => d.Quantity < 0).ToList(),
                Bids = result.Data.Where(d => d.Quantity > 0).ToList()
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

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("len", limit?.ToString(CultureInfo.InvariantCulture));
            var prec = JsonConvert.SerializeObject(precision, new PrecisionConverter(false));

            var result = await _baseClient.SendRequestAsync<IEnumerable<BitfinexOrderBookFundingEntry>>(_baseClient.GetUrl($"book/{symbol}/{prec}", "2"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
            if (!result)
                return result.As<BitfinexFundingOrderBook>(default);

            var book = new BitfinexFundingOrderBook()
            {
                Asks = result.Data.Where(d => d.Quantity > 0).ToList(),
                Bids = result.Data.Where(d => d.Quantity < 0).ToList()
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

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("len", limit?.ToString(CultureInfo.InvariantCulture));
            var prec = JsonConvert.SerializeObject(Precision.R0, new PrecisionConverter(false));

            var result = await _baseClient.SendRequestAsync<IEnumerable<BitfinexRawOrderBookEntry>>(_baseClient.GetUrl($"book/{symbol}/{prec}", "2"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
            if (!result)
                return result.As<BitfinexRawOrderBook>(default);

            var book = new BitfinexRawOrderBook()
            {
                Asks = result.Data.Where(d => d.Quantity < 0).ToList(),
                Bids = result.Data.Where(d => d.Quantity > 0).ToList()
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

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("len", limit?.ToString(CultureInfo.InvariantCulture));
            var prec = JsonConvert.SerializeObject(Precision.R0, new PrecisionConverter(false));

            var result = await _baseClient.SendRequestAsync<IEnumerable<BitfinexRawOrderBookFundingEntry>>(_baseClient.GetUrl($"book/{symbol}/{prec}", "2"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
            if (!result)
                return result.As<BitfinexRawFundingOrderBook>(default);

            var book = new BitfinexRawFundingOrderBook()
            {
                Asks = result.Data.Where(d => d.Quantity > 0).ToList(),
                Bids = result.Data.Where(d => d.Quantity < 0).ToList()
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
                endpoint = $"candles/trade:{JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false))}:{symbol}:{fundingPeriod}/last";
            }
            else
            {
                endpoint = $"candles/trade:{JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false))}:{symbol}/last";
            }

            return await _baseClient.SendRequestAsync<BitfinexKline>(_baseClient.GetUrl(endpoint, "2"), HttpMethod.Get, ct).ConfigureAwait(false);
        }
        #endregion

        #region Get Klines
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexKline>>> GetKlinesAsync(string symbol, KlineInterval interval, string? fundingPeriod = null, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 10000);

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            string endpoint;
            if (fundingPeriod != null)
            {
                endpoint = $"candles/trade:{JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false))}:{symbol}:{fundingPeriod}/hist";
            }
            else
            {
                endpoint = $"candles/trade:{JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false))}:{symbol}/hist";
            }

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexKline>>(_baseClient.GetUrl(endpoint, "2"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }
        #endregion

        #region Get Average Price
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexAveragePrice>> GetAveragePriceAsync(string symbol, decimal quantity, decimal? rateLimit = null, int? period = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("period", period?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("rate_limit", rateLimit?.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestAsync<BitfinexAveragePrice>(_baseClient.GetUrl("calc/trade/avg", "2"), HttpMethod.Post, ct, parameters).ConfigureAwait(false);
        }
        #endregion

        #region Get Foreign Exchange Rate
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

            return await _baseClient.SendRequestAsync<BitfinexForeignExchangeRate>(_baseClient.GetUrl("calc/fx", "2"), HttpMethod.Post, ct, parameters).ConfigureAwait(false);
        }
        #endregion

        #region Get Derivatives Status
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexDerivativesStatus>>> GetDerivativesStatusAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                {"keys", symbols?.Any() == true ? string.Join(",", symbols): "ALL"}
            };

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexDerivativesStatus>>(_baseClient.GetUrl("status/deriv", "2"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }
        #endregion

        #region Get Derivatives Status History
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexDerivativesStatus>>> GetDerivativesStatusHistoryAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexDerivativesStatus>>(_baseClient.GetUrl($"status/deriv/{symbol}/hist", "2"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }
        #endregion

        #region Get Liquidations
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexLiquidation>>> GetLiquidationsAsync(int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            var result = await _baseClient.SendRequestAsync<IEnumerable<IEnumerable<BitfinexLiquidation>>>(_baseClient.GetUrl($"liquidations/hist", "2"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<BitfinexLiquidation>>(default);

            return result.As(result.Data.SelectMany(d => d));
        }
        #endregion

        #region Get Funding Statistics
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexFundingStats>>> GetFundingStatisticsAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexFundingStats>>(_baseClient.GetUrl($"funding/stats/{symbol}/hist", "2"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }
        #endregion

        #region Get Funding Size History
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexStats>>> GetFundingSizeHistoryAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            var endpoint = _baseClient.GetUrl($"stats1/funding.size:1m:{symbol}/{EnumConverter.GetString(StatSection.History)}", "2");
            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexStats>>(endpoint, HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }
        #endregion

        #region Get Last Funding Size
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexStats>> GetLastFundingSizeAsync(string symbol, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            var endpoint = _baseClient.GetUrl($"stats1/funding.size:1m:{symbol}/{EnumConverter.GetString(StatSection.Last)}", "2");
            return await _baseClient.SendRequestAsync<BitfinexStats>(endpoint, HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }
        #endregion

        #region Get Last Credit Size
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexStats>> GetLastCreditSizeAsync(string symbol, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            var endpoint = _baseClient.GetUrl($"stats1/credits.size:1m:{symbol}/{EnumConverter.GetString(StatSection.Last)}", "2");
            return await _baseClient.SendRequestAsync<BitfinexStats>(endpoint, HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }
        #endregion

        #region Get Credit Size History
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexStats>>> GetCreditSizeHistoryAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            var endpoint = _baseClient.GetUrl($"stats1/credits.size:1m:{symbol}/{EnumConverter.GetString(StatSection.History)}", "2");
            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexStats>>(endpoint, HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }
        #endregion

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexStats>> GetLastCreditSizeAsync(string asset, string symbol, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            var endpoint = _baseClient.GetUrl($"stats1/credits.size.sym:1m:{asset}:{symbol}/{EnumConverter.GetString(StatSection.Last)}", "2");
            return await _baseClient.SendRequestAsync<BitfinexStats>(endpoint, HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexStats>>> GetCreditSizeHistoryAsync(string asset, string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            var endpoint = _baseClient.GetUrl($"stats1/credits.size.sym:1m:{asset}:{symbol}/{EnumConverter.GetString(StatSection.History)}", "2");
            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexStats>>(endpoint, HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexStats>> GetLastLongsShortsTotalsAsync(string symbol, StatSide side, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            var endpoint = _baseClient.GetUrl($"stats1/pos.size:1m:{symbol}:{EnumConverter.GetString(side)}/{EnumConverter.GetString(StatSection.Last)}", "2");
            return await _baseClient.SendRequestAsync<BitfinexStats>(endpoint, HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexStats>>> GetLongsShortsTotalsHistoryAsync(string symbol, StatSide side, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            var endpoint = _baseClient.GetUrl($"stats1/pos.size:1m:{symbol}:{EnumConverter.GetString(side)}/{EnumConverter.GetString(StatSection.History)}", "2");
            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexStats>>(endpoint, HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexStats>> GetLastTradingVolumeAsync(int period, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            var endpoint = _baseClient.GetUrl($"stats1/vol.{period}d:30m:BFX/{EnumConverter.GetString(StatSection.Last)}", "2");
            return await _baseClient.SendRequestAsync<BitfinexStats>(endpoint, HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexStats>>> GetTradingVolumeHistoryAsync(int period, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            var endpoint = _baseClient.GetUrl($"stats1/vol.{period}d:30m:BFX/{EnumConverter.GetString(StatSection.History)}", "2");
            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexStats>>(endpoint, HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexStats>> GetLastVolumeWeightedAveragePriceAsync(string symbol, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            var endpoint = _baseClient.GetUrl($"stats1/vwap:1d:{symbol}/{EnumConverter.GetString(StatSection.Last)}", "2");
            return await _baseClient.SendRequestAsync<BitfinexStats>(endpoint, HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexStats>>> GetVolumeWeightedAveragePriceHistoryAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            var endpoint = _baseClient.GetUrl($"stats1/vwap:1d:{symbol}/{EnumConverter.GetString(StatSection.History)}", "2");
            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexStats>>(endpoint, HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }
    }
}
