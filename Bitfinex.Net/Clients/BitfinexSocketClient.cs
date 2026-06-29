using System;
using Microsoft.Extensions.Logging;
using Bitfinex.Net.Interfaces.Clients;
using CryptoExchange.Net.Authentication;
using Bitfinex.Net.Objects.Options;
using CryptoExchange.Net.Clients;
using Microsoft.Extensions.Options;
using CryptoExchange.Net.Objects.Options;
using Bitfinex.Net.Clients.ExchangeApi;
using Bitfinex.Net.Interfaces.Clients.ExchangeApi;

namespace Bitfinex.Net.Clients
{
    /// <inheritdoc cref="IBitfinexSocketClient" />
    public class BitfinexSocketClient : BaseSocketClient<BitfinexEnvironment, BitfinexCredentials>, IBitfinexSocketClient
    {
        #region Api clients

        /// <inheritdoc />
        public IBitfinexSocketClientExchangeApi ExchangeApi { get; }

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

            ExchangeApi = AddApiClient(new BitfinexSocketClientExchangeApi(loggerFactory, options.Value));
        }

        #endregion

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public static void SetDefaultOptions(Action<BitfinexSocketOptions> optionsDelegate)
        {
            BitfinexSocketOptions.Default = ApplyOptionsDelegate(optionsDelegate);
        }
    }
}