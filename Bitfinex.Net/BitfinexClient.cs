using Bitfinex.Net.Converters;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.RestV1Objects;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Bitfinex.Net.Interfaces;

namespace Bitfinex.Net
{
    public class BitfinexClient: RestClient, IBitfinexClient
    {
        #region fields

        private static BitfinexClientOptions defaultOptions = new BitfinexClientOptions();
        private static BitfinexClientOptions DefaultOptions => defaultOptions.Copy<BitfinexClientOptions>();

        private const string ApiVersion1 = "1";
        private const string ApiVersion2 = "2";

        private const string StatusEndpoint = "platform/status";
        private const string FundingBookEndpoint = "lendbook/{}";
        private const string LendsEndpoint = "lends/{}";
        private const string SymbolsEndpoint = "symbols";
        private const string SymbolDetailsEndpoint = "symbols_details";
        private const string CurrenciesEndpoint = "conf/pub:map:currency:label";
        private const string TickersEndpoint = "tickers";
        private const string TradesEndpoint = "trades/{}/hist";
        private const string OrderBookEndpoint = "book/{}/{}";
        private const string StatsEndpoint = "stats1/{}:1m:{}:{}/{}";
        private const string LastCandleEndpoint = "candles/trade:{}:{}/last";
        private const string CandlesEndpoint = "candles/trade:{}:{}/hist";
        private const string MarketAverageEndpoint = "calc/trade/avg";
        private const string ForeignExchangeEndpoint = "calc/fx";

        private const string WalletsEndpoint = "auth/r/wallets";
        private const string CalcAvailableBalanceEndpoint = "auth/calc/order/avail";
        private const string OpenOrdersEndpoint = "auth/r/orders";
        private const string OrderHistoryEndpoint = "auth/r/orders/{}/hist";
        private const string OrderTradesEndpoint = "auth/r/order/{}:{}/trades";
        private const string MyTradesEndpoint = "auth/r/trades/{}/hist";
        private const string UserInfoEndpoint = "auth/r/info/user";
        private const string LedgerEntriesEndpoint = "auth/r/ledgers/{}/hist";

        private const string ActivePositionsEndpoint = "auth/r/positions";
        private const string PositionHistoryEndpoint = "auth/r/positions/hist";
        private const string PositionAuditEndpoint = "auth/r/positions/audit";
        private const string ActiveFundingOffersEndpoint = "auth/r/funding/offers/{}";
        private const string FundingOfferHistoryEndpoint = "auth/r/funding/offers/{}/hist";
        private const string FundingLoansEndpoint = "auth/r/funding/loans/{}";
        private const string FundingLoansHistoryEndpoint = "auth/r/funding/loans/{}/hist";
        private const string FundingCreditsEndpoint = "auth/r/funding/credits/{}";
        private const string FundingCreditsHistoryEndpoint = "auth/r/funding/credits/{}/hist";
        private const string FundingTradesEndpoint = "auth/r/funding/trades/{}/hist";
        private const string MarginInfoBaseEndpoint = "auth/r/info/margin/base";
        private const string MarginInfoSymbolEndpoint = "auth/r/info/margin/{}";
        private const string FundingInfoEndpoint = "auth/r/info/funding/{}";

        private const string MovementsEndpoint = "auth/r/movements/{}/hist";
        private const string DailyPerformanceEndpoint = "auth/r/stats/perf:1D/hist";

        private const string AlertListEndpoint = "auth/r/alerts";
        private const string SetAlertEndpoint = "auth/w/alert/set";
        private const string DeleteAlertEndpoint = "auth/w/alert/price:{}:{}/del";

        private const string AccountInfoEndpoint = "account_infos";
        private const string SummaryEndpoint = "summary";
        private const string WithdrawalFeeEndpoint = "account_fees";
        private const string PlaceOrderEndpoint = "order/new";
        private const string CancelOrderEndpoint = "order/cancel";
        private const string CancelAllOrderEndpoint = "order/cancel/all";
        private const string OrderStatusEndpoint = "order/status";

        private const string DepositAddressEndpoint = "deposit/new";
        private const string TransferEndpoint = "transfer";
        private const string WithdrawEndpoint = "withdraw";

        private const string ClaimPositionEndpoint = "position/claim";
        private const string NewOfferEndpoint = "offer/new";
        private const string CancelOfferEndpoint = "offer/cancel";
        private const string GetOfferEndpoint = "offer/status";
        private const string CloseMarginFundingEndpoint = "funding/close";
        private const string ClosePositionEndpoint = "position/close";
        #endregion

        #region constructor/destructor
        /// <summary>
        /// Create a new instance of BitfinexClient using the default options
        /// </summary>
        public BitfinexClient(): this(DefaultOptions)
        {
        }

        /// <summary>
        /// Create a new instance of BitfinexClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public BitfinexClient(BitfinexClientOptions options) : base(options, options.ApiCredentials == null ? null : new BitfinexAuthenticationProvider(options.ApiCredentials))
        {
            Configure(options);
        }
        #endregion

        #region methods
        /// <summary>
        /// Sets the default options to use for new clients
        /// </summary>
        /// <param name="options">The options to use for new clients</param>
        public static void SetDefaultOptions(BitfinexClientOptions options)
        {
            defaultOptions = options;
        }

        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        public void SetApiCredentials(string apiKey, string apiSecret)
        {
            SetAuthenticationProvider(new BitfinexAuthenticationProvider(new ApiCredentials(apiKey, apiSecret)));
        }

        #region Version2
        /// <summary>
        /// Gets the platform status
        /// </summary>
        /// <returns>Whether Bitfinex platform is running normally or not</returns>
        public WebCallResult<BitfinexPlatformStatus> GetPlatformStatus() => GetPlatformStatusAsync().Result;

