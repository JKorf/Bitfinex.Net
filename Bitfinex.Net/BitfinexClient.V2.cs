using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Bitfinex.Net.Converters;
using Bitfinex.Net.Errors;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net
{
    public partial class BitfinexClient
    {
        #region fields
        private const string StatusEndpoint = "platform/status";
        private const string TickersEndpoint = "tickers";
        private const string TradesEndpoint = "trades/{}/hist";
        private const string OrderBookEndpoint = "book/{}/{}";
        private const string StatsEndpoint = "stats1/{}:1m:{}:{}/{}";
        private const string LastCandleEndpoint = "candles/trade:{}:{}/last";
        private const string CandlesEndpoint = "candles/trade:{}:{}/hist";
        private const string MarketAverageEndpoint = "calc/trade/avg";

        private const string WalletsEndpoint = "auth/r/wallets";
        private const string OpenOrdersEndpoint = "auth/r/orders";
        private const string OrderHistoryEndpoint = "auth/r/orders/{}/hist";
        private const string OrderTradesEndpoint = "auth/r/order/{}:{}/trades";
        private const string MyTradesEndpoint = "auth/r/trades/{}/hist";

        private const string ActivePositionsEndpoint = "auth/r/positions";
        private const string ActiveFundingOffersEndpoint = "auth/r/funding/offers/{}";
        private const string FundingOfferHistoryEndpoint = "auth/r/funding/offers/{}/hist";
        private const string FundingLoansEndpoint = "auth/r/funding/loans/{}";
        private const string FundingLoansHistoryEndpoint = "auth/r/funding/loans/{}/hist";
        private const string FundingCreditsEndpoint = "auth/r/funding/credits/{}";
        private const string FundingCreditsHistoryEndpoint = "auth/r/funding/credits/{}/hist";
        private const string FundingTradesEndpoint = "auth/r/funding/trades/{}/hist";
        private const string MaginInfoBaseEndpoint = "auth/r/info/margin/base";
        private const string MaginInfoSymbolEndpoint = "auth/r/info/margin/{}";
        private const string FundingInfoEndpoint = "auth/r/info/funding/{}";

        private const string MovementsEndpoint = "auth/r/movements/{}/hist";
        private const string DailyPerformanceEndpoint = "auth/r/stats/perf:1D/hist";

        private const string AlertListEndpoint = "auth/r/alerts";
        private const string SetAlertEndpoint = "auth/w/alert/set";
        private const string DeleteAlertEndpoint = "auth/w/alert/price:{}:{}/del";

        #endregion

        #region methods
        public BitfinexApiResult<BitfinexPlatformStatus> GetPlatformStatus() => GetPlatformStatusAsync().Result;
        public async Task<BitfinexApiResult<BitfinexPlatformStatus>> GetPlatformStatusAsync()
        {
            return await ExecutePublicRequest<BitfinexPlatformStatus>(GetUrl(StatusEndpoint, NewApiVersion), GetMethod);
        }

        public BitfinexApiResult<BitfinexMarketOverview[]> GetTicker(params string[] markets) => GetTickerAsync(markets).Result;
        public async Task<BitfinexApiResult<BitfinexMarketOverview[]>> GetTickerAsync(params string[] markets)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"symbols", string.Join(",", markets)}
            };

            return await ExecutePublicRequest<BitfinexMarketOverview[]>(GetUrl(TickersEndpoint, NewApiVersion, parameters), GetMethod);
        }

        public BitfinexApiResult<BitfinexTradeSimple[]> GetTrades(string market, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null) => GetTradesAsync(market, limit, startTime, endTime, sorting).Result;
        public async Task<BitfinexApiResult<BitfinexTradeSimple[]>> GetTradesAsync(string market, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null)
        {
            var parameters = new Dictionary<string, string>();
            AddOptionalParameter(parameters, "limit", limit?.ToString());
            AddOptionalParameter(parameters, "start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            AddOptionalParameter(parameters, "end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);
            AddOptionalParameter(parameters, "sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            return await ExecutePublicRequest<BitfinexTradeSimple[]>(GetUrl(FillPathParameter(TradesEndpoint, market), NewApiVersion, parameters), GetMethod);
        }

        public BitfinexApiResult<BitfinexOrderBookEntry[]> GetOrderBook(string symbol, Precision precision, int? limit = null) => GetOrderBookAsync(symbol,  precision, limit).Result;
        public async Task<BitfinexApiResult<BitfinexOrderBookEntry[]>> GetOrderBookAsync(string symbol, Precision precision, int? limit = null)
        {
            if (limit != null && (limit != 25 && limit != 100))
                return ThrowErrorMessage<BitfinexOrderBookEntry[]>(BitfinexErrors.GetError(BitfinexErrorKey.InputValidationFailed), "Limit can only be 25 or 100 for the order book");

            var parameters = new Dictionary<string, string>();
            AddOptionalParameter(parameters, "len", limit?.ToString());

            return await ExecutePublicRequest<BitfinexOrderBookEntry[]>(GetUrl(FillPathParameter(OrderBookEndpoint, symbol, precision.ToString()), NewApiVersion, parameters), GetMethod);
        }

        public BitfinexApiResult<BitfinexStats> GetStats(string symbol, StatKey key, StatSide side, StatSection section, Sorting? sorting = null) => GetStatsAsync(symbol, key, side, section, sorting).Result;
        public async Task<BitfinexApiResult<BitfinexStats>> GetStatsAsync(string symbol, StatKey key, StatSide side, StatSection section, Sorting? sorting)
        {
            // TODO
            var parameters = new Dictionary<string, string>();
            AddOptionalParameter(parameters, "sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            var endpoint = FillPathParameter(StatsEndpoint,
                JsonConvert.SerializeObject(key, new StatKeyConverter(false)),
                symbol,
                JsonConvert.SerializeObject(side, new StatSideConverter(false)),
                JsonConvert.SerializeObject(section, new StatSectionConverter(false)));

            return await ExecutePublicRequest<BitfinexStats>(GetUrl(endpoint, NewApiVersion, parameters), GetMethod);
        }

        public BitfinexApiResult<BitfinexCandle> GetLastCandle(TimeFrame timeFrame, string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null)
            => GetLastCandleAsync(timeFrame, symbol, limit, startTime, endTime, sorting).Result;
        public async Task<BitfinexApiResult<BitfinexCandle>> GetLastCandleAsync(TimeFrame timeFrame, string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null)
        {
            var parameters = new Dictionary<string, string>();
            AddOptionalParameter(parameters, "len", limit?.ToString());
            AddOptionalParameter(parameters, "start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            AddOptionalParameter(parameters, "end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);
            AddOptionalParameter(parameters, "sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            var endpoint = FillPathParameter(LastCandleEndpoint,
                JsonConvert.SerializeObject(timeFrame, new TimeFrameConverter(false)),
                symbol);

            return await ExecutePublicRequest<BitfinexCandle>(GetUrl(endpoint, NewApiVersion, parameters), GetMethod);
        }

        public BitfinexApiResult<BitfinexCandle[]> GetCandles(TimeFrame timeFrame, string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null) 
            => GetCandlesAsync(timeFrame, symbol, limit, startTime, endTime, sorting).Result;
        public async Task<BitfinexApiResult<BitfinexCandle[]>> GetCandlesAsync(TimeFrame timeFrame, string symbol,int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null)
        {
            var parameters = new Dictionary<string, string>();
            AddOptionalParameter(parameters, "len", limit?.ToString());
            AddOptionalParameter(parameters, "start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            AddOptionalParameter(parameters, "end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);
            AddOptionalParameter(parameters, "sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            var endpoint = FillPathParameter(CandlesEndpoint,
                JsonConvert.SerializeObject(timeFrame, new TimeFrameConverter(false)),
                symbol);

            return await ExecutePublicRequest<BitfinexCandle[]>(GetUrl(endpoint, NewApiVersion, parameters), GetMethod);
        }

        public BitfinexApiResult<BitfinexMarketAveragePrice> GetMarketAveragePrice(string symbol, double amount, double rateLimit, int? period = null) => GetMarketAveragePriceAsync(symbol, amount, rateLimit, period).Result;
        public async Task<BitfinexApiResult<BitfinexMarketAveragePrice>> GetMarketAveragePriceAsync(string symbol, double amount, double rateLimit, int? period = null)
        {
            var parameters = new Dictionary<string, string>()
            {
                { "symbol", symbol },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) },
                { "rate_limit", rateLimit.ToString(CultureInfo.InvariantCulture) },
            };
            AddOptionalParameter(parameters, "period", period?.ToString());

            return await ExecutePublicRequest<BitfinexMarketAveragePrice>(GetUrl(MarketAverageEndpoint, NewApiVersion, parameters), PostMethod);
        }

        public BitfinexApiResult<BitfinexWallet[]> GetWallets() => GetWalletsAsync().Result;
        public async Task<BitfinexApiResult<BitfinexWallet[]>> GetWalletsAsync()
        {
            if (string.IsNullOrEmpty(apiKey) || encryptor == null)
                return ThrowErrorMessage<BitfinexWallet[]>(BitfinexErrors.GetError(BitfinexErrorKey.NoApiCredentialsProvided));

            return await ExecuteAuthenticatedRequestV2<BitfinexWallet[]>(GetUrl(WalletsEndpoint, NewApiVersion), PostMethod);
        }

        public BitfinexApiResult<BitfinexOrder[]> GetActiveOrders() => GetActiveOrdersAsync().Result;
        public async Task<BitfinexApiResult<BitfinexOrder[]>> GetActiveOrdersAsync()
        {
            if (string.IsNullOrEmpty(apiKey) || encryptor == null)
                return ThrowErrorMessage<BitfinexOrder[]>(BitfinexErrors.GetError(BitfinexErrorKey.NoApiCredentialsProvided));

            return await ExecuteAuthenticatedRequestV2<BitfinexOrder[]>(GetUrl(OpenOrdersEndpoint, NewApiVersion), PostMethod);
        }

        public BitfinexApiResult<BitfinexOrder[]> GetOrderHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetOrderHistoryAsync(symbol, startTime, endTime, limit).Result;
        public async Task<BitfinexApiResult<BitfinexOrder[]>> GetOrderHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            if (string.IsNullOrEmpty(apiKey) || encryptor == null)
                return ThrowErrorMessage<BitfinexOrder[]>(BitfinexErrors.GetError(BitfinexErrorKey.NoApiCredentialsProvided));

            var parameters = new Dictionary<string, string>();
            AddOptionalParameter(parameters, "len", limit?.ToString());
            AddOptionalParameter(parameters, "start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            AddOptionalParameter(parameters, "end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);

            return await ExecuteAuthenticatedRequestV2<BitfinexOrder[]>(GetUrl(FillPathParameter(OrderHistoryEndpoint, symbol), NewApiVersion), PostMethod, parameters);
        }

        public BitfinexApiResult<BitfinexOrder[]> GetTradesForOrder(string symbol, long orderId) => GetTradesForOrderAsync(symbol, orderId).Result;
        public async Task<BitfinexApiResult<BitfinexOrder[]>> GetTradesForOrderAsync(string symbol, long orderId)
        {
            if (string.IsNullOrEmpty(apiKey) || encryptor == null)
                return ThrowErrorMessage<BitfinexOrder[]>(BitfinexErrors.GetError(BitfinexErrorKey.NoApiCredentialsProvided));

            return await ExecuteAuthenticatedRequestV2<BitfinexOrder[]>(GetUrl(FillPathParameter(OrderTradesEndpoint, symbol, orderId.ToString()), NewApiVersion), PostMethod);
        }

        public BitfinexApiResult<BitfinexOrder[]> GetTradeHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetTradeHistoryAsync(symbol, startTime, endTime, limit).Result;
        public async Task<BitfinexApiResult<BitfinexOrder[]>> GetTradeHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            if (string.IsNullOrEmpty(apiKey) || encryptor == null)
                return ThrowErrorMessage<BitfinexOrder[]>(BitfinexErrors.GetError(BitfinexErrorKey.NoApiCredentialsProvided));

            var parameters = new Dictionary<string, string>();
            AddOptionalParameter(parameters, "len", limit?.ToString());
            AddOptionalParameter(parameters, "start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            AddOptionalParameter(parameters, "end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);

            return await ExecuteAuthenticatedRequestV2<BitfinexOrder[]>(GetUrl(FillPathParameter(MyTradesEndpoint, symbol), NewApiVersion), PostMethod, parameters);
        }

        public BitfinexApiResult<BitfinexPosition[]> GetActivePositions() => GetActivePositionsAsync().Result;
        public async Task<BitfinexApiResult<BitfinexPosition[]>> GetActivePositionsAsync()
        {
            if (string.IsNullOrEmpty(apiKey) || encryptor == null)
                return ThrowErrorMessage<BitfinexPosition[]>(BitfinexErrors.GetError(BitfinexErrorKey.NoApiCredentialsProvided));

            return await ExecuteAuthenticatedRequestV2<BitfinexPosition[]>(GetUrl(ActivePositionsEndpoint, NewApiVersion), PostMethod);
        }

        public BitfinexApiResult<BitfinexFundingOffer[]> GetActiveFundingOffers(string symbol) => GetActiveFundingOffersAsync(symbol).Result;
        public async Task<BitfinexApiResult<BitfinexFundingOffer[]>> GetActiveFundingOffersAsync(string symbol)
        {
            if (string.IsNullOrEmpty(apiKey) || encryptor == null)
                return ThrowErrorMessage<BitfinexFundingOffer[]>(BitfinexErrors.GetError(BitfinexErrorKey.NoApiCredentialsProvided));

            return await ExecuteAuthenticatedRequestV2<BitfinexFundingOffer[]>(GetUrl(FillPathParameter(ActiveFundingOffersEndpoint, symbol), NewApiVersion), PostMethod);
        }

        public BitfinexApiResult<BitfinexFundingOffer[]> GetFundingOfferHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetFundingOfferHistoryAsync(symbol, startTime, endTime, limit).Result;
        public async Task<BitfinexApiResult<BitfinexFundingOffer[]>> GetFundingOfferHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            if (string.IsNullOrEmpty(apiKey) || encryptor == null)
                return ThrowErrorMessage<BitfinexFundingOffer[]>(BitfinexErrors.GetError(BitfinexErrorKey.NoApiCredentialsProvided));

            var parameters = new Dictionary<string, string>();
            AddOptionalParameter(parameters, "len", limit?.ToString());
            AddOptionalParameter(parameters, "start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            AddOptionalParameter(parameters, "end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);

            return await ExecuteAuthenticatedRequestV2<BitfinexFundingOffer[]>(GetUrl(FillPathParameter(FundingOfferHistoryEndpoint, symbol), NewApiVersion), PostMethod, parameters);
        }

        public BitfinexApiResult<BitfinexFundingLoan[]> GetFundingLoans(string symbol) => GetFundingLoansAsync(symbol).Result;
        public async Task<BitfinexApiResult<BitfinexFundingLoan[]>> GetFundingLoansAsync(string symbol)
        {
            if (string.IsNullOrEmpty(apiKey) || encryptor == null)
                return ThrowErrorMessage<BitfinexFundingLoan[]>(BitfinexErrors.GetError(BitfinexErrorKey.NoApiCredentialsProvided));

            return await ExecuteAuthenticatedRequestV2<BitfinexFundingLoan[]>(GetUrl(FillPathParameter(FundingLoansEndpoint, symbol), NewApiVersion), PostMethod);
        }

        public BitfinexApiResult<BitfinexFundingLoan[]> GetFundingLoansHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetFundingLoansHistoryAsync(symbol, startTime, endTime, limit).Result;
        public async Task<BitfinexApiResult<BitfinexFundingLoan[]>> GetFundingLoansHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            if (string.IsNullOrEmpty(apiKey) || encryptor == null)
                return ThrowErrorMessage<BitfinexFundingLoan[]>(BitfinexErrors.GetError(BitfinexErrorKey.NoApiCredentialsProvided));

            var parameters = new Dictionary<string, string>();
            AddOptionalParameter(parameters, "len", limit?.ToString());
            AddOptionalParameter(parameters, "start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            AddOptionalParameter(parameters, "end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);

            return await ExecuteAuthenticatedRequestV2<BitfinexFundingLoan[]>(GetUrl(FillPathParameter(FundingLoansHistoryEndpoint, symbol), NewApiVersion), PostMethod, parameters);
        }

        public BitfinexApiResult<BitfinexFundingCredit[]> GetFundingCredits(string symbol) => GetFundingCreditsAsync(symbol).Result;
        public async Task<BitfinexApiResult<BitfinexFundingCredit[]>> GetFundingCreditsAsync(string symbol)
        {
            if (string.IsNullOrEmpty(apiKey) || encryptor == null)
                return ThrowErrorMessage<BitfinexFundingCredit[]>(BitfinexErrors.GetError(BitfinexErrorKey.NoApiCredentialsProvided));

            return await ExecuteAuthenticatedRequestV2<BitfinexFundingCredit[]>(GetUrl(FillPathParameter(FundingCreditsEndpoint, symbol), NewApiVersion), PostMethod);
        }

        public BitfinexApiResult<BitfinexFundingCredit[]> GetFundingCreditsHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetFundingCreditsHistoryAsyncTask(symbol, startTime, endTime, limit).Result;
        public async Task<BitfinexApiResult<BitfinexFundingCredit[]>> GetFundingCreditsHistoryAsyncTask(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            if (string.IsNullOrEmpty(apiKey) || encryptor == null)
                return ThrowErrorMessage<BitfinexFundingCredit[]>(BitfinexErrors.GetError(BitfinexErrorKey.NoApiCredentialsProvided));

            var parameters = new Dictionary<string, string>();
            AddOptionalParameter(parameters, "len", limit?.ToString());
            AddOptionalParameter(parameters, "start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            AddOptionalParameter(parameters, "end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);

            return await ExecuteAuthenticatedRequestV2<BitfinexFundingCredit[]>(GetUrl(FillPathParameter(FundingCreditsHistoryEndpoint, symbol), NewApiVersion), PostMethod, parameters);
        }

        public BitfinexApiResult<BitfinexFundingCredit[]> GetFundingTradesHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetFundingTradesHistoryAsync(symbol, startTime, endTime, limit).Result;
        public async Task<BitfinexApiResult<BitfinexFundingCredit[]>> GetFundingTradesHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            if (string.IsNullOrEmpty(apiKey) || encryptor == null)
                return ThrowErrorMessage<BitfinexFundingCredit[]>(BitfinexErrors.GetError(BitfinexErrorKey.NoApiCredentialsProvided));

            var parameters = new Dictionary<string, string>();
            AddOptionalParameter(parameters, "len", limit?.ToString());
            AddOptionalParameter(parameters, "start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            AddOptionalParameter(parameters, "end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);

            return await ExecuteAuthenticatedRequestV2<BitfinexFundingCredit[]>(GetUrl(FillPathParameter(FundingTradesEndpoint, symbol), NewApiVersion), PostMethod);
        }

        public BitfinexApiResult<BitfinexMarginBase> GetBaseMarginInfo() => GetBaseMarginInfoAsync().Result;
        public async Task<BitfinexApiResult<BitfinexMarginBase>> GetBaseMarginInfoAsync()
        {
            if (string.IsNullOrEmpty(apiKey) || encryptor == null)
                return ThrowErrorMessage<BitfinexMarginBase>(BitfinexErrors.GetError(BitfinexErrorKey.NoApiCredentialsProvided));

            return await ExecuteAuthenticatedRequestV2<BitfinexMarginBase>(GetUrl(MaginInfoBaseEndpoint, NewApiVersion), PostMethod);
        }

        public BitfinexApiResult<BitfinexMarginSymbol> GetSymbolMarginInfo(string symbol) => GetSymbolMarginInfoAsync(symbol).Result;
        public async Task<BitfinexApiResult<BitfinexMarginSymbol>> GetSymbolMarginInfoAsync(string symbol)
        {
            if (string.IsNullOrEmpty(apiKey) || encryptor == null)
                return ThrowErrorMessage<BitfinexMarginSymbol>(BitfinexErrors.GetError(BitfinexErrorKey.NoApiCredentialsProvided));

            return await ExecuteAuthenticatedRequestV2<BitfinexMarginSymbol>(GetUrl(FillPathParameter(MaginInfoSymbolEndpoint, symbol), NewApiVersion), PostMethod);
        }

        public BitfinexApiResult<BitfinexFundingInfo> GetFundingInfo(string symbol) => GetFundingInfoAsync(symbol).Result;
        public async Task<BitfinexApiResult<BitfinexFundingInfo>> GetFundingInfoAsync(string symbol)
        {
            if (string.IsNullOrEmpty(apiKey) || encryptor == null)
                return ThrowErrorMessage<BitfinexFundingInfo>(BitfinexErrors.GetError(BitfinexErrorKey.NoApiCredentialsProvided));

            return await ExecuteAuthenticatedRequestV2<BitfinexFundingInfo>(GetUrl(FillPathParameter(FundingInfoEndpoint, symbol), NewApiVersion), PostMethod);
        }

        public BitfinexApiResult<object> GetMovements(string symbol) => GetMovementsAsync(symbol).Result;
        public async Task<BitfinexApiResult<object>> GetMovementsAsync(string symbol)
        {
            if (string.IsNullOrEmpty(apiKey) || encryptor == null)
                return ThrowErrorMessage<object>(BitfinexErrors.GetError(BitfinexErrorKey.NoApiCredentialsProvided));

            // TODO what is the result of this?
            return await ExecuteAuthenticatedRequestV2<object>(GetUrl(FillPathParameter(MovementsEndpoint, symbol), NewApiVersion), PostMethod);
        }

        public BitfinexApiResult<BitfinexPerformance> GetDailyPerformance() => GetDailyPerformanceAsync().Result;
        public async Task<BitfinexApiResult<BitfinexPerformance>> GetDailyPerformanceAsync()
        {
            if (string.IsNullOrEmpty(apiKey) || encryptor == null)
                return ThrowErrorMessage<BitfinexPerformance>(BitfinexErrors.GetError(BitfinexErrorKey.NoApiCredentialsProvided));

            // TODO doesn't work?
            return await ExecuteAuthenticatedRequestV2<BitfinexPerformance>(GetUrl(DailyPerformanceEndpoint, NewApiVersion), PostMethod);
        }

        public BitfinexApiResult<BitfinexAlert[]> GetAlertList() => GetAlertListAsync().Result;
        public async Task<BitfinexApiResult<BitfinexAlert[]>> GetAlertListAsync()
        {
            if (string.IsNullOrEmpty(apiKey) || encryptor == null)
                return ThrowErrorMessage<BitfinexAlert[]>(BitfinexErrors.GetError(BitfinexErrorKey.NoApiCredentialsProvided));

            var parameters = new Dictionary<string, string>()
            {
                { "type", "price" } 
            };

            return await ExecuteAuthenticatedRequestV2<BitfinexAlert[]>(GetUrl(AlertListEndpoint, NewApiVersion), PostMethod, parameters);
        }

        public BitfinexApiResult<BitfinexAlert> SetAlert(string symbol, double price) => SetAlertAsync(symbol, price).Result;
        public async Task<BitfinexApiResult<BitfinexAlert>> SetAlertAsync(string symbol, double price)
        {
            if (string.IsNullOrEmpty(apiKey) || encryptor == null)
                return ThrowErrorMessage<BitfinexAlert>(BitfinexErrors.GetError(BitfinexErrorKey.NoApiCredentialsProvided));

            var parameters = new Dictionary<string, string>()
            {
                { "type", "price" },
                { "symbol", symbol },
                { "price", price.ToString(CultureInfo.InvariantCulture) }
            };

            return await ExecuteAuthenticatedRequestV2<BitfinexAlert>(GetUrl(SetAlertEndpoint, NewApiVersion), PostMethod, parameters);
        }

        public BitfinexApiResult<BitfinexSuccessResult> DeleteAlert(string symbol, double price) => DeleteAlertAsync(symbol, price).Result;
        public async Task<BitfinexApiResult<BitfinexSuccessResult>> DeleteAlertAsync(string symbol, double price)
        {
            if (string.IsNullOrEmpty(apiKey) || encryptor == null)
                return ThrowErrorMessage<BitfinexSuccessResult>(BitfinexErrors.GetError(BitfinexErrorKey.NoApiCredentialsProvided));

            return await ExecuteAuthenticatedRequestV2<BitfinexSuccessResult>(GetUrl(FillPathParameter(DeleteAlertEndpoint, symbol, price.ToString(CultureInfo.InvariantCulture)), NewApiVersion), PostMethod);
        }
        #endregion
    }
}
