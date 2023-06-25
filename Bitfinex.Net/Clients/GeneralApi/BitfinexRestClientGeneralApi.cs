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

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override Error ParseErrorResponse(JToken data)
        {
            if (!(data is JArray))
            {
                if (data["error"] != null && data["code"] != null && data["error_description"] != null)
                    return new ServerError((int)data["code"]!, data["error"] + ": " + data["error_description"]);
                if (data["message"] != null)
                    return new ServerError(data["message"]!.ToString());
                else
                    return new ServerError(data.ToString());
            }

            var error = data.ToObject<BitfinexError>();
            return new ServerError(error!.ErrorCode, error.ErrorMessage);

        }

        /// <inheritdoc />
        public override TimeSyncInfo? GetTimeSyncInfo() => null;

        /// <inheritdoc />
        public override TimeSpan? GetTimeOffset() => null;
    }
}
