using System;
using Microsoft.Extensions.Logging;
using Bitfinex.Net.Interfaces.Clients;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using Bitfinex.Net.Clients.SpotApi;
using CryptoExchange.Net.Authentication;
using Bitfinex.Net.Objects.Options;
using CryptoExchange.Net.Clients;
using Microsoft.Extensions.Options;
using CryptoExchange.Net.Objects.Options;

namespace Bitfinex.Net.Clients
{
    /// <inheritdoc cref="IBitfinexSocketClient" />
    public class BitfinexSocketClient : BaseSocketClient, IBitfinexSocketClient
    {
        #region Api clients

        /// <inheritdoc />
        public IBitfinexSocketClientSpotApi SpotApi { get; }

        #endregion

        #region ctor

        /// <summary>
        /// Create a new instance of the BitfinexSocketClient
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public BitfinexSocketClient(Action<BitfinexSocketOptions>? optionsDelegate = null)
            : this(Options.Create(ApplyOptionsDelegate(optionsDelegate)), null)
        {
        }

        /// <summary>
        /// Create a new instance of the BitfinexSocketClient
        /// </summary>
        /// <param name="loggerFactory">The logger factory</param>
        /// <param name="options">Option configuration</param>
        public BitfinexSocketClient(IOptions<BitfinexSocketOptions> options, ILoggerFactory? loggerFactory = null) : base(loggerFactory, "Bitfinex")
        {
            Initialize(options.Value);

            SpotApi = AddApiClient(new BitfinexSocketClientSpotApi(_logger, options.Value));
        }

        #endregion
        /// <inheritdoc />
        public void SetOptions(UpdateOptions options)
        {
            SpotApi.SetOptions(options);
        }

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public static void SetDefaultOptions(Action<BitfinexSocketOptions> optionsDelegate)
        {
            BitfinexSocketOptions.Default = ApplyOptionsDelegate(optionsDelegate);
        }

        /// <inheritdoc />
        public void SetApiCredentials(ApiCredentials credentials)
        {
            SpotApi.SetApiCredentials(credentials);
        }
    }
}