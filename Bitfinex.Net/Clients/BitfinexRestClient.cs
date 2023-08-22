using CryptoExchange.Net;
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
        public BitfinexRestClient(Action<BitfinexRestOptions> optionsDelegate) : this(null, null, optionsDelegate)
        {
        }

        /// <summary>
        /// Create a new instance of the BitfinexRestClient using provided options
        /// </summary>
        public BitfinexRestClient(ILoggerFactory? loggerFactory = null, HttpClient? httpClient = null) : this(httpClient, loggerFactory, null)
        {
        }

        /// <summary>
        /// Create a new instance of the BitfinexRestClient using provided options
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        /// <param name="loggerFactory">The logger factory</param>
        /// <param name="httpClient">Http client for this client</param>
        public BitfinexRestClient(HttpClient? httpClient, ILoggerFactory? loggerFactory, Action<BitfinexRestOptions>? optionsDelegate = null) : base(loggerFactory, "Bitfinex")
        {
            var options = BitfinexRestOptions.Default.Copy();
            if (optionsDelegate != null)
                optionsDelegate(options);
            Initialize(options);

            GeneralApi = AddApiClient(new BitfinexRestClientGeneralApi(_logger, httpClient, options));
            SpotApi = AddApiClient(new BitfinexRestClientSpotApi(_logger, httpClient, options));
        }
        #endregion

        #region methods

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public static void SetDefaultOptions(Action<BitfinexRestOptions> optionsDelegate)
        {
            var options = BitfinexRestOptions.Default.Copy();
            optionsDelegate(options);
            BitfinexRestOptions.Default = options;
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
