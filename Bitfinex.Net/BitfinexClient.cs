using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
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
        private const string ApiVersion = "2";

        private const string StatusEndpoint = "platform/status";
        private const string TickersEndpoint = "tickers";
        private const string TradesEndpoint = "trades/{}/hist";
        private const string OrderBookEndpoint = "book/{}/{}";
        private const string StatsEndpoint = "stats1/{}:1m:{}:{}/{}";
        private const string LastCandleEndpoint = "candles/trade:{}:{}/last";
        private const string CandlesEndpoint = "candles/trade:{}:{}/hist";
        private const string MarketAverageEndpoint = "calc/trade/avg";
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
        public BitfinexApiResult<BitfinexPlatformStatus> GetPlatformStatus() => GetPlatformStatusAsync().Result;
        public async Task<BitfinexApiResult<BitfinexPlatformStatus>> GetPlatformStatusAsync()
        {
            return await ExecuteRequest<BitfinexPlatformStatus>(GetUrl(StatusEndpoint, ApiVersion), GetMethod);
        }

        public BitfinexApiResult<BitfinexMarketOverview[]> GetTicker(params string[] markets) => GetTickerAsync(markets).Result;
        public async Task<BitfinexApiResult<BitfinexMarketOverview[]>> GetTickerAsync(params string[] markets)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"symbols", string.Join(",", markets)}
            };

            return await ExecuteRequest<BitfinexMarketOverview[]>(GetUrl(TickersEndpoint, ApiVersion, parameters), GetMethod);
        }

        public BitfinexApiResult<BitfinexTrade[]> GetTrades(string market, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null) => GetTradesAsync(market, limit, startTime, endTime, sorting).Result;
        public async Task<BitfinexApiResult<BitfinexTrade[]>> GetTradesAsync(string market, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null)
        {
            var parameters = new Dictionary<string, string>();
            AddOptionalParameter(parameters, "limit", limit?.ToString());
            AddOptionalParameter(parameters, "start", startTime != null ? JsonConvert.SerializeObject(startTime, new TimestampConverter(false)) : null);
            AddOptionalParameter(parameters, "end", endTime != null ? JsonConvert.SerializeObject(endTime, new TimestampConverter(false)) : null);
            AddOptionalParameter(parameters, "sort", sorting != null ? JsonConvert.SerializeObject(sorting, new SortingConverter(false)) : null);

            return await ExecuteRequest<BitfinexTrade[]>(GetUrl(FillPathParameter(TradesEndpoint, market), ApiVersion, parameters), GetMethod);
        }

        public BitfinexApiResult<BitfinexOrderBookEntry[]> GetOrderBook(string symbol, Precision precision, int? limit = null) => GetOrderBookAsync(symbol,  precision, limit).Result;
        public async Task<BitfinexApiResult<BitfinexOrderBookEntry[]>> GetOrderBookAsync(string symbol, Precision precision, int? limit = null)
        {
            if (limit != 25 && limit != 100)
                return ThrowErrorMessage<BitfinexOrderBookEntry[]>("Limit can only be 25 or 100 for the order book");

            var parameters = new Dictionary<string, string>();
            AddOptionalParameter(parameters, "len", limit?.ToString());

            return await ExecuteRequest<BitfinexOrderBookEntry[]>(GetUrl(FillPathParameter(OrderBookEndpoint, symbol, precision.ToString()), ApiVersion, parameters), GetMethod);
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

            return await ExecuteRequest<BitfinexStats>(GetUrl(endpoint, ApiVersion, parameters), GetMethod);
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

            return await ExecuteRequest<BitfinexCandle>(GetUrl(endpoint, ApiVersion, parameters), GetMethod);
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

            return await ExecuteRequest<BitfinexCandle[]>(GetUrl(endpoint, ApiVersion, parameters), GetMethod);
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

            return await ExecuteRequest<BitfinexMarketAveragePrice>(GetUrl(MarketAverageEndpoint, ApiVersion, parameters), PostMethod);
        }

        #region private
        private async Task<BitfinexApiResult<T>> ExecuteRequest<T>(Uri uri, string httpMethod, bool signed = false)
        {
            string returnedData = "";
            try
            {
                var uriString = uri.ToString();
                if (signed)
                {
                }

                var request = RequestFactory.Create(uriString);
                request.Method = httpMethod;
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
            var result = $"{BaseAddress}/v{version}/{endpoint}?";
            if (parameters != null && parameters.Count > 0)
                result += $"{string.Join("&", parameters.Select(s => $"{s.Key}={s.Value}"))}";
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
                sbinary += t.ToString("X2"); /* hex format */
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
