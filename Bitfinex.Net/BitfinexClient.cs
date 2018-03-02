using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Bitfinex.Net.Converters;
using Bitfinex.Net.Objects;
using CryptoExchange.Net;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Requests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Net
{
    public partial class BitfinexClient : ExchangeClient
    {
        #region fields

        private static BitfinexClientOptions defaultOptions = new BitfinexClientOptions();

        private const string GetMethod = "GET";
        private const string PostMethod = "POST";

        private const string BaseAddress = "https://api.bitfinex.com";
        private const string NewApiVersion = "2";

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

        private string nonce => Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds * 10).ToString(CultureInfo.InvariantCulture);
        #endregion

        #region constructor/destructor
        /// <summary>
        /// Create a new instance of BinanceClient using the default options
        /// </summary>
        public BitfinexClient(): this(defaultOptions)
        {
        }

        /// <summary>
        /// Create a new instance of BinanceClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public BitfinexClient(BitfinexClientOptions options) : base(options, options.ApiCredentials == null ? null : new BitfinexAuthenticationProvider(options.ApiCredentials))
        {
        }
        #endregion

        #region methods
        public CallResult<BitfinexPlatformStatus> GetPlatformStatus() => GetPlatformStatusAsync().Result;
        public async Task<CallResult<BitfinexPlatformStatus>> GetPlatformStatusAsync()
        {
            return await ExecuteRequest<BitfinexPlatformStatus>(GetUrl(StatusEndpoint, NewApiVersion));
        }

        public CallResult<BitfinexMarketOverview[]> GetTicker(params string[] markets) => GetTickerAsync(markets).Result;
        public async Task<CallResult<BitfinexMarketOverview[]>> GetTickerAsync(params string[] markets)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"symbols", string.Join(",", markets)}
            };

            return await ExecuteRequest<BitfinexMarketOverview[]>(GetUrl(TickersEndpoint, NewApiVersion), GetMethod, parameters);
        }
        
        public CallResult<BitfinexTradeSimple[]> GetTrades(string market, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null) => GetTradesAsync(market, limit, startTime, endTime, sorting).Result;
        public async Task<CallResult<BitfinexTradeSimple[]>> GetTradesAsync(string market, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString());
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            return await ExecuteRequest<BitfinexTradeSimple[]>(GetUrl(FillPathParameter(TradesEndpoint, market), NewApiVersion), GetMethod, parameters);
        }
        
        public CallResult<BitfinexOrderBookEntry[]> GetOrderBook(string symbol, Precision precision, int? limit = null) => GetOrderBookAsync(symbol,  precision, limit).Result;
        public async Task<CallResult<BitfinexOrderBookEntry[]>> GetOrderBookAsync(string symbol, Precision precision, int? limit = null)
        {
            if (limit != null && (limit != 25 && limit != 100))
                return new CallResult<BitfinexOrderBookEntry[]>(null, new ArgumentError("Limit should be either 25 or 100"));

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("len", limit?.ToString());

            return await ExecuteRequest<BitfinexOrderBookEntry[]>(GetUrl(FillPathParameter(OrderBookEndpoint, symbol, precision.ToString()), NewApiVersion), GetMethod, parameters);
        }
        
        public CallResult<BitfinexStats> GetStats(string symbol, StatKey key, StatSide side, StatSection section, Sorting? sorting = null) => GetStatsAsync(symbol, key, side, section, sorting).Result;
        public async Task<CallResult<BitfinexStats>> GetStatsAsync(string symbol, StatKey key, StatSide side, StatSection section, Sorting? sorting)
        {
            // TODO
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            var endpoint = FillPathParameter(StatsEndpoint,
                JsonConvert.SerializeObject(key, new StatKeyConverter(false)),
                symbol,
                JsonConvert.SerializeObject(side, new StatSideConverter(false)),
                JsonConvert.SerializeObject(section, new StatSectionConverter(false)));

            return await ExecuteRequest<BitfinexStats>(GetUrl(endpoint, NewApiVersion), GetMethod, parameters);
        }
        
        public CallResult<BitfinexCandle> GetLastCandle(TimeFrame timeFrame, string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null)
            => GetLastCandleAsync(timeFrame, symbol, limit, startTime, endTime, sorting).Result;
        public async Task<CallResult<BitfinexCandle>> GetLastCandleAsync(TimeFrame timeFrame, string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("len", limit?.ToString());
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            var endpoint = FillPathParameter(LastCandleEndpoint,
                JsonConvert.SerializeObject(timeFrame, new TimeFrameConverter(false)),
                symbol);

            return await ExecuteRequest<BitfinexCandle>(GetUrl(endpoint, NewApiVersion), GetMethod, parameters);
        }
        
        public CallResult<BitfinexCandle[]> GetCandles(TimeFrame timeFrame, string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null) 
            => GetCandlesAsync(timeFrame, symbol, limit, startTime, endTime, sorting).Result;
        public async Task<CallResult<BitfinexCandle[]>> GetCandlesAsync(TimeFrame timeFrame, string symbol,int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("len", limit?.ToString());
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);
            parameters.AddOptionalParameter("sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            var endpoint = FillPathParameter(CandlesEndpoint,
                JsonConvert.SerializeObject(timeFrame, new TimeFrameConverter(false)),
                symbol);

            return await ExecuteRequest<BitfinexCandle[]>(GetUrl(endpoint, NewApiVersion), GetMethod, parameters);
        }
        
        public CallResult<BitfinexMarketAveragePrice> GetMarketAveragePrice(string symbol, decimal amount, decimal rateLimit, int? period = null) => GetMarketAveragePriceAsync(symbol, amount, rateLimit, period).Result;
        public async Task<CallResult<BitfinexMarketAveragePrice>> GetMarketAveragePriceAsync(string symbol, decimal amount, decimal rateLimit, int? period = null)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "symbol", symbol },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) },
                { "rate_limit", rateLimit.ToString(CultureInfo.InvariantCulture) },
            };
            parameters.AddOptionalParameter("period", period?.ToString());

            return await ExecuteRequest<BitfinexMarketAveragePrice>(GetUrl(MarketAverageEndpoint, NewApiVersion), PostMethod, parameters);
        }
        
        public CallResult<BitfinexWallet[]> GetWallets() => GetWalletsAsync().Result;
        public async Task<CallResult<BitfinexWallet[]>> GetWalletsAsync()
        {
            return await ExecuteRequest<BitfinexWallet[]>(GetUrl(WalletsEndpoint, NewApiVersion), PostMethod, null, true);
        }
        public CallResult<BitfinexOrder[]> GetActiveOrders() => GetActiveOrdersAsync().Result;
        public async Task<CallResult<BitfinexOrder[]>> GetActiveOrdersAsync()
        {
            return await ExecuteRequest<BitfinexOrder[]>(GetUrl(OpenOrdersEndpoint, NewApiVersion), PostMethod, null, true);
        }

        public CallResult<BitfinexOrder[]> GetOrderHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetOrderHistoryAsync(symbol, startTime, endTime, limit).Result;
        public async Task<CallResult<BitfinexOrder[]>> GetOrderHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("len", limit?.ToString());
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);

            return await ExecuteRequest<BitfinexOrder[]>(GetUrl(FillPathParameter(OrderHistoryEndpoint, symbol), NewApiVersion), PostMethod, parameters, true);
        }

        public CallResult<BitfinexOrder[]> GetTradesForOrder(string symbol, long orderId) => GetTradesForOrderAsync(symbol, orderId).Result;
        public async Task<CallResult<BitfinexOrder[]>> GetTradesForOrderAsync(string symbol, long orderId)
        {
            return await ExecuteRequest<BitfinexOrder[]>(GetUrl(FillPathParameter(OrderTradesEndpoint, symbol, orderId.ToString()), NewApiVersion), PostMethod, null, true);
        }

        public CallResult<BitfinexOrder[]> GetTradeHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetTradeHistoryAsync(symbol, startTime, endTime, limit).Result;
        public async Task<CallResult<BitfinexOrder[]>> GetTradeHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("len", limit?.ToString());
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);

            return await ExecuteRequest<BitfinexOrder[]>(GetUrl(FillPathParameter(MyTradesEndpoint, symbol), NewApiVersion), PostMethod, parameters, true);
        }

        public CallResult<BitfinexPosition[]> GetActivePositions() => GetActivePositionsAsync().Result;
        public async Task<CallResult<BitfinexPosition[]>> GetActivePositionsAsync()
        {
            return await ExecuteRequest<BitfinexPosition[]>(GetUrl(ActivePositionsEndpoint, NewApiVersion), PostMethod, null, true);
        }

        public CallResult<BitfinexFundingOffer[]> GetActiveFundingOffers(string symbol) => GetActiveFundingOffersAsync(symbol).Result;
        public async Task<CallResult<BitfinexFundingOffer[]>> GetActiveFundingOffersAsync(string symbol)
        {
            return await ExecuteRequest<BitfinexFundingOffer[]>(GetUrl(FillPathParameter(ActiveFundingOffersEndpoint, symbol), NewApiVersion), PostMethod, null, true);
        }

        public CallResult<BitfinexFundingOffer[]> GetFundingOfferHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetFundingOfferHistoryAsync(symbol, startTime, endTime, limit).Result;
        public async Task<CallResult<BitfinexFundingOffer[]>> GetFundingOfferHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("len", limit?.ToString());
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);

            return await ExecuteRequest<BitfinexFundingOffer[]>(GetUrl(FillPathParameter(FundingOfferHistoryEndpoint, symbol), NewApiVersion), PostMethod, parameters, true);
        }

        public CallResult<BitfinexFundingLoan[]> GetFundingLoans(string symbol) => GetFundingLoansAsync(symbol).Result;
        public async Task<CallResult<BitfinexFundingLoan[]>> GetFundingLoansAsync(string symbol)
        {
            return await ExecuteRequest<BitfinexFundingLoan[]>(GetUrl(FillPathParameter(FundingLoansEndpoint, symbol), NewApiVersion), PostMethod, null, true);
        }

        public CallResult<BitfinexFundingLoan[]> GetFundingLoansHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetFundingLoansHistoryAsync(symbol, startTime, endTime, limit).Result;
        public async Task<CallResult<BitfinexFundingLoan[]>> GetFundingLoansHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("len", limit?.ToString());
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);

            return await ExecuteRequest<BitfinexFundingLoan[]>(GetUrl(FillPathParameter(FundingLoansHistoryEndpoint, symbol), NewApiVersion), PostMethod, parameters, true);
        }

        public CallResult<BitfinexFundingCredit[]> GetFundingCredits(string symbol) => GetFundingCreditsAsync(symbol).Result;
        public async Task<CallResult<BitfinexFundingCredit[]>> GetFundingCreditsAsync(string symbol)
        {
            return await ExecuteRequest<BitfinexFundingCredit[]>(GetUrl(FillPathParameter(FundingCreditsEndpoint, symbol), NewApiVersion), PostMethod, null, true);
        }

        public CallResult<BitfinexFundingCredit[]> GetFundingCreditsHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetFundingCreditsHistoryAsyncTask(symbol, startTime, endTime, limit).Result;
        public async Task<CallResult<BitfinexFundingCredit[]>> GetFundingCreditsHistoryAsyncTask(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("len", limit?.ToString());
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);

            return await ExecuteRequest<BitfinexFundingCredit[]>(GetUrl(FillPathParameter(FundingCreditsHistoryEndpoint, symbol), NewApiVersion), PostMethod, parameters, true);
        }

        public CallResult<BitfinexFundingCredit[]> GetFundingTradesHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetFundingTradesHistoryAsync(symbol, startTime, endTime, limit).Result;
        public async Task<CallResult<BitfinexFundingCredit[]>> GetFundingTradesHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("len", limit?.ToString());
            parameters.AddOptionalParameter("start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            parameters.AddOptionalParameter("end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);

            return await ExecuteRequest<BitfinexFundingCredit[]>(GetUrl(FillPathParameter(FundingTradesEndpoint, symbol), NewApiVersion), PostMethod);
        }

        public CallResult<BitfinexMarginBase> GetBaseMarginInfo() => GetBaseMarginInfoAsync().Result;
        public async Task<CallResult<BitfinexMarginBase>> GetBaseMarginInfoAsync()
        {
            return await ExecuteRequest<BitfinexMarginBase>(GetUrl(MaginInfoBaseEndpoint, NewApiVersion), PostMethod, null, true);
        }

        public CallResult<BitfinexMarginSymbol> GetSymbolMarginInfo(string symbol) => GetSymbolMarginInfoAsync(symbol).Result;
        public async Task<CallResult<BitfinexMarginSymbol>> GetSymbolMarginInfoAsync(string symbol)
        {
            return await ExecuteRequest<BitfinexMarginSymbol>(GetUrl(FillPathParameter(MaginInfoSymbolEndpoint, symbol), NewApiVersion), PostMethod, null, true);
        }

        public CallResult<BitfinexFundingInfo> GetFundingInfo(string symbol) => GetFundingInfoAsync(symbol).Result;
        public async Task<CallResult<BitfinexFundingInfo>> GetFundingInfoAsync(string symbol)
        {
            return await ExecuteRequest<BitfinexFundingInfo>(GetUrl(FillPathParameter(FundingInfoEndpoint, symbol), NewApiVersion), PostMethod, null, true);
        }

        public CallResult<BitfinexMovement[]> GetMovements(string symbol) => GetMovementsAsync(symbol).Result;
        public async Task<CallResult<BitfinexMovement[]>> GetMovementsAsync(string symbol)
        {
            return await ExecuteRequest<BitfinexMovement[]>(GetUrl(FillPathParameter(MovementsEndpoint, symbol), NewApiVersion), PostMethod, null, true);
        }

        public CallResult<BitfinexPerformance> GetDailyPerformance() => GetDailyPerformanceAsync().Result;
        public async Task<CallResult<BitfinexPerformance>> GetDailyPerformanceAsync()
        {
            // TODO doesn't work?
            return await ExecuteRequest<BitfinexPerformance>(GetUrl(DailyPerformanceEndpoint, NewApiVersion), PostMethod, null, true);
        }

        public CallResult<BitfinexAlert[]> GetAlertList() => GetAlertListAsync().Result;
        public async Task<CallResult<BitfinexAlert[]>> GetAlertListAsync()
        {
            var parameters = new Dictionary<string, object>()
            {
                { "type", "price" } 
            };

            return await ExecuteRequest<BitfinexAlert[]>(GetUrl(AlertListEndpoint, NewApiVersion), PostMethod, parameters, true);
        }

        public CallResult<BitfinexAlert> SetAlert(string symbol, decimal price) => SetAlertAsync(symbol, price).Result;
        public async Task<CallResult<BitfinexAlert>> SetAlertAsync(string symbol, decimal price)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "type", "price" },
                { "symbol", symbol },
                { "price", price.ToString(CultureInfo.InvariantCulture) }
            };

            return await ExecuteRequest<BitfinexAlert>(GetUrl(SetAlertEndpoint, NewApiVersion), PostMethod, parameters, true);
        }

        public CallResult<BitfinexSuccessResult> DeleteAlert(string symbol, decimal price) => DeleteAlertAsync(symbol, price).Result;
        public async Task<CallResult<BitfinexSuccessResult>> DeleteAlertAsync(string symbol, decimal price)
        {
            return await ExecuteRequest<BitfinexSuccessResult>(GetUrl(FillPathParameter(DeleteAlertEndpoint, symbol, price.ToString(CultureInfo.InvariantCulture)), NewApiVersion), PostMethod, null, true);
        }
        


        protected override IRequest ConstructRequest(Uri uri, string method, Dictionary<string, object> parameters, bool signed)
        {
            var uriString = uri.ToString();
            if (!signed && parameters != null)
            {
                if (!uriString.EndsWith("?"))
                    uriString += "?";

                uriString += $"{string.Join("&", parameters.Select(s => $"{s.Key}={s.Value}"))}";
            }

            var request = RequestFactory.Create(uriString);
            request.Method = method;
            request.ContentType = "application/json";
            request.Accept = "application/json";

            if (signed)
            {
                if (parameters == null)
                    parameters = new Dictionary<string, object>();

                var json = JsonConvert.SerializeObject(parameters);
                var data = Encoding.UTF8.GetBytes(json);

                var n = nonce;
                var signature = $"/api{uri.PathAndQuery}{n}{json}";
                var signedData = authProvider.Sign(signature);
                request.Headers.Add($"bfx-nonce: {n}");
                request.Headers.Add($"bfx-apikey: {authProvider.Credentials.Key}");
                request.Headers.Add($"bfx-signature: {signedData.ToLower()}");

                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);
            }

            return request;
        }

        protected override Error ParseErrorResponse(string data)
        {
            var error = JArray.Parse(data).ToObject<BitfinexError>();
            return new ServerError(error.ErrorCode, error.ErrorMessage);
        }

        private Uri GetUrl(string endpoint, string version)
        {
            var result = $"{BaseAddress}/v{version}/{endpoint}";
            return new Uri(result);
        }

        private string FillPathParameter(string endpoint, params string[] values)
        {
            foreach (var value in values)
            {
                int index = endpoint.IndexOf("{}");
                if (index >= 0)
                {
                    endpoint = endpoint.Remove(index, 2);
                    endpoint = endpoint.Insert(index, value);
                }
            }
            return endpoint;
        }
        #endregion
    }
}
