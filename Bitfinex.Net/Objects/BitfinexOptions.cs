using System;
using Bitfinex.Net.Interfaces;
using CryptoExchange.Net.Objects;

namespace Bitfinex.Net.Objects
{
    public class BitfinexClientOptions : RestClientOptions
    {
        public BitfinexClientOptions()
        {
            BaseAddress = "https://api.bitfinex.com";
        }
    }

    public class BitfinexSocketClientOptions: SocketClientOptions
    {
        public BitfinexSocketClientOptions()
        {
            SocketSubscriptionsCombineTarget = 10;
            BaseAddress = "wss://api.bitfinex.com/ws/2";
            SocketNoDataTimeout = TimeSpan.FromSeconds(30);
        }
    }

    public class BitfinexOrderBookOptions : OrderBookOptions
    {
        /// <summary>
        /// The client to use for the socket connection. When using the same client for multiple order books the connection can be shared.
        /// </summary>
        public IBitfinexSocketClient SocketClient { get; }

        /// <summary>
        /// </summary>
        /// <param name="client">The client to use for the socket connection. When using the same client for multiple order books the connection can be shared.</param>
        public BitfinexOrderBookOptions(IBitfinexSocketClient client = null) : base("Bitfinex", false)
        {
            SocketClient = client;
        }
    }
}
