using Bitfinex.Net.Interfaces.Clients.GeneralApi;
using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Options;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients.GeneralApi
{
    /// <inheritdoc cref="IBitfinexRestClientGeneralApi" />
    public class BitfinexRestClientGeneralApi : RestApiClient, IBitfinexRestClientGeneralApi
    {
        #region fields
        internal string? AffiliateCode { get; set; }

        /// <inheritdoc />
        public new BitfinexRestOptions ClientOptions => (BitfinexRestOptions)base.ClientOptions;
        #endregion

        #region Api clients
        /// <inheritdoc />
        public IBitfinexRestClientGeneralApiFunding Funding { get; }
        #endregion

        #region ctor

        internal BitfinexRestClientGeneralApi(ILogger logger, HttpClient? httpClient, BitfinexRestOptions options) :
            base(logger, httpClient, options.Environment.RestAddress, options, options.SpotOptions)
        {
            Funding = new BitfinexRestClientGeneralApiFunding(this);

            AffiliateCode = options.AffiliateCode;
        }

        #endregion

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new BitfinexAuthenticationProvider(credentials, ClientOptions.NonceProvider ?? new BitfinexNonceProvider());

        internal Task<WebCallResult<T>> SendRequestAsync<T>(
            Uri uri,
            HttpMethod method,
            CancellationToken cancellationToken,
            Dictionary<string, object>? parameters = null,
            bool signed = false) where T : class
                => base.SendRequestAsync<T>(uri, method, cancellationToken, parameters, signed);

        internal Uri GetUrl(string endpoint, string version)
        {
            return new Uri(BaseAddress.AppendPath($"v{version}", endpoint));
        }

        /// <inheritdoc />
        protected override Error ParseErrorResponse(int httpStatusCode, IEnumerable<KeyValuePair<string, IEnumerable<string>>> responseHeaders, string data)
        {
            var errorData = ValidateJson(data);
            if (!errorData)
                return new ServerError(data);

            if (!(errorData.Data is JArray))
            {
                if (errorData.Data["error"] != null && errorData.Data["code"] != null && errorData.Data["error_description"] != null)
                    return new ServerError((int)errorData.Data["code"]!, errorData.Data["error"] + ": " + errorData.Data["error_description"]);
                if (errorData.Data["message"] != null)
                    return new ServerError(errorData.Data["message"]!.ToString());
                else
                    return new ServerError(errorData.Data.ToString());
            }

            var error = errorData.Data.ToObject<BitfinexError>();
            return new ServerError(error!.ErrorCode, error.ErrorMessage);
        }

        /// <inheritdoc />
        public override TimeSyncInfo? GetTimeSyncInfo() => null;

        /// <inheritdoc />
        public override TimeSpan? GetTimeOffset() => null;
    }
}
