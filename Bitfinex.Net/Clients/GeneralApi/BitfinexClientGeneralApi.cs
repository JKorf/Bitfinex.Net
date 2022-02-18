using Bitfinex.Net.Clients.SpotApi;
using Bitfinex.Net.Interfaces.Clients.GeneralApi;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.Internal;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients.GeneralApi
{
    /// <inheritdoc cref="IBitfinexClientGeneralApi" />
    public class BitfinexClientGeneralApi : RestApiClient, IBitfinexClientGeneralApi
    {
        #region fields
        internal string? AffiliateCode { get; set; }

        private readonly BitfinexClientOptions _options;
        private readonly Log _log;
        private readonly BitfinexClient _baseClient;
        #endregion

        #region Api clients
        /// <inheritdoc />
        public IBitfinexClientGeneralApiFunding Funding { get; }
        #endregion

        #region ctor

        internal BitfinexClientGeneralApi(Log log, BitfinexClient baseClient, BitfinexClientOptions options) :
            base(options, options.SpotApiOptions)
        {
            _baseClient = baseClient;
            _log = log;
            _options = options;

            Funding = new BitfinexClientGeneralApiFunding(this);

            AffiliateCode = options.AffiliateCode;
        }

        #endregion

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new BitfinexAuthenticationProvider(credentials, _options.NonceProvider ?? new BitfinexNonceProvider());

        internal Task<WebCallResult<T>> SendRequestAsync<T>(
            Uri uri,
            HttpMethod method,
            CancellationToken cancellationToken,
            Dictionary<string, object>? parameters = null,
            bool signed = false) where T : class
                => _baseClient.SendRequestAsync<T>(this, uri, method, cancellationToken, parameters, signed);

        internal Uri GetUrl(string endpoint, string version)
        {
            return new Uri(BaseAddress.AppendPath($"v{version}", endpoint));
        }

        /// <inheritdoc />
        protected override Task<WebCallResult<DateTime>> GetServerTimestampAsync()
            => Task.FromResult(new WebCallResult<DateTime>(null, null, null, null, null, null, null, null, DateTime.UtcNow, null));

        /// <inheritdoc />
        protected override TimeSyncInfo GetTimeSyncInfo()
            => new TimeSyncInfo(_log, _options.SpotApiOptions.AutoTimestamp, BitfinexClientSpotApi.TimeSyncState);

        /// <inheritdoc />
        public override TimeSpan GetTimeOffset()
            => BitfinexClientSpotApi.TimeSyncState.TimeOffset;
    }
}