        /// <summary>
        /// Gets the platform status
        /// </summary>
        /// <returns>Whether Bitfinex platform is running normally or not</returns>
        public async Task<WebCallResult<BitfinexPlatformStatus>> GetPlatformStatusAsync()
        {
            return await ExecuteRequest<BitfinexPlatformStatus>(GetUrl(StatusEndpoint, ApiVersion2)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a list of supported currencies
        /// </summary>
        /// <returns></returns>
        public WebCallResult<BitfinexCurrency[]> GetCurrencies() => GetCurrenciesAsync().Result;

        /// <summary>
        /// Gets a list of supported currencies
        /// </summary>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexCurrency[]>> GetCurrenciesAsync(params string[] symbols)
        {
            var result = await ExecuteRequest<List<BitfinexCurrency[]>>(GetUrl(CurrenciesEndpoint, ApiVersion2), Constants.GetMethod).ConfigureAwait(false);
            if(!result.Success)
                return WebCallResult<BitfinexCurrency[]>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);
            return new WebCallResult<BitfinexCurrency[]>(result.ResponseStatusCode, result.ResponseHeaders, result.Data[0], null);
        }


        /// <summary>
        /// Returns basic market data for the provided symbols
        /// </summary>
        /// <param name="symbols">The symbols to get data for</param>
        /// <returns>Market data</returns>
        public WebCallResult<BitfinexMarketOverviewRest[]> GetTicker(params string[] symbols) => GetTickerAsync(symbols).Result;

        /// <summary>
        /// Returns basic market data for the provided symbols
        /// </summary>
        /// <param name="symbols">The symbols to get data for</param>
        /// <returns>Market data</returns>
        public async Task<WebCallResult<BitfinexMarketOverviewRest[]>> GetTickerAsync(params string[] symbols)
        {
            var parameters = new Dictionary<string, object>
            {
                {"symbols", string.Join(",", symbols)}
            };

            return await ExecuteRequest<BitfinexMarketOverviewRest[]>(GetUrl(TickersEndpoint, ApiVersion2), Constants.GetMethod, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Get recent trades for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get trades for</param>
        /// <param name="limit">The amount of results</param>
        /// <param name="startTime">The start time to return trades for</param>
        /// <param name="endTime">The end time to return trades for</param>
        /// <param name="sorting">The way the result is sorted</param>
        /// <returns>Trades for the symbol</returns>
        public WebCallResult<BitfinexTradeSimple[]> GetTrades(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null) => GetTradesAsync(symbol, limit, startTime, endTime, sorting).Result;

        /// <summary>
        /// Get recent trades for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get trades for</param>
        /// <param name="limit">The amount of results</param>
        /// <param name="startTime">The start time to return trades for</param>
        /// <param name="endTime">The end time to return trades for</param>
        /// <param name="sorting">The way the result is sorted</param>
        /// <returns>Trades for the symbol</returns>
        public async Task<WebCallResult<BitfinexTradeSimple[]>> GetTradesAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null)
        {
            // Only accepts tBTCUSD format
            if (symbol.Length == 6)
                symbol = "t" + symbol.ToUpper();

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString());
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            return await ExecuteRequest<BitfinexTradeSimple[]>(GetUrl(FillPathParameter(TradesEndpoint, symbol), ApiVersion2), Constants.GetMethod, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the order book for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the order book for</param>
        /// <param name="precision">The precision of the data</param>
        /// <param name="limit">The amount of results in the book</param>
        /// <returns>The order book for the symbol</returns>
        public WebCallResult<BitfinexOrderBookEntry[]> GetOrderBook(string symbol, Precision precision, int? limit = null) => GetOrderBookAsync(symbol, precision, limit).Result;

        /// <summary>
        /// Gets the order book for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the order book for</param>
        /// <param name="precision">The precision of the data</param>
        /// <param name="limit">The amount of results in the book</param>
        /// <returns>The order book for the symbol</returns>
        public async Task<WebCallResult<BitfinexOrderBookEntry[]>> GetOrderBookAsync(string symbol, Precision precision, int? limit = null)
        {
            if (limit != null && limit != 25 && limit != 100)
                return WebCallResult<BitfinexOrderBookEntry[]>.CreateErrorResult(new ArgumentError("Limit should be either 25 or 100"));

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("len", limit?.ToString());
            var prec = JsonConvert.SerializeObject(precision, new PrecisionConverter(false));

            return await ExecuteRequest<BitfinexOrderBookEntry[]>(GetUrl(FillPathParameter(OrderBookEndpoint, symbol, prec), ApiVersion2), Constants.GetMethod, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Get various stats for the symbol
        /// </summary>
        /// <param name="symbol">The symbol to request stats for</param>
        /// <param name="key">The type of stats</param>
        /// <param name="side">Side of the stats</param>
        /// <param name="section">Section of the stats</param>
        /// <param name="sorting">The way the result should be sorted</param>
        /// <returns></returns>
        public WebCallResult<BitfinexStats> GetStats(string symbol, StatKey key, StatSide side, StatSection section, Sorting? sorting = null) => GetStatsAsync(symbol, key, side, section, sorting).Result;

        /// <summary>
        /// Get various stats for the symbol
        /// </summary>
        /// <param name="symbol">The symbol to request stats for</param>
        /// <param name="key">The type of stats</param>
        /// <param name="side">Side of the stats</param>
        /// <param name="section">Section of the stats</param>
        /// <param name="sorting">The way the result should be sorted</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexStats>> GetStatsAsync(string symbol, StatKey key, StatSide side, StatSection section, Sorting? sorting)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            var endpoint = FillPathParameter(StatsEndpoint,
                JsonConvert.SerializeObject(key, new StatKeyConverter(false)),
                symbol,
                JsonConvert.SerializeObject(side, new StatSideConverter(false)),
                JsonConvert.SerializeObject(section, new StatSectionConverter(false)));

            return await ExecuteRequest<BitfinexStats>(GetUrl(endpoint, ApiVersion2), Constants.GetMethod, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the last candle for a symbol
        /// </summary>
        /// <param name="timeFrame">The time frame of the candle</param>
        /// <param name="symbol">The symbol to get the candle for</param>
        /// <returns>The last candle for the symbol</returns>
        public WebCallResult<BitfinexCandle> GetLastCandle(TimeFrame timeFrame, string symbol)
            => GetLastCandleAsync(timeFrame, symbol).Result;

        /// <summary>
        /// Get the last candle for a symbol
        /// </summary>
        /// <param name="timeFrame">The time frame of the candle</param>
        /// <param name="symbol">The symbol to get the candle for</param>
        /// <returns>The last candle for the symbol</returns>
        public async Task<WebCallResult<BitfinexCandle>> GetLastCandleAsync(TimeFrame timeFrame, string symbol)
        {
            var endpoint = FillPathParameter(LastCandleEndpoint, JsonConvert.SerializeObject(timeFrame, new TimeFrameConverter(false)), symbol);

            return await ExecuteRequest<BitfinexCandle>(GetUrl(endpoint, ApiVersion2)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets candles for a symbol
        /// </summary>
        /// <param name="timeFrame">The time frame of the candles</param>
        /// <param name="symbol">The symbol to get the candles for</param>
        /// <param name="limit">The amount of results</param>
        /// <param name="startTime">The start time of the candles</param>
        /// <param name="endTime">The end time of the candles</param>
        /// <param name="sorting">The way the result is sorted</param>
        /// <returns></returns>
        public WebCallResult<BitfinexCandle[]> GetCandles(TimeFrame timeFrame, string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null) 
            => GetCandlesAsync(timeFrame, symbol, limit, startTime, endTime, sorting).Result;

        /// <summary>
        /// Gets candles for a symbol
        /// </summary>
        /// <param name="timeFrame">The time frame of the candles</param>
        /// <param name="symbol">The symbol to get the candles for</param>
        /// <param name="limit">The amount of results</param>
        /// <param name="startTime">The start time of the candles</param>
        /// <param name="endTime">The end time of the candles</param>
        /// <param name="sorting">The way the result is sorted</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexCandle[]>> GetCandlesAsync(TimeFrame timeFrame, string symbol,int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString());
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            var endpoint = FillPathParameter(CandlesEndpoint,
                JsonConvert.SerializeObject(timeFrame, new TimeFrameConverter(false)),
                symbol);

            return await ExecuteRequest<BitfinexCandle[]>(GetUrl(endpoint, ApiVersion2), Constants.GetMethod, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Calculate the average execution price
        /// </summary>
        /// <param name="symbol">The symbol to calculate for</param>
        /// <param name="amount">The amount to execute</param>
        /// <param name="rateLimit">Limit to price</param>
        /// <param name="period">Maximum period for margin funding</param>
        /// <returns>The average price at which the execution would happen</returns>
        public WebCallResult<BitfinexMarketAveragePrice> GetMarketAveragePrice(string symbol, decimal amount, decimal rateLimit, int? period = null) => GetMarketAveragePriceAsync(symbol, amount, rateLimit, period).Result;
        
        /// <summary>
        /// Calculate the average execution price
        /// </summary>
        /// <param name="symbol">The symbol to calculate for</param>
        /// <param name="amount">The amount to execute</param>
        /// <param name="rateLimit">Limit to price</param>
        /// <param name="period">Maximum period for margin funding</param>
        /// <returns>The average price at which the execution would happen</returns>
        public async Task<WebCallResult<BitfinexMarketAveragePrice>> GetMarketAveragePriceAsync(string symbol, decimal amount, decimal? rateLimit = null, int? period = null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("period", period?.ToString());
            parameters.AddOptionalParameter("rate_limit", rateLimit?.ToString(CultureInfo.InvariantCulture));

            return await ExecuteRequest<BitfinexMarketAveragePrice>(GetUrl(MarketAverageEndpoint, ApiVersion2), Constants.PostMethod, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the exchange rate for the currencies
        /// </summary>
        /// <param name="currency1">The first currency</param>
        /// <param name="currency2">The second currency</param>
        public WebCallResult<BitfinexForeignExchangeRate> GetForeignExchangeRate(string currency1, string currency2) => GetForeignExchangeRateAsync(currency1, currency2).Result;

        /// <summary>
        /// Returns the exchange rate for the currencies
        /// </summary>
        /// <param name="currency1">The first currency</param>
        /// <param name="currency2">The second currency</param>
        public async Task<WebCallResult<BitfinexForeignExchangeRate>> GetForeignExchangeRateAsync(string currency1, string currency2)
        {
            var parameters = new Dictionary<string, object>
            {
                { "ccy1", currency1 },
                { "ccy2", currency2 }
            };

            return await ExecuteRequest<BitfinexForeignExchangeRate>(GetUrl(ForeignExchangeEndpoint, ApiVersion2), Constants.PostMethod, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Get all funds
        /// </summary>
        /// <returns></returns>
        public WebCallResult<BitfinexWallet[]> GetWallets() => GetWalletsAsync().Result;

        /// <summary>
        /// Get all funds
        /// </summary>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexWallet[]>> GetWalletsAsync()
        {
            return await ExecuteRequest<BitfinexWallet[]>(GetUrl(WalletsEndpoint, ApiVersion2), Constants.PostMethod, null, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the active orders
        /// </summary>
        /// <returns></returns>
        public WebCallResult<BitfinexOrder[]> GetActiveOrders() => GetActiveOrdersAsync().Result;

        /// <summary>
        /// Get the active orders
        /// </summary>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexOrder[]>> GetActiveOrdersAsync()
        {
            return await ExecuteRequest<BitfinexOrder[]>(GetUrl(OpenOrdersEndpoint, ApiVersion2), Constants.PostMethod, null, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the order history for a symbol for this account
        /// </summary>
        /// <param name="symbol">The symbol to get the history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        public WebCallResult<BitfinexOrder[]> GetOrderHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetOrderHistoryAsync(symbol, startTime, endTime, limit).Result;

        /// <summary>
        /// Get the order history for a symbol for this account
        /// </summary>
        /// <param name="symbol">The symbol to get the history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexOrder[]>> GetOrderHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString());
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);

            return await ExecuteRequest<BitfinexOrder[]>(GetUrl(FillPathParameter(OrderHistoryEndpoint, symbol), ApiVersion2), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the individual trades for an order
        /// </summary>
        /// <param name="symbol">The symbol of the order</param>
        /// <param name="orderId">The order Id</param>
        /// <returns></returns>
        public WebCallResult<BitfinexTradeDetails[]> GetTradesForOrder(string symbol, long orderId) => GetTradesForOrderAsync(symbol, orderId).Result;

        /// <summary>
        /// Get the individual trades for an order
        /// </summary>
        /// <param name="symbol">The symbol of the order</param>
        /// <param name="orderId">The order Id</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexTradeDetails[]>> GetTradesForOrderAsync(string symbol, long orderId)
        {
            return await ExecuteRequest<BitfinexTradeDetails[]>(GetUrl(FillPathParameter(OrderTradesEndpoint, symbol, orderId.ToString()), ApiVersion2), Constants.PostMethod, null, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the trade history for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        public WebCallResult<BitfinexTradeDetails[]> GetTradeHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetTradeHistoryAsync(symbol, startTime, endTime, limit).Result;

        /// <summary>
        /// Get the trade history for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexTradeDetails[]>> GetTradeHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString());
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);

            return await ExecuteRequest<BitfinexTradeDetails[]>(GetUrl(FillPathParameter(MyTradesEndpoint, symbol), ApiVersion2), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the active positions
        /// </summary>
        /// <returns></returns>
        public WebCallResult<BitfinexPosition[]> GetActivePositions() => GetActivePositionsAsync().Result;

        /// <summary>
        /// Get the active positions
        /// </summary>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexPosition[]>> GetActivePositionsAsync()
        {
            return await ExecuteRequest<BitfinexPosition[]>(GetUrl(ActivePositionsEndpoint, ApiVersion2), Constants.PostMethod, null, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get a list of historical positions
        /// </summary>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        public WebCallResult<BitfinexPositionExtended[]> GetPositionHistory(DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetPositionHistoryAsync(startTime, endTime, limit).Result;

        /// <summary>
        /// Get a list of historical positions
        /// </summary>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexPositionExtended[]>> GetPositionHistoryAsync(DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString());
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);

            return await ExecuteRequest<BitfinexPositionExtended[]>(GetUrl(PositionHistoryEndpoint, ApiVersion2), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get positions by id
        /// </summary>
        /// <param name="ids">The id's of positions to return</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        public WebCallResult<BitfinexPositionExtended[]> GetPositionsById(string[] ids, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetPositionsByIdAsync(ids, startTime, endTime, limit).Result;

        /// <summary>
        /// Get positions by id
        /// </summary>
        /// <param name="ids">The id's of positions to return</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexPositionExtended[]>> GetPositionsByIdAsync(string[] ids, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "id", ids }
            };
            parameters.AddOptionalParameter("limit", limit?.ToString());
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);

            return await ExecuteRequest<BitfinexPositionExtended[]>(GetUrl(PositionAuditEndpoint, ApiVersion2), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the active funding offers
        /// </summary>
        /// <param name="symbol">The symbol to return the funding offer for</param>
        /// <returns></returns>
        public WebCallResult<BitfinexFundingOffer[]> GetActiveFundingOffers(string symbol) => GetActiveFundingOffersAsync(symbol).Result;

        /// <summary>
        /// Get the active funding offers
        /// </summary>
        /// <param name="symbol">The symbol to return the funding offer for</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexFundingOffer[]>> GetActiveFundingOffersAsync(string symbol)
        {
            return await ExecuteRequest<BitfinexFundingOffer[]>(GetUrl(FillPathParameter(ActiveFundingOffersEndpoint, symbol), ApiVersion2), Constants.PostMethod, null, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the funding offer history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        public WebCallResult<BitfinexFundingOffer[]> GetFundingOfferHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetFundingOfferHistoryAsync(symbol, startTime, endTime, limit).Result;
        
        /// <summary>
        /// Get the funding offer history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexFundingOffer[]>> GetFundingOfferHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString());
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);

            return await ExecuteRequest<BitfinexFundingOffer[]>(GetUrl(FillPathParameter(FundingOfferHistoryEndpoint, symbol), ApiVersion2), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the funding loans
        /// </summary>
        /// <param name="symbol">The symbol to get the funding loans for</param>
        /// <returns></returns>
        public WebCallResult<BitfinexFunding[]> GetFundingLoans(string symbol) => GetFundingLoansAsync(symbol).Result;

        /// <summary>
        /// Get the funding loans
        /// </summary>
        /// <param name="symbol">The symbol to get the funding loans for</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexFunding[]>> GetFundingLoansAsync(string symbol)
        {
            return await ExecuteRequest<BitfinexFunding[]>(GetUrl(FillPathParameter(FundingLoansEndpoint, symbol), ApiVersion2), Constants.PostMethod, null, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the funding loan history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        public WebCallResult<BitfinexFunding[]> GetFundingLoansHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetFundingLoansHistoryAsync(symbol, startTime, endTime, limit).Result;

        /// <summary>
        /// Get the funding loan history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexFunding[]>> GetFundingLoansHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString());
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);

            return await ExecuteRequest<BitfinexFunding[]>(GetUrl(FillPathParameter(FundingLoansHistoryEndpoint, symbol), ApiVersion2), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the funding credits
        /// </summary>
        /// <param name="symbol">The symbol to get the funding credits for</param>
        /// <returns></returns>
        public WebCallResult<BitfinexFundingCredit[]> GetFundingCredits(string symbol) => GetFundingCreditsAsync(symbol).Result;

        /// <summary>
        /// Get the funding credits
        /// </summary>
        /// <param name="symbol">The symbol to get the funding credits for</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexFundingCredit[]>> GetFundingCreditsAsync(string symbol)
        {
            return await ExecuteRequest<BitfinexFundingCredit[]>(GetUrl(FillPathParameter(FundingCreditsEndpoint, symbol), ApiVersion2), Constants.PostMethod, null, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the funding credits history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        public WebCallResult<BitfinexFundingCredit[]> GetFundingCreditsHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetFundingCreditsHistoryAsync(symbol, startTime, endTime, limit).Result;

        /// <summary>
        /// Get the funding credits history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexFundingCredit[]>> GetFundingCreditsHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString());
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);

            return await ExecuteRequest<BitfinexFundingCredit[]>(GetUrl(FillPathParameter(FundingCreditsHistoryEndpoint, symbol), ApiVersion2), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the funding trades history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        public WebCallResult<BitfinexFundingTrade[]> GetFundingTradesHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetFundingTradesHistoryAsync(symbol, startTime, endTime, limit).Result;

        /// <summary>
        /// Get the funding trades history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexFundingTrade[]>> GetFundingTradesHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString());
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);

            return await ExecuteRequest<BitfinexFundingTrade[]>(GetUrl(FillPathParameter(FundingTradesEndpoint, symbol), ApiVersion2), Constants.PostMethod).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the base margin info
        /// </summary>
        /// <returns></returns>
        public WebCallResult<BitfinexMarginBase> GetBaseMarginInfo() => GetBaseMarginInfoAsync().Result;

        /// <summary>
        /// Get the base margin info
        /// </summary>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexMarginBase>> GetBaseMarginInfoAsync()
        {
            return await ExecuteRequest<BitfinexMarginBase>(GetUrl(MarginInfoBaseEndpoint, ApiVersion2), Constants.PostMethod, null, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the margin info for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the info for</param>
        /// <returns></returns>
        public WebCallResult<BitfinexMarginSymbol> GetSymbolMarginInfo(string symbol) => GetSymbolMarginInfoAsync(symbol).Result;

        /// <summary>
        /// Get the margin info for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the info for</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexMarginSymbol>> GetSymbolMarginInfoAsync(string symbol)
        {
            return await ExecuteRequest<BitfinexMarginSymbol>(GetUrl(FillPathParameter(MarginInfoSymbolEndpoint, symbol), ApiVersion2), Constants.PostMethod, null, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get funding info for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the info for</param>
        /// <returns></returns>
        public WebCallResult<BitfinexFundingInfo> GetFundingInfo(string symbol) => GetFundingInfoAsync(symbol).Result;

        /// <summary>
        /// Get funding info for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the info for</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexFundingInfo>> GetFundingInfoAsync(string symbol)
        {
            return await ExecuteRequest<BitfinexFundingInfo>(GetUrl(FillPathParameter(FundingInfoEndpoint, symbol), ApiVersion2), Constants.PostMethod, null, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the withdrawal/deposit history
        /// </summary>
        /// <param name="symbol">Symbol to get history for</param>
        /// <returns></returns>
        public WebCallResult<BitfinexMovement[]> GetMovements(string symbol) => GetMovementsAsync(symbol).Result;

        /// <summary>
        /// Get the withdrawal/deposit history
        /// </summary>
        /// <param name="symbol">Symbol to get history for</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexMovement[]>> GetMovementsAsync(string symbol)
        {
            return await ExecuteRequest<BitfinexMovement[]>(GetUrl(FillPathParameter(MovementsEndpoint, symbol), ApiVersion2), Constants.PostMethod, null, true).ConfigureAwait(false);
        }
        
        public WebCallResult<BitfinexPerformance> GetDailyPerformance() => GetDailyPerformanceAsync().Result;
        public async Task<WebCallResult<BitfinexPerformance>> GetDailyPerformanceAsync()
        {
            // TODO doesn't work?
            return await ExecuteRequest<BitfinexPerformance>(GetUrl(DailyPerformanceEndpoint, ApiVersion2), Constants.PostMethod, null, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the list of alerts
        /// </summary>
        /// <returns></returns>
        public WebCallResult<BitfinexAlert[]> GetAlertList() => GetAlertListAsync().Result;
        
        /// <summary>
        /// Get the list of alerts
        /// </summary>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexAlert[]>> GetAlertListAsync()
        {
            var parameters = new Dictionary<string, object>
            {
                { "type", "price" } 
            };

            return await ExecuteRequest<BitfinexAlert[]>(GetUrl(AlertListEndpoint, ApiVersion2), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Set an alert
        /// </summary>
        /// <param name="symbol">The symbol to set the alert for</param>
        /// <param name="price">The price to set the alert for</param>
        /// <returns></returns>
        public WebCallResult<BitfinexAlert> SetAlert(string symbol, decimal price) => SetAlertAsync(symbol, price).Result;

        /// <summary>
        /// Set an alert
        /// </summary>
        /// <param name="symbol">The symbol to set the alert for</param>
        /// <param name="price">The price to set the alert for</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexAlert>> SetAlertAsync(string symbol, decimal price)
        {
            var parameters = new Dictionary<string, object>
            {
                { "type", "price" },
                { "symbol", symbol },
                { "price", price.ToString(CultureInfo.InvariantCulture) }
            };

            return await ExecuteRequest<BitfinexAlert>(GetUrl(SetAlertEndpoint, ApiVersion2), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete an existing alert
        /// </summary>
        /// <param name="symbol">The symbol of the alert to delete</param>
        /// <param name="price">The price of the alert to delete</param>
        /// <returns></returns>
        public WebCallResult<BitfinexSuccessResult> DeleteAlert(string symbol, decimal price) => DeleteAlertAsync(symbol, price).Result;

        /// <summary>
        /// Delete an existing alert
        /// </summary>
        /// <param name="symbol">The symbol of the alert to delete</param>
        /// <param name="price">The price of the alert to delete</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexSuccessResult>> DeleteAlertAsync(string symbol, decimal price)
        {
            return await ExecuteRequest<BitfinexSuccessResult>(GetUrl(FillPathParameter(DeleteAlertEndpoint, symbol, price.ToString(CultureInfo.InvariantCulture)), ApiVersion2), Constants.PostMethod, null, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Calculates the available balance for a symbol at a specific rate
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="side">Buy or sell</param>
        /// <param name="rate">The rate/price</param>
        /// <param name="type">The wallet type</param>
        /// <returns></returns>
        public WebCallResult<BitfinexAvailableBalance> GetAvailableBalance(string symbol, OrderSide side, decimal rate, WalletType type) => GetAvailableBalanceAsync(symbol, side, rate, type).Result;

        /// <summary>
        /// Calculates the available balance for a symbol at a specific rate
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="side">Buy or sell</param>
        /// <param name="rate">The rate/price</param>
        /// <param name="type">The wallet type</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexAvailableBalance>> GetAvailableBalanceAsync(string symbol, OrderSide side, decimal rate, WalletType type)
        {
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "dir", side == OrderSide.Buy ? 1: -1 },
                { "rate", rate },
                { "type", JsonConvert.SerializeObject(type, new WalletTypeConverter(false)).ToUpper() }
            };

            return await ExecuteRequest<BitfinexAvailableBalance>(GetUrl(CalcAvailableBalanceEndpoint, ApiVersion2), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get changes in your balance for a currency
        /// </summary>
        /// <param name="currency">The currency to check the ledger for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        public WebCallResult<BitfinexLedgerEntry[]> GetLedgerEntries(string currency, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetLedgerEntriesAsync(currency, startTime, endTime, limit).Result;

        /// <summary>
        /// Get changes in your balance for a currency
        /// </summary>
        /// <param name="currency">The currency to check the ledger for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexLedgerEntry[]>> GetLedgerEntriesAsync(string currency, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString());
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter()) : null);

            return await ExecuteRequest<BitfinexLedgerEntry[]>(GetUrl(FillPathParameter(LedgerEntriesEndpoint, currency), ApiVersion2), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }
        
        /// <summary>
        /// Gets information about the user associated with the api key/secret
        /// </summary>
        /// <returns></returns>
        public WebCallResult<BitfinexUserInfo> GetUserInfo() => GetUserInfoAsync().Result;

        /// <summary>
        /// Gets information about the user associated with the api key/secret
        /// </summary>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexUserInfo>> GetUserInfoAsync()
        {
            return await ExecuteRequest<BitfinexUserInfo>(GetUrl(UserInfoEndpoint, ApiVersion2), Constants.PostMethod, signed: true).ConfigureAwait(false);
        }
        #endregion

        #region Version1

        /// <summary>
        /// Gets the margin funding book
        /// </summary>
        /// <param name="currency">Currency to get the book for</param>
        /// <param name="limit">Limit of the results</param>
        /// <returns></returns>
        public WebCallResult<BitfinexFundingBook> GetFundingBook(string currency, int? limit = null) => GetFundingBookAsync(currency, limit).Result;

        /// <summary>
        /// Gets the margin funding book
        /// </summary>
        /// <param name="currency">Currency to get the book for</param>
        /// <param name="limit">Limit of the results</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexFundingBook>> GetFundingBookAsync(string currency, int? limit = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit_bids", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("limit_asks", limit?.ToString(CultureInfo.InvariantCulture));
            return await ExecuteRequest<BitfinexFundingBook>(GetUrl(FillPathParameter(FundingBookEndpoint, currency), ApiVersion1)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the most recent lends
        /// </summary>
        /// <param name="currency">Currency to get the book for</param>
        /// <param name="startTime">Return data after this time</param>
        /// <param name="limit">Limit of the results</param>
        /// <returns></returns>
        public WebCallResult<BitfinexLend[]> GetLends(string currency, DateTime? startTime = null, int? limit = null) => GetLendsAsync(currency, startTime, limit).Result;

        /// <summary>
        /// Gets the most recent lends
        /// </summary>
        /// <param name="currency">Currency to get the book for</param>
        /// <param name="startTime">Return data after this time</param>
        /// <param name="limit">Limit of the results</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexLend[]>> GetLendsAsync(string currency, DateTime? startTime = null, int ? limit = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit_lends", limit?.ToString(CultureInfo.InvariantCulture));
            return await ExecuteRequest<BitfinexLend[]>(GetUrl(FillPathParameter(LendsEndpoint, currency), ApiVersion1)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a list of all symbols
        /// </summary>
        /// <returns></returns>
        public WebCallResult<string[]> GetSymbols() => GetSymbolsAsync().Result;

        /// <summary>
        /// Gets a list of all symbols
        /// </summary>
        /// <returns></returns>
        public async Task<WebCallResult<string[]>> GetSymbolsAsync()
        {
            return await ExecuteRequest<string[]>(GetUrl(SymbolsEndpoint, ApiVersion1)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets details of all symbols
        /// </summary>
        /// <returns></returns>
        public WebCallResult<BitfinexSymbolDetails[]> GetSymbolDetails() => GetSymbolDetailsAsync().Result;

        /// <summary>
        /// Gets details of all symbols
        /// </summary>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexSymbolDetails[]>> GetSymbolDetailsAsync()
        {
            return await ExecuteRequest<BitfinexSymbolDetails[]>(GetUrl(SymbolDetailsEndpoint, ApiVersion1)).ConfigureAwait(false);
        }

        /// <summary>
        /// Get information about your account
        /// </summary>
        /// <returns></returns>
        public WebCallResult<BitfinexAccountInfo> GetAccountInfo() => GetAccountInfoAsync().Result;

        /// <summary>
        /// Get information about your account
        /// </summary>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexAccountInfo>> GetAccountInfoAsync()
        {
            var result = await ExecuteRequest<BitfinexAccountInfo[]>(GetUrl(AccountInfoEndpoint, ApiVersion1), Constants.PostMethod, null, true).ConfigureAwait(false);
            return result.Success ? new WebCallResult<BitfinexAccountInfo>(result.ResponseStatusCode, result.ResponseHeaders, result.Data[0], null) : WebCallResult<BitfinexAccountInfo>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);
        }

        /// <summary>
        /// Get withdrawal fees for this account
        /// </summary>
        /// <returns></returns>
        public WebCallResult<BitfinexWithdrawalFees> GetWithdrawalFees() => GetWithdrawalFeesAsync().Result;

        /// <summary>
        /// Get withdrawal fees for this account
        /// </summary>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexWithdrawalFees>> GetWithdrawalFeesAsync()
        {
            return await ExecuteRequest<BitfinexWithdrawalFees>(GetUrl(WithdrawalFeeEndpoint, ApiVersion1), Constants.PostMethod, null, true).ConfigureAwait(false);
        }
        
        /// <summary>
        /// Get 30-day summary on trading volume and margin funding
        /// </summary>
        /// <returns></returns>
        public WebCallResult<Bitfinex30DaySummary> Get30DaySummary() => Get30DaySummaryAsync().Result;

        /// <summary>
        /// Get 30-day summary on trading volume and margin funding
        /// </summary>
        /// <returns></returns>
        public async Task<WebCallResult<Bitfinex30DaySummary>> Get30DaySummaryAsync()
        {
            return await ExecuteRequest<Bitfinex30DaySummary>(GetUrl(SummaryEndpoint, ApiVersion1), Constants.PostMethod, null, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Place a new order
        /// </summary>
        /// <param name="symbol">Symbol to place order for</param>
        /// <param name="side">Side of the order</param>
        /// <param name="type">Type of the order</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price for the order</param>
        /// <param name="hidden">If the order should be placed as hidden</param>
        /// <param name="postOnly">If the only should only be placed if it isn't immediately filled</param>
        /// <param name="useAllAvailable">If all available funds should be used</param>
        /// <param name="stopLimitPrice">The stop price if a stop limit order is placed</param>
        /// <param name="ocoOrder">If the order is a one-cancels-other order</param>
        /// <param name="ocoBuyPrice">The one-cancels-other buy price</param>
        /// <param name="ocoSellPrice">The one-cancels-other sell price</param>
        /// <returns></returns>
        public WebCallResult<BitfinexPlacedOrder> PlaceOrder(
            string symbol,
            OrderSide side,
            OrderTypeV1 type,
            decimal amount, 
            decimal price, 
            bool? hidden = null, 
            bool? postOnly = null, 
            bool? useAllAvailable = null,
            decimal? stopLimitPrice = null,
            bool? ocoOrder = null, 
            decimal? ocoBuyPrice = null, 
            decimal? ocoSellPrice = null) => PlaceOrderAsync(symbol, side, type, amount, price, hidden, postOnly, useAllAvailable, stopLimitPrice, ocoOrder, ocoBuyPrice, ocoSellPrice).Result;

        /// <summary>
        /// Place a new order
        /// </summary>
        /// <param name="symbol">Symbol to place order for</param>
        /// <param name="side">Side of the order</param>
        /// <param name="type">Type of the order</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price for the order</param>
        /// <param name="hidden">If the order should be placed as hidden</param>
        /// <param name="postOnly">If the only should only be placed if it isn't immediately filled</param>
        /// <param name="useAllAvailable">If all available funds should be used</param>
        /// <param name="stopLimitPrice">The stop price if a stop limit order is placed</param>
        /// <param name="ocoOrder">If the order is a one-cancels-other order</param>
        /// <param name="ocoBuyPrice">The one-cancels-other buy price</param>
        /// <param name="ocoSellPrice">The one-cancels-other sell price</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexPlacedOrder>> PlaceOrderAsync(
            string symbol, 
            OrderSide side, 
            OrderTypeV1 type, 
            decimal amount, 
            decimal price, 
            bool? hidden = null,
            bool? postOnly = null, 
            bool? useAllAvailable = null,
            decimal? stopLimitPrice = null,
            bool? ocoOrder = null, 
            decimal? ocoBuyPrice = null,
            decimal? ocoSellPrice = null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) },
                { "price", price.ToString(CultureInfo.InvariantCulture) },
                { "exchange", "bitfinex" },
                { "side", JsonConvert.SerializeObject(side, new OrderSideConverter(false)) },
                { "type", JsonConvert.SerializeObject(type, new OrderTypeV1Converter(false)) }
            };
            parameters.AddOptionalParameter("is_hidden", hidden);
            parameters.AddOptionalParameter("is_postonly", postOnly);
            parameters.AddOptionalParameter("use_all_available", useAllAvailable == true ? "1": null);
            parameters.AddOptionalParameter(side == OrderSide.Buy ? "buy_stoplimit_price" : "sell_stoplimit_price", stopLimitPrice?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("ocoorder", ocoOrder);
            parameters.AddOptionalParameter("buy_price_oco", ocoBuyPrice);
            parameters.AddOptionalParameter("sell_price_oco", ocoSellPrice);

            return await ExecuteRequest<BitfinexPlacedOrder>(GetUrl(PlaceOrderEndpoint, ApiVersion1), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancel a specific order
        /// </summary>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <returns></returns>
        public WebCallResult<BitfinexPlacedOrder> CancelOrder(long orderId) => CancelOrderAsync(orderId).Result;

        /// <summary>
        /// Cancel a specific order
        /// </summary>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexPlacedOrder>> CancelOrderAsync(long orderId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "order_id", orderId }
            };

            return await ExecuteRequest<BitfinexPlacedOrder>(GetUrl(CancelOrderEndpoint, ApiVersion1), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancels all open orders
        /// </summary>
        /// <returns></returns>
        public WebCallResult<BitfinexResult> CancelAllOrders() => CancelAllOrdersAsync().Result;

        /// <summary>
        /// Cancels all open orders
        /// </summary>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexResult>> CancelAllOrdersAsync()
        {
            return await ExecuteRequest<BitfinexResult>(GetUrl(CancelAllOrderEndpoint, ApiVersion1), Constants.PostMethod, null, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the status of a specific order
        /// </summary>
        /// <param name="orderId">The order id of the order to get</param>
        /// <returns></returns>
        public WebCallResult<BitfinexPlacedOrder> GetOrder(long orderId) => GetOrderAsync(orderId).Result;

        /// <summary>
        /// Get the status of a specific order
        /// </summary>
        /// <param name="orderId">The order id of the order to get</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexPlacedOrder>> GetOrderAsync(long orderId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "order_id", orderId }
            };

            return await ExecuteRequest<BitfinexPlacedOrder>(GetUrl(OrderStatusEndpoint, ApiVersion1), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }
        
        /// <summary>
        /// Gets a deposit address for a currency
        /// </summary>
        /// <param name="currency">The currency to get address for</param>
        /// <param name="toWallet">The type of wallet the deposit is for</param>
        /// <param name="forceNew">If true a new address will be generated (previous addresses will still be valid)</param>
        /// <returns></returns>
        public WebCallResult<BitfinexDepositAddress> GetDepositAddress(string currency, WithdrawWallet toWallet, bool? forceNew = null) => GetDepositAddressAsync(currency, toWallet, forceNew).Result;

        /// <summary>
        /// Gets a deposit address for a currency
        /// </summary>
        /// <param name="currency">The currency to get address for</param>
        /// <param name="toWallet">The type of wallet the deposit is for</param>
        /// <param name="forceNew">If true a new address will be generated (previous addresses will still be valid)</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexDepositAddress>> GetDepositAddressAsync(string currency, WithdrawWallet toWallet, bool? forceNew = null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "method", currency },
                { "wallet_name", JsonConvert.SerializeObject(toWallet, new WithdrawWalletConverter(false)) }
            };
            parameters.AddOptionalParameter("renew", forceNew.HasValue? JsonConvert.SerializeObject(toWallet, new BoolToIntConverter(false)): null);

            return await ExecuteRequest<BitfinexDepositAddress>(GetUrl(DepositAddressEndpoint, ApiVersion1), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Transfers funds from one wallet to another
        /// </summary>
        /// <param name="currency">The currency to transfer</param>
        /// <param name="fromWallet">The wallet to remove funds from</param>
        /// <param name="toWallet">The wallet to add funds to</param>
        /// <param name="amount">The amount to transfer</param>
        /// <returns></returns>
        public WebCallResult<BitfinexTransferResult> WalletTransfer(string currency, decimal amount, WithdrawWallet fromWallet, WithdrawWallet toWallet) => WalletTransferAsync(currency, amount, fromWallet, toWallet).Result;

        /// <summary>
        /// Transfers funds from one wallet to another
        /// </summary>
        /// <param name="currency">The currency to transfer</param>
        /// <param name="fromWallet">The wallet to remove funds from</param>
        /// <param name="toWallet">The wallet to add funds to</param>
        /// <param name="amount">The amount to transfer</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexTransferResult>> WalletTransferAsync(string currency, decimal amount, WithdrawWallet fromWallet, WithdrawWallet toWallet)
        {
            var parameters = new Dictionary<string, object>
            {
                { "currency", currency },
                { "amount", amount },
                { "walletfrom", JsonConvert.SerializeObject(fromWallet, new WithdrawWalletConverter(false)) },
                { "walletto", JsonConvert.SerializeObject(toWallet, new WithdrawWalletConverter(false)) },
            };
            return await ExecuteRequest<BitfinexTransferResult>(GetUrl(TransferEndpoint, ApiVersion1), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Withdraw funds from Bitfinex, either to a crypto currency address or a bank account
        /// All withdrawals need the withdrawType, wallet and amount parameters
        /// CryptoCurrency withdrawals need the address parameters, the paymentId can be used for Monero as payment id and for Ripple as tag
        /// Wire withdrawals need the bank parameters. In some cases your bank will require the use of an intermediary bank, if this is the case, please supply those fields as well.
        /// </summary>
        /// <param name="withdrawType">The type of funds to withdraw</param>
        /// <param name="wallet">The wallet to withdraw from</param>
        /// <param name="amount">The amount to withdraw</param>
        /// <param name="address">The destination of the withdrawal</param>
        /// <param name="accountNumber">The account number</param>
        /// <param name="bankSwift">The SWIFT code of the bank</param>
        /// <param name="bankName">The bank name</param>
        /// <param name="bankAddress">The bank address</param>
        /// <param name="bankCity">The bank city</param>
        /// <param name="bankCountry">The bank country</param>
        /// <param name="paymentDetails">Message for the receiver</param>
        /// <param name="expressWire">Whether it is an express wire withdrawal</param>
        /// <param name="intermediaryBankName">Intermediary bank name</param>
        /// <param name="intermediaryBankAddress">Intermediary bank address</param>
        /// <param name="intermediaryBankCity">Intermediary bank city</param>
        /// <param name="intermediaryBankCountry">Intermediary bank country</param>
        /// <param name="intermediaryBankAccount">Intermediary bank account</param>
        /// <param name="intermediaryBankSwift">Intermediary bank SWIFT code</param>
        /// <param name="accountName">The name of the account</param>
        /// <param name="paymentId">Hex string for Monero transaction</param>
        /// <returns></returns>
        public WebCallResult<BitfinexWithdrawalResult> Withdraw(string withdrawType,
                                                             WithdrawWallet wallet,
                                                             decimal amount,
                                                             string address = null,
                                                             string accountNumber = null,
                                                             string bankSwift = null,
                                                             string bankName = null,
                                                             string bankAddress = null,
                                                             string bankCity = null,
                                                             string bankCountry = null,
                                                             string paymentDetails = null,
                                                             bool? expressWire = null,
                                                             string intermediaryBankName = null,
                                                             string intermediaryBankAddress = null,
                                                             string intermediaryBankCity = null,
                                                             string intermediaryBankCountry = null,
                                                             string intermediaryBankAccount = null,
                                                             string intermediaryBankSwift = null,
                                                             string accountName = null,
                                                             string paymentId = null) =>
                                                             WithdrawAsync(withdrawType, wallet, amount, address, accountNumber, bankSwift, bankName, bankAddress,
                                                                 bankCity, bankCountry, paymentDetails, expressWire, intermediaryBankName, intermediaryBankAddress,
                                                                 intermediaryBankCity, intermediaryBankCountry, intermediaryBankAccount, intermediaryBankSwift,
                                                                 accountName, paymentId).Result;
                                                                                    

        /// <summary>
        /// Withdraw funds from Bitfinex, either to a crypto currency address or a bank account
        /// All withdrawals need the withdrawType, wallet and amount parameters
        /// CryptoCurrency withdrawals need the address parameters, the paymentId can be used for Monero as payment id and for Ripple as tag
        /// Wire withdrawals need the bank parameters. In some cases your bank will require the use of an intermediary bank, if this is the case, please supply those fields as well.
        /// </summary>
        /// <param name="withdrawType">The type of funds to withdraw</param>
        /// <param name="wallet">The wallet to withdraw from</param>
        /// <param name="amount">The amount to withdraw</param>
        /// <param name="address">The destination of the withdrawal</param>
        /// <param name="accountNumber">The account number</param>
        /// <param name="bankSwift">The SWIFT code of the bank</param>
        /// <param name="bankName">The bank name</param>
        /// <param name="bankAddress">The bank address</param>
        /// <param name="bankCity">The bank city</param>
        /// <param name="bankCountry">The bank country</param>
        /// <param name="paymentDetails">Message for the receiver</param>
        /// <param name="expressWire">Whether it is an express wire withdrawal</param>
        /// <param name="intermediaryBankName">Intermediary bank name</param>
        /// <param name="intermediaryBankAddress">Intermediary bank address</param>
        /// <param name="intermediaryBankCity">Intermediary bank city</param>
        /// <param name="intermediaryBankCountry">Intermediary bank country</param>
        /// <param name="intermediaryBankAccount">Intermediary bank account</param>
        /// <param name="intermediaryBankSwift">Intermediary bank SWIFT code</param>
        /// <param name="accountName">The name of the account</param>
        /// <param name="paymentId">Hex string for Monero transaction</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexWithdrawalResult>> WithdrawAsync(string withdrawType, 
                                                                         WithdrawWallet wallet, 
                                                                         decimal amount, 
                                                                         string address = null, 
                                                                         string accountNumber = null,
                                                                         string bankSwift = null, 
                                                                         string bankName = null,
                                                                         string bankAddress = null,
                                                                         string bankCity = null,
                                                                         string bankCountry = null,
                                                                         string paymentDetails = null,
                                                                         bool? expressWire = null,
                                                                         string intermediaryBankName = null,
                                                                         string intermediaryBankAddress = null,
                                                                         string intermediaryBankCity = null,
                                                                         string intermediaryBankCountry = null,
                                                                         string intermediaryBankAccount = null,
                                                                         string intermediaryBankSwift = null,
                                                                         string accountName = null, 
                                                                         string paymentId = null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "withdraw_type", withdrawType },
                { "walletselected", JsonConvert.SerializeObject(wallet, new WithdrawWalletConverter(false)) },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("address", address);
            parameters.AddOptionalParameter("payment_id", paymentId);
            parameters.AddOptionalParameter("account_name", accountName);
            parameters.AddOptionalParameter("account_number", accountNumber);
            parameters.AddOptionalParameter("swift", bankSwift);
            parameters.AddOptionalParameter("bank_name", bankName);
            parameters.AddOptionalParameter("bank_address", bankAddress);
            parameters.AddOptionalParameter("bank_city", bankCity);
            parameters.AddOptionalParameter("bank_country", bankCountry);
            parameters.AddOptionalParameter("detail_payment", paymentDetails);
            parameters.AddOptionalParameter("expressWire", expressWire == null ? null :JsonConvert.SerializeObject(expressWire, new BoolToIntConverter(false)));
            parameters.AddOptionalParameter("intermediary_bank_name", intermediaryBankName);
            parameters.AddOptionalParameter("intermediary_bank_address", intermediaryBankAddress);
            parameters.AddOptionalParameter("intermediary_bank_city", intermediaryBankCity);
            parameters.AddOptionalParameter("intermediary_bank_country", intermediaryBankCountry);
            parameters.AddOptionalParameter("intermediary_bank_account", intermediaryBankAccount);
            parameters.AddOptionalParameter("intermediary_bank_swift", intermediaryBankSwift);

            var result = await ExecuteRequest<BitfinexWithdrawalResult[]>(GetUrl(WithdrawEndpoint, ApiVersion1), Constants.PostMethod, parameters, true).ConfigureAwait(false);
            if (!result.Success)
                return WebCallResult<BitfinexWithdrawalResult>.CreateErrorResult(result.ResponseStatusCode, null, result.Error);

            if(!result.Data[0].Success)
                return WebCallResult<BitfinexWithdrawalResult>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, new ServerError(result.Data[0].Message));
            return new WebCallResult<BitfinexWithdrawalResult>(result.ResponseStatusCode, result.ResponseHeaders, result.Data[0], null);
        }
        
        /// <summary>
        /// Claim a position
        /// </summary>
        /// <param name="id">The id of the position to claim</param>
        /// <param name="amount">The (partial) amount to be claimed</param>
        /// <returns></returns>
        public WebCallResult<BitfinexDepositAddress> ClaimPosition(long id, decimal amount) => ClaimPositionAsync(id, amount).Result;

        /// <summary>
        /// Claim a position
        /// </summary>
        /// <param name="id">The id of the position to claim</param>
        /// <param name="amount">The (partial) amount to be claimed</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexDepositAddress>> ClaimPositionAsync(long id, decimal amount)
        {
            var parameters = new Dictionary<string, object>
            {
                { "position_id", id },
                { "amount", amount }
            };
            return await ExecuteRequest<BitfinexDepositAddress>(GetUrl(ClaimPositionEndpoint, ApiVersion1), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Submit a new order
        /// </summary>
        /// <param name="currency">The currency</param>
        /// <param name="amount">The amount</param>
        /// <param name="rate">Rate to lend or borrow at in percent per 365 days (0 for FRR)</param>
        /// <param name="period">Number of days</param>
        /// <param name="direction">Direction of the offer</param>
        /// <returns></returns>
        public WebCallResult<BitfinexOffer> NewOffer(string currency, decimal amount, decimal rate, int period, FundingType direction) => NewOfferAsync(currency, amount, rate, period, direction).Result;

        /// <summary>
        /// Submit a new order
        /// </summary>
        /// <param name="currency">The currency</param>
        /// <param name="amount">The amount</param>
        /// <param name="rate">Rate to lend or borrow at in percent per 365 days (0 for FRR)</param>
        /// <param name="period">Number of days</param>
        /// <param name="direction">Direction of the offer</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexOffer>> NewOfferAsync(string currency, decimal amount, decimal rate, int period, FundingType direction)
        {
            var parameters = new Dictionary<string, object>
            {
                { "currency", currency },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) },
                { "rate", rate.ToString(CultureInfo.InvariantCulture) },
                { "period", period },
                { "direction", JsonConvert.SerializeObject(direction, new FundingTypeConverter(false)) },
            };
            return await ExecuteRequest<BitfinexOffer>(GetUrl(NewOfferEndpoint, ApiVersion1), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancel an offer
        /// </summary>
        /// <param name="offerId">The id of the offer to cancel</param>
        /// <returns></returns>
        public WebCallResult<BitfinexOffer> CancelOffer(long offerId) => CancelOfferAsync(offerId).Result;

        /// <summary>
        /// Cancel an offer
        /// </summary>
        /// <param name="offerId">The id of the offer to cancel</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexOffer>> CancelOfferAsync(long offerId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "offer_id", offerId }
            };
            return await ExecuteRequest<BitfinexOffer>(GetUrl(CancelOfferEndpoint, ApiVersion1), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancel an offer
        /// </summary>
        /// <param name="offerId">The id of the offer to cancel</param>
        /// <returns></returns>
        public WebCallResult<BitfinexOffer> GetOffer(long offerId) => GetOfferAsync(offerId).Result;

        /// <summary>
        /// Cancel an offer
        /// </summary>
        /// <param name="offerId">The id of the offer to cancel</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexOffer>> GetOfferAsync(long offerId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "offer_id", offerId }
            };
            return await ExecuteRequest<BitfinexOffer>(GetUrl(GetOfferEndpoint, ApiVersion1), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Close margin funding
        /// </summary>
        /// <param name="swapId">The id to close</param>
        /// <returns></returns>
        public WebCallResult<BitfinexFundingContract> CloseMarginFunding(long swapId) => CloseMarginFundingAsync(swapId).Result;

        /// <summary>
        /// Close margin funding
        /// </summary>
        /// <param name="swapId">The id to close</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexFundingContract>> CloseMarginFundingAsync(long swapId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "swap_id", swapId }
            };
            return await ExecuteRequest<BitfinexFundingContract>(GetUrl(CloseMarginFundingEndpoint, ApiVersion1), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Close a position
        /// </summary>
        /// <param name="positionId">The id to close</param>
        /// <returns></returns>
        public WebCallResult<BitfinexClosePositionResult> ClosePosition(long positionId) => ClosePositionAsync(positionId).Result;

        /// <summary>
        /// Close a position
        /// </summary>
        /// <param name="positionId">The id to close</param>
        /// <returns></returns>
        public async Task<WebCallResult<BitfinexClosePositionResult>> ClosePositionAsync(long positionId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "position_id", positionId }
            };
            return await ExecuteRequest<BitfinexClosePositionResult>(GetUrl(ClosePositionEndpoint, ApiVersion1), Constants.PostMethod, parameters, true).ConfigureAwait(false);
        }
        #endregion

        #region private methods

        protected override Error ParseErrorResponse(JToken data)
        {
            if (!(data is JArray))
            {
                if(data["error"] != null && data["code"] != null && data["error_description"] != null)
                    return new ServerError((int)data["code"], data["error"] + ": " +data["error_description"]);
                if(data["message"] != null)
                    return new ServerError(-1, data["message"].ToString());
                else
                    return new ServerError(-1, data.ToString());
            }

            var error = data.ToObject<BitfinexError>();
            return new ServerError(error.ErrorCode, error.ErrorMessage);

        }

        private Uri GetUrl(string endpoint, string version)
        {
            var result = $"{BaseAddress}/v{version}/{endpoint}";
            return new Uri(result);
        }
        #endregion
        #endregion
    }
}
