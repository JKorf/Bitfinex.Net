using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Bitfinex.Net.Errors;
using Bitfinex.Net.Implementations;
using Bitfinex.Net.Interfaces;
using Bitfinex.Net.Logging;
using Bitfinex.Net.Objects;
using Bitfinex.Net.RateLimiter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Net
{
    public partial class BitfinexClient : BitfinexAbstractClient
    {
        #region fields
        private const string GetMethod = "GET";
        private const string PostMethod = "POST";

        private const string BaseAddress = "https://api.bitfinex.com";
        private const string NewApiVersion = "2";
        private const string OldApiVersion = "1";

        private string nonce => Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds * 10).ToString(CultureInfo.InvariantCulture);
        private List<IRateLimiter> rateLimiters = new List<IRateLimiter>();
        #endregion

        #region properties
        public IRequestFactory RequestFactory = new RequestFactory();

        /// <summary>
        /// The max amount of retries to do if the Bittrex service is temporarily unavailable
        /// </summary>
        public int MaxRetries { get; set; } = 2;
        #endregion

        #region constructor/destructor
        public BitfinexClient()
        {
            if (BitfinexDefaults.MaxCallRetry != null)
                MaxRetries = BitfinexDefaults.MaxCallRetry.Value;

            foreach (var rateLimiter in BitfinexDefaults.RateLimiters)
                rateLimiters.Add(rateLimiter);
        }

        public BitfinexClient(string apiKey, string apiSecret)
        {
            if (BitfinexDefaults.MaxCallRetry != null)
                MaxRetries = BitfinexDefaults.MaxCallRetry.Value;

            foreach (var rateLimiter in BitfinexDefaults.RateLimiters)
                rateLimiters.Add(rateLimiter);

            SetApiCredentials(apiKey, apiSecret);
        }
        #endregion

        #region methods
        /// <summary>
        /// Adds a rate limiter to the client. There are 2 choices, the <see cref="RateLimiterTotal"/> and the <see cref="RateLimiterPerEndpoint"/>.
        /// </summary>
        /// <param name="limiter">The limiter to add</param>
        public void AddRateLimiter(IRateLimiter limiter)
        {
            rateLimiters.Add(limiter);
        }

        /// <summary>
        /// Removes all rate limiters from this client
        /// </summary>
        public void RemoveRateLimiters()
        {
            rateLimiters.Clear();
        }


        private async Task<BitfinexApiResult<T>> ExecutePublicRequest<T>(Uri uri, string httpMethod)
        {
            var uriString = uri.ToString();
            var request = RequestFactory.Create(uriString);
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Method = httpMethod;

            return await GetResponse<T>(request);
        }

        private async Task<BitfinexApiResult<T>> ExecuteAuthenticatedRequestV1<T>(Uri uri, string httpMethod, Dictionary<string, object> bodyParameters = null)
        {
            var uriString = uri.ToString();
            var path = uri.PathAndQuery;
            var n = nonce;

            var signature = new JObject
            {
                ["request"] = path,
                ["nonce"] = n
            };
            if(bodyParameters != null)
                foreach(var keyvalue in bodyParameters)
                    signature.Add(keyvalue.Key, JToken.FromObject(keyvalue.Value));

            var payload = Convert.ToBase64String(Encoding.ASCII.GetBytes(signature.ToString()));
            var signedData = ByteToString(encryptor.ComputeHash(Encoding.ASCII.GetBytes(payload)));

            var request = RequestFactory.Create(uriString);
            request.Accept = "application/json";
            request.Method = httpMethod;

            request.Headers.Add($"X-BFX-APIKEY: {apiKey}");
            request.Headers.Add($"X-BFX-PAYLOAD: {payload}");
            request.Headers.Add($"X-BFX-SIGNATURE: {signedData}");

            return await GetResponse<T>(request);
        }

        private async Task<BitfinexApiResult<T>> ExecuteAuthenticatedRequestV2<T>(Uri uri, string httpMethod, Dictionary<string, string> bodyParameters = null)
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

            var n = nonce;
            var signature = $"/api{uri.PathAndQuery}{n}{json}";
            var signedData = ByteToString(encryptor.ComputeHash(Encoding.ASCII.GetBytes(signature)));
            request.Headers.Add($"bfx-nonce: {n}");
            request.Headers.Add($"bfx-apikey: {apiKey}");
            request.Headers.Add($"bfx-signature: {signedData}");
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
                stream.Write(data, 0, data.Length);

            return await GetResponse<T>(request);
        }

        private async Task<BitfinexApiResult<T>> GetResponse<T>(IRequest request)
        {
            string returnedData = "";
            try
            {
                foreach (var limiter in rateLimiters)
                {
                    double limitedBy = limiter.LimitRequest(request.RequestUri.AbsolutePath);
                    if (limitedBy > 0)
                        log.Write(LogVerbosity.Debug, $"Request {request.RequestUri.AbsolutePath} was limited by {limitedBy}ms by {limiter.GetType().Name}");
                }

                log.Write(LogVerbosity.Debug, $"Sending request to {request.RequestUri}");
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
                    var error = await TryReadError(response);
                    if(error != null)
                        return ThrowErrorMessage<T>(error);
                }

                return ThrowErrorMessage<T>(BitfinexErrors.GetError(BitfinexErrorKey.ErrorWeb), $"Request to {request.RequestUri} failed because of a webexception. Status: {response.StatusCode}-{response.StatusDescription}, Message: {we.Message}");
            }
            catch (JsonReaderException jre)
            {
                return ThrowErrorMessage<T>(BitfinexErrors.GetError(BitfinexErrorKey.ParseErrorReader), $"Request to {request.RequestUri} failed, couldn't parse the returned data. Error occured at Path: {jre.Path}, LineNumber: {jre.LineNumber}, LinePosition: {jre.LinePosition}. Received data: {returnedData}");
            }
            catch (JsonSerializationException jse)
            {
                return ThrowErrorMessage<T>(BitfinexErrors.GetError(BitfinexErrorKey.ParseErrorSerialization), $"Request to {request.RequestUri} failed, couldn't deserialize the returned data. Message: {jse.Message}. Received data: {returnedData}");
            }
            catch (Exception e)
            {
                return ThrowErrorMessage<T>(BitfinexErrors.GetError(BitfinexErrorKey.UnknownError), $"Request to {request.RequestUri} failed with unknown error: {e.Message}. Received data: {returnedData}");
            }
        }

        private async Task<BitfinexError> TryReadError(HttpWebResponse response)
        {
            try
            {
                // Try read error response
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var data = await reader.ReadToEndAsync();
                    if (!data.Contains("\"message\":"))
                        // V2 error
                        return JsonConvert.DeserializeObject<BitfinexError>(data);

                    // V1 error
                    var json = JObject.Parse(data);
                    return new BitfinexError(6000, json["message"].ToString());
                }
            }
            catch (Exception)
            {
                log.Write(LogVerbosity.Warning,
                    $"Couldn't parse error response for status code {response.StatusCode}");
                return null;
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

        private void AddOptionalParameter(Dictionary<string, object> dictionary, string key, object value)
        {
            if (value != null)
                dictionary.Add(key, value);
        }
        #endregion
    }
}
