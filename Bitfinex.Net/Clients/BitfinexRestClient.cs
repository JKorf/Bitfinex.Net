using System;
using Bitfinex.Net.Interfaces.Clients;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using Bitfinex.Net.Interfaces.Clients.GeneralApi;
using Bitfinex.Net.Clients.GeneralApi;
using Bitfinex.Net.Clients.SpotApi;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Bitfinex.Net.Objects.Options;
using CryptoExchange.Net.Clients;
using Microsoft.Extensions.Options;
using CryptoExchange.Net.Objects.Options;

namespace Bitfinex.Net.Clients
{
    /// <inheritdoc cref="IBitfinexRestClient" />
    public class BitfinexRestClient : BaseRestClient, IBitfinexRestClient
    {
        #region Api clients

        /// <inheritdoc />
        public IBitfinexRestClientGeneralApi GeneralApi { get; }
        /// <inheritdoc />
        public IBitfinexRestClientSpotApi SpotApi { get; }

        #endregion

        #region constructor/destructor
        /// <summary>
        /// Create a new instance of the BitfinexRestClient using provided options
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public BitfinexRestClient(Action<BitfinexRestOptions>? optionsDelegate = null)
            : this(null, null, Options.Create(ApplyOptionsDelegate(optionsDelegate)))
        {
        }

        /// <summary>
        /// Create a new instance of the BitfinexRestClient using provided options
        /// </summary>
        /// <param name="options">Option configuration</param>
        /// <param name="loggerFactory">The logger factory</param>
        /// <param name="httpClient">Http client for this client</param>
        public BitfinexRestClient(HttpClient? httpClient, ILoggerFactory? loggerFactory, IOptions<BitfinexRestOptions> options) : base(loggerFactory, "Bitfinex")
        {
            Initialize(options.Value);

            GeneralApi = AddApiClient(new BitfinexRestClientGeneralApi(_logger, httpClient, options.Value));
            SpotApi = AddApiClient(new BitfinexRestClientSpotApi(_logger, httpClient, options.Value));
        }
        #endregion

        #region methods
        /// <inheritdoc />
        public void SetOptions(UpdateOptions options)
        {
            GeneralApi.SetOptions(options);
            SpotApi.SetOptions(options);
        }

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public static void SetDefaultOptions(Action<BitfinexRestOptions> optionsDelegate)
        {
            BitfinexRestOptions.Default = ApplyOptionsDelegate(optionsDelegate);
        }

        /// <inheritdoc />
        public void SetApiCredentials(ApiCredentials credentials)
        {
            GeneralApi.SetApiCredentials(credentials);
            SpotApi.SetApiCredentials(credentials);
        }
        #endregion
    }
}
