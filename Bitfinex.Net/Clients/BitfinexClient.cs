using Bitfinex.Net.Objects;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Interfaces.Clients;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using Bitfinex.Net.Interfaces.Clients.GeneralApi;
using Bitfinex.Net.Clients.GeneralApi;
using Bitfinex.Net.Clients.SpotApi;
using CryptoExchange.Net.Authentication;

namespace Bitfinex.Net.Clients
{
    /// <inheritdoc cref="IBitfinexClient" />
    public class BitfinexClient : BaseRestClient, IBitfinexClient
    {
        #region Api clients

        /// <inheritdoc />
        public IBitfinexClientGeneralApi GeneralApi { get; }
        /// <inheritdoc />
        public IBitfinexClientSpotApi SpotApi { get; }

        #endregion

        #region constructor/destructor
        /// <summary>
        /// Create a new instance of BitfinexClient using the default options
        /// </summary>
        public BitfinexClient() : this(BitfinexClientOptions.Default)
        {
        }

        /// <summary>
        /// Create a new instance of BitfinexClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public BitfinexClient(BitfinexClientOptions options) : base("Bitfinex", options)
        {
            if (options == null)
                throw new ArgumentException("Cant pass null options, use empty constructor for default");

            GeneralApi = AddApiClient(new BitfinexClientGeneralApi(log, options));
            SpotApi = AddApiClient(new BitfinexClientSpotApi(log, options));
        }
        #endregion

        #region methods

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="options">Options to use as default</param>
        public static void SetDefaultOptions(BitfinexClientOptions options)
        {
            BitfinexClientOptions.Default = options;
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
