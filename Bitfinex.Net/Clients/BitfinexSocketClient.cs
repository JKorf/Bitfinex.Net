using Bitfinex.Net.Objects;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Bitfinex.Net.Enums;
using System.Threading;
using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Interfaces.Clients;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using Bitfinex.Net.Clients.SpotApi;
using CryptoExchange.Net.Authentication;
using Bitfinex.Net.Objects.Options;

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
        /// <param name="loggerFactory">The logger factory</param>
        public BitfinexSocketClient(ILoggerFactory? loggerFactory = null) : this((x) => { }, loggerFactory)
        {
        }

        /// <summary>
        /// Create a new instance of the BitfinexSocketClient
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public BitfinexSocketClient(Action<BitfinexSocketOptions> optionsDelegate) : this(optionsDelegate, null)
        {
        }

        /// <summary>
        /// Create a new instance of the BitfinexSocketClient
        /// </summary>
        /// <param name="loggerFactory">The logger factory</param>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public BitfinexSocketClient(Action<BitfinexSocketOptions> optionsDelegate, ILoggerFactory? loggerFactory = null) : base(loggerFactory, "Bitfinex")
        {
            var options = BitfinexSocketOptions.Default.Copy();
            optionsDelegate(options);
            Initialize(options);

            SpotApi = AddApiClient(new BitfinexSocketClientSpotApi(_logger, options));
        }

        #endregion

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public static void SetDefaultOptions(Action<BitfinexSocketOptions> optionsDelegate)
        {
            var options = BitfinexSocketOptions.Default.Copy();
            optionsDelegate(options);
            BitfinexSocketOptions.Default = options;
        }

        /// <inheritdoc />
        public void SetApiCredentials(ApiCredentials credentials)
        {
            SpotApi.SetApiCredentials(credentials);
        }
    }
}