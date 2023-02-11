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

namespace Bitfinex.Net.Clients
{
    /// <inheritdoc cref="IBitfinexSocketClient" />
    public class BitfinexSocketClient : BaseSocketClient, IBitfinexSocketClient
    {
        #region Api clients

        /// <inheritdoc />
        public IBitfinexSocketClientSpotStreams SpotStreams { get; }

        #endregion

        #region ctor
        /// <summary>
        /// Create a new instance of BitfinexSocketClient using the default options
        /// </summary>
        public BitfinexSocketClient() : this(BitfinexSocketClientOptions.Default)
        {
        }

        /// <summary>
        /// Create a new instance of BitfinexSocketClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public BitfinexSocketClient(BitfinexSocketClientOptions options) : base("Bitfinex", options)
        {
            if (options == null)
                throw new ArgumentException("Cant pass null options, use empty constructor for default");

            SpotStreams = AddApiClient(new BitfinexSocketClientSpotStreams(log, options));
        }
        #endregion

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="options">Options to use as default</param>
        public static void SetDefaultOptions(BitfinexSocketClientOptions options)
        {
            BitfinexSocketClientOptions.Default = options;
        }
    }
}
