using System;
using CryptoExchange.Net.Objects;

namespace Bitfinex.Net.Objects
{
    public class BitfinexClientOptions : ClientOptions
    {
        public BitfinexClientOptions()
        {
            BaseAddress = "https://api.bitfinex.com";
        }
    }

    public class BitfinexSocketClientOptions: SocketClientOptions
    {
        /// <summary>
        /// The time to wait for a socket response
        /// </summary>
        public TimeSpan SocketResponseTimeout { get; set; } = TimeSpan.FromSeconds(10);
        /// <summary>
        /// The time after which the connection is assumed to be dropped
        /// </summary>
        public TimeSpan SocketNoDataTimeout { get; set; } = TimeSpan.FromSeconds(30);


        public BitfinexSocketClientOptions()
        {
            BaseAddress = "wss://api.bitfinex.com/ws/2";
        }        

        public BitfinexSocketClientOptions Copy()
        {
            var copy = Copy<BitfinexSocketClientOptions>();
            copy.SocketResponseTimeout = SocketResponseTimeout;
            copy.SocketNoDataTimeout = SocketNoDataTimeout;
            return copy;
        }
    }
}
