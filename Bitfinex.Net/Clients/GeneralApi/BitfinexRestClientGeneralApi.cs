using Bitfinex.Net.Interfaces.Clients.GeneralApi;
using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Options;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Interfaces;
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
        protected override Error ParseErrorResponse(int httpStatusCode, IEnumerable<KeyValuePair<string, IEnumerable<string>>> responseHeaders, IMessageAccessor accessor)
        {
            if (!accessor.IsJson)
                return new ServerError(accessor.GetOriginalString());

            if (accessor.GetNodeType() != NodeType.Array)
            {
                var error = accessor.GetValue<string?>(MessagePath.Get().Property("error"));
                var errorCode = accessor.GetValue<int?>(MessagePath.Get().Property("code"));
                var errorDesc = accessor.GetValue<string?>(MessagePath.Get().Property("error_description"));
                if (error != null && errorCode != null && errorDesc != null)
                    return new ServerError(errorCode.Value, $"{error}: {errorDesc}");

                var message = accessor.GetValue<string?>(MessagePath.Get().Property("message"));
                if (message != null)
                    return new ServerError(message);

                return new ServerError(accessor.GetOriginalString());
            }

            var code = accessor.GetValue<int?>(MessagePath.Get().Index(1));
            var msg = accessor.GetValue<string>(MessagePath.Get().Index(2));
            if (msg == null)
                return new ServerError(accessor.GetOriginalString());

            if (code == null)
                return new ServerError(msg);

            return new ServerError(code.Value, msg);
        }

        /// <inheritdoc />
        public override TimeSyncInfo? GetTimeSyncInfo() => null;

        /// <inheritdoc />
        public override TimeSpan? GetTimeOffset() => null;
    }
}
