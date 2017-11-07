using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Bitfinex.Net.Converters;
using Bitfinex.Net.Implementations;
using Bitfinex.Net.Interfaces;
using Bitfinex.Net.Logging;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net
{
    public class BitfinexClient : BitfinexAbstractClient, IDisposable
    {
        #region fields

        private const string GetMethod = "GET";
        private const string PostMethod = "POST";

        private const string BaseAddress = "https://api.bitfinex.com";
        private const string NewApiVersion = "2";
        private const string OldApiVersion = "1";

        private const string SymbolsEndpoint = "symbols";
        private const string SymbolDetailsEndpoint = "symbols_details";

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

        private string nonce => Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds * 10).ToString();
        #endregion

        #region properties
        public IRequestFactory RequestFactory = new RequestFactory();
        #endregion

        #region constructor/destructor
        public BitfinexClient()
        {
        }

        public BitfinexClient(string apiKey, string apiSecret)
        {
            SetApiCredentials(apiKey, apiSecret);
        }

        ~BitfinexClient()
        {
            Dispose(false);
        }
        #endregion

        #region methods
        public BitfinexApiResult<string[]> GetSymbols() => GetSymbolsAsync().Result;
        public async Task<BitfinexApiResult<string[]>> GetSymbolsAsync()
        {
            return await ExecuteRequest<string[]>(GetUrl(SymbolsEndpoint, OldApiVersion), GetMethod);
        }

        public BitfinexApiResult<BitfinexSymbol[]> GetSymbolDetails() => GetSymbolDetailsAsync().Result;
        public async Task<BitfinexApiResult<BitfinexSymbol[]>> GetSymbolDetailsAsync()
        {
            return await ExecuteRequest<BitfinexSymbol[]>(GetUrl(SymbolDetailsEndpoint, OldApiVersion), GetMethod);
        }

        public BitfinexApiResult<BitfinexPlatformStatus> GetPlatformStatus() => GetPlatformStatusAsync().Result;
        public async Task<BitfinexApiResult<BitfinexPlatformStatus>> GetPlatformStatusAsync()
        {
            return await ExecuteRequest<BitfinexPlatformStatus>(GetUrl(StatusEndpoint, NewApiVersion), GetMethod);
        }

        public BitfinexApiResult<BitfinexMarketOverview[]> GetTicker(params string[] markets) => GetTickerAsync(markets).Result;
        public async Task<BitfinexApiResult<BitfinexMarketOverview[]>> GetTickerAsync(params string[] markets)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"symbols", string.Join(",", markets)}
            };

            return await ExecuteRequest<BitfinexMarketOverview[]>(GetUrl(TickersEndpoint, NewApiVersion, parameters), GetMethod);
        }

        public BitfinexApiResult<BitfinexTradeSimple[]> GetTrades(string market, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null) => GetTradesAsync(market, limit, startTime, endTime, sorting).Result;
        public async Task<BitfinexApiResult<BitfinexTradeSimple[]>> GetTradesAsync(string market, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null)
        {
            var parameters = new Dictionary<string, string>();
            AddOptionalParameter(parameters, "limit", limit?.ToString());
            AddOptionalParameter(parameters, "start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            AddOptionalParameter(parameters, "end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);
            AddOptionalParameter(parameters, "sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            return await ExecuteRequest<BitfinexTradeSimple[]>(GetUrl(FillPathParameter(TradesEndpoint, market), NewApiVersion, parameters), GetMethod);
        }

        public BitfinexApiResult<BitfinexOrderBookEntry[]> GetOrderBook(string symbol, Precision precision, int? limit = null) => GetOrderBookAsync(symbol,  precision, limit).Result;
        public async Task<BitfinexApiResult<BitfinexOrderBookEntry[]>> GetOrderBookAsync(string symbol, Precision precision, int? limit = null)
        {
            if (limit != 25 && limit != 100)
                return ThrowErrorMessage<BitfinexOrderBookEntry[]>("Limit can only be 25 or 100 for the order book");

            var parameters = new Dictionary<string, string>();
            AddOptionalParameter(parameters, "len", limit?.ToString());

            return await ExecuteRequest<BitfinexOrderBookEntry[]>(GetUrl(FillPathParameter(OrderBookEndpoint, symbol, precision.ToString()), NewApiVersion, parameters), GetMethod);
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

            return await ExecuteRequest<BitfinexStats>(GetUrl(endpoint, NewApiVersion, parameters), GetMethod);
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

            return await ExecuteRequest<BitfinexCandle>(GetUrl(endpoint, NewApiVersion, parameters), GetMethod);
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

            return await ExecuteRequest<BitfinexCandle[]>(GetUrl(endpoint, NewApiVersion, parameters), GetMethod);
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

            return await ExecuteRequest<BitfinexMarketAveragePrice>(GetUrl(MarketAverageEndpoint, NewApiVersion, parameters), PostMethod);
        }

        public BitfinexApiResult<BitfinexWallet[]> GetWallets() => GetWalletsAsync().Result;
        public async Task<BitfinexApiResult<BitfinexWallet[]>> GetWalletsAsync()
        {
            return await ExecuteRequest<BitfinexWallet[]>(GetUrl(WalletsEndpoint, NewApiVersion), PostMethod, true);
        }

        public BitfinexApiResult<BitfinexOrder[]> GetActiveOrders() => GetActiveOrdersAsync().Result;
        public async Task<BitfinexApiResult<BitfinexOrder[]>> GetActiveOrdersAsync()
        {
            return await ExecuteRequest<BitfinexOrder[]>(GetUrl(OpenOrdersEndpoint, NewApiVersion), PostMethod, true);
        }

        public BitfinexApiResult<BitfinexOrder[]> GetOrderHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetOrderHistoryAsync(symbol, startTime, endTime, limit).Result;
        public async Task<BitfinexApiResult<BitfinexOrder[]>> GetOrderHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            var parameters = new Dictionary<string, string>();
            AddOptionalParameter(parameters, "len", limit?.ToString());
            AddOptionalParameter(parameters, "start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            AddOptionalParameter(parameters, "end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);

            return await ExecuteRequest<BitfinexOrder[]>(GetUrl(FillPathParameter(OrderHistoryEndpoint, symbol), NewApiVersion), PostMethod, true, parameters);
        }

        public BitfinexApiResult<BitfinexOrder[]> GetTradesForOrder(string symbol, long orderId) => GetTradesForOrderAsync(symbol, orderId).Result;
        public async Task<BitfinexApiResult<BitfinexOrder[]>> GetTradesForOrderAsync(string symbol, long orderId)
        {
            return await ExecuteRequest<BitfinexOrder[]>(GetUrl(FillPathParameter(OrderTradesEndpoint, symbol, orderId.ToString()), NewApiVersion), PostMethod, true);
        }

        public BitfinexApiResult<BitfinexOrder[]> GetTradeHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null) => GetTradeHistoryAsync(symbol, startTime, endTime, limit).Result;
        public async Task<BitfinexApiResult<BitfinexOrder[]>> GetTradeHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            var parameters = new Dictionary<string, string>();
            AddOptionalParameter(parameters, "len", limit?.ToString());
            AddOptionalParameter(parameters, "start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            AddOptionalParameter(parameters, "end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);

            return await ExecuteRequest<BitfinexOrder[]>(GetUrl(FillPathParameter(MyTradesEndpoint, symbol), NewApiVersion), PostMethod, true, parameters);
        }

        public BitfinexApiResult<BitfinexPosition[]> GetActivePositions() => GetActivePositionsAsync().Result;
        public async Task<BitfinexApiResult<BitfinexPosition[]>> GetActivePositionsAsync()
        {
            return await ExecuteRequest<BitfinexPosition[]>(GetUrl(ActivePositionsEndpoint, NewApiVersion), PostMethod, true);
        }

        public BitfinexApiResult<BitfinexFundingOffer[]> GetActiveFundingOffers(string symbol) => GetActiveFundingOffersAsync(symbol).Result;
        public async Task<BitfinexApiResult<BitfinexFundingOffer[]>> GetActiveFundingOffersAsync(string symbol)
        {
            return await ExecuteRequest<BitfinexFundingOffer[]>(GetUrl(FillPathParameter(ActiveFundingOffersEndpoint, symbol), NewApiVersion), PostMethod, true);
        }

        public BitfinexApiResult<BitfinexFundingOffer[]> GetFundingOfferHistory(string symbol) => GetFundingOfferHistoryAsync(symbol).Result;
        public async Task<BitfinexApiResult<BitfinexFundingOffer[]>> GetFundingOfferHistoryAsync(string symbol)
        {
            return await ExecuteRequest<BitfinexFundingOffer[]>(GetUrl(FillPathParameter(FundingOfferHistoryEndpoint, symbol), NewApiVersion), PostMethod, true);
        }

        public BitfinexApiResult<BitfinexFundingLoan[]> GetFundingLoans(string symbol) => GetFundingLoansAsync(symbol).Result;
        public async Task<BitfinexApiResult<BitfinexFundingLoan[]>> GetFundingLoansAsync(string symbol)
        {
            return await ExecuteRequest<BitfinexFundingLoan[]>(GetUrl(FillPathParameter(FundingLoansEndpoint, symbol), NewApiVersion), PostMethod, true);
        }

        #region private
        private async Task<BitfinexApiResult<T>> ExecuteRequest<T>(Uri uri, string httpMethod, bool signed = false, Dictionary<string, string> bodyParameters = null)
        {
            string returnedData = "";
            try
            {
                if (bodyParameters == null)
                    bodyParameters = new Dictionary<string, string>();

                var json = JsonConvert.SerializeObject(bodyParameters);
                var data = Encoding.ASCII.GetBytes(json);

                var uriString = uri.ToString();
                var request = RequestFactory.Create(uriString);
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.Method = httpMethod;
                if (signed)
                {
                    var n = nonce;
                    var signature = $"/api{uri.PathAndQuery}{n}{json}";
                    var signedData = ByteToString(encryptor.ComputeHash(Encoding.ASCII.GetBytes(signature)));
                    request.Headers.Add($"bfx-nonce: {n}");
                    request.Headers.Add($"bfx-apikey: {apiKey}");
                    request.Headers.Add($"bfx-signature: {signedData}");
                    request.ContentLength = data.Length;

                    using (var stream = request.GetRequestStream())
                        stream.Write(data, 0, data.Length);
                }

                log.Write(LogVerbosity.Debug, $"Sending request to {uriString}");
                var response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    returnedData = await reader.ReadToEndAsync();
                    return ReturnResult(JsonConvert.DeserializeObject<T>(returnedData));
                }
            }
            catch (WebException we)
            {
                var response = (HttpWebResponse)we.Response;
                if ((int)response.StatusCode >= 400)
                {
                    try
                    {
                        // Try read error response
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            var data = await reader.ReadToEndAsync();
                            var error = JsonConvert.DeserializeObject<BitfinexErrorResponse>(data);
                            return ThrowErrorMessage<T>($"{error.ErrorCode} - {error.ErrorMessage}");
                        }
                    }
                    catch (Exception)
                    {
                        log.Write(LogVerbosity.Warning, $"Couldn't parse error response for status code {response.StatusCode}");
                    }
                }

                var errorMessage =
                    $"Request to {uri} failed because of a webexception. Status: {response.StatusCode}-{response.StatusDescription}, Message: {we.Message}";
                log.Write(LogVerbosity.Warning, errorMessage);
                return ThrowErrorMessage<T>(errorMessage);
            }
            catch (JsonReaderException jre)
            {
                var errorMessage =
                    $"Request to {uri} failed, couldn't parse the returned data. Error occured at Path: {jre.Path}, LineNumber: {jre.LineNumber}, LinePosition: {jre.LinePosition}. Received data: {returnedData}";
                log.Write(LogVerbosity.Warning, errorMessage);
                return ThrowErrorMessage<T>(errorMessage);
            }
            catch (JsonSerializationException jse)
            {
                var errorMessage =
                    $"Request to {uri} failed, couldn't deserialize the returned data. Message: {jse.Message}. Received data: {returnedData}";
                log.Write(LogVerbosity.Warning, errorMessage);
                return ThrowErrorMessage<T>(errorMessage);
            }
            catch (Exception e)
            {
                var errorMessage = $"Request to {uri} failed with unknown error: " + e.Message;
                log.Write(LogVerbosity.Warning, errorMessage);
                return ThrowErrorMessage<T>(errorMessage);
            }
        }

        private Uri GetUrl(string endpoint, string version, Dictionary<string, string> parameters = null)
        {
            var result = $"{BaseAddress}/v{version}/{endpoint}";
            if (parameters != null && parameters.Count > 0)
                result += $"?{string.Join("&", parameters.Select(s => $"{s.Key}={s.Value}"))}";
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

        private void AddOptionalParameter(Dictionary<string, string> dictionary, string key, string value)
        {
            if (value != null)
                dictionary.Add(key, value);
        }

        private string ByteToString(byte[] buff)
        {
            var sbinary = "";
            foreach (byte t in buff)
                sbinary += t.ToString("x2"); /* hex format */
            return sbinary;
        }

        private void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
        #endregion
    }
}
