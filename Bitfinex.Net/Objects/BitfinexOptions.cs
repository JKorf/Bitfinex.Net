using System;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;

namespace Bitfinex.Net.Objects
{
    public class BitfinexClientOptions : ExchangeOptions
    {
        public BitfinexClientOptions()
        {
            BaseAddress = "https://api.bitfinex.com";
        }
    }

    public class BitfinexSocketClientOptions: ExchangeOptions
    {
        public BitfinexSocketClientOptions()
        {
            BaseAddress = "wss://api.bitfinex.com/ws/2";
        }

        /// <summary>
        /// The receive timeout after which a lost connection is assumed
        /// </summary>
        public TimeSpan SocketReceiveTimeout { get; set; } = TimeSpan.FromSeconds(15);

        /// <summary>
        /// The time to wait for a subscribe response
        /// </summary>
        public TimeSpan SubscribeResponseTimeout { get; set; } = TimeSpan.FromSeconds(15);

        /// <summary>
        /// The time to wait for an order confirmation
        /// </summary>
        public TimeSpan OrderActionConfirmationTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// The time to wait before trying to reconnect on lost connection
        /// </summary>
        public TimeSpan ReconnectionInterval { get; set; } = TimeSpan.FromSeconds(2);
    }
}
