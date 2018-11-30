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
        

        public BitfinexSocketClientOptions()
        {
            BaseAddress = "wss://api.bitfinex.com/ws/2";
        }        

        public BitfinexSocketClientOptions Copy()
        {
            var copy = Copy<BitfinexSocketClientOptions>();
            copy.SocketResponseTimeout = SocketResponseTimeout;
            return copy;
        }
    }
}
