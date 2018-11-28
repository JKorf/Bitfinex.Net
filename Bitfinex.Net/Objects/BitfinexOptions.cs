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
        /// The receive timeout after which a lost connection is assumed
        /// </summary>
        public TimeSpan SocketReceiveTimeout { get; set; } = TimeSpan.FromSeconds(20);

        /// <summary>
        /// The time to wait for a subscribe response
        /// </summary>
        public TimeSpan SubscribeResponseTimeout { get; set; } = TimeSpan.FromSeconds(15);

        /// <summary>
        /// The time to wait for an order confirmation
        /// </summary>
        public TimeSpan OrderActionConfirmationTimeout { get; set; } = TimeSpan.FromSeconds(30);


        public BitfinexSocketClientOptions()
        {
            BaseAddress = "wss://api.bitfinex.com/ws/2";
        }        

        public BitfinexSocketClientOptions Copy()
        {
            var copy = Copy<BitfinexSocketClientOptions>();
            copy.SocketReceiveTimeout = SocketReceiveTimeout;
            copy.SubscribeResponseTimeout = SubscribeResponseTimeout;
            copy.OrderActionConfirmationTimeout = OrderActionConfirmationTimeout;
            return copy;
        }
    }
}
