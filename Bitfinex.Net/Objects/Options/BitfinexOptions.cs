using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bitfinex.Net.Objects.Options
{
    /// <summary>
    /// Bitfinex options
    /// </summary>
    public class BitfinexOptions
    {
        /// <summary>
        /// Rest client options
        /// </summary>
        public BitfinexRestOptions Rest { get; set; } = new BitfinexRestOptions();

        /// <summary>
        /// Socket client options
        /// </summary>
        public BitfinexSocketOptions Socket { get; set; } = new BitfinexSocketOptions();

        /// <summary>
        /// Trade environment. Contains info about URL's to use to connect to the API. Use `BitfinexEnvironment` to swap environment, for example `Environment = BitfinexEnvironment.Live`
        /// </summary>
        public BitfinexEnvironment? Environment { get; set; }

        /// <summary>
        /// The api credentials used for signing requests.
        /// </summary>
        public ApiCredentials? ApiCredentials { get; set; }

        /// <summary>
        /// The DI service lifetime for the IBinanceSocketClient
        /// </summary>
        public ServiceLifetime? SocketClientLifeTime { get; set; }
    }
}
