using System;
using System.Net.Http;
using Bitfinex.Net.Interfaces;
using CryptoExchange.Net.Objects;

namespace Bitfinex.Net.Objects
{
    /// <summary>
    /// Options for the BitfinexClient
    /// </summary>
    public class BitfinexClientOptions : RestClientOptions
    {
        /// <summary>
        /// Default affiliate code to use when placing orders
        /// </summary>
        public string? AffiliateCode { get; set; } = "kCCe-CNBO";

        /// <summary>
        /// Create new client options
        /// </summary>
        public BitfinexClientOptions(): base("https://api.bitfinex.com")
        {
        }

        /// <summary>
        /// Create new client options
        /// </summary>
        /// <param name="client">HttpClient to use for requests from this client</param>
        public BitfinexClientOptions(HttpClient client) : base(client, "https://api.bitfinex.com")
        {
        }

        /// <summary>
        /// Create new client options
        /// </summary>
        /// <param name="apiAddress">Custom API address to use</param>
        /// <param name="client">HttpClient to use for requests from this client</param>
        public BitfinexClientOptions(HttpClient client, string apiAddress) : base(client, apiAddress)
        {
        }
    }

    /// <summary>
    /// Options for the BitfinexSocketClient
    /// </summary>
    public class BitfinexSocketClientOptions: SocketClientOptions
    {
        /// <summary>
        /// Default affiliate code to use when placing orders
        /// </summary>
        public string? AffiliateCode { get; set; } = "kCCe-CNBO";

        /// <summary>
        /// Create new socket options
        /// </summary>
        public BitfinexSocketClientOptions(): base("wss://api.bitfinex.com/ws/2")
        {
            SocketSubscriptionsCombineTarget = 10;
            SocketNoDataTimeout = TimeSpan.FromSeconds(30);
        }
    }

    /// <summary>
    /// Options for the BitfinexSymbolOrderBook
    /// </summary>
    public class BitfinexOrderBookOptions : OrderBookOptions
    {
        /// <summary>
        /// The client to use for the socket connection. When using the same client for multiple order books the connection can be shared.
        /// </summary>
        public IBitfinexSocketClient? SocketClient { get; }

        /// <summary>
        /// </summary>
        /// <param name="client">The client to use for the socket connection. When using the same client for multiple order books the connection can be shared.</param>
        public BitfinexOrderBookOptions(IBitfinexSocketClient? client = null) : base("Bitfinex", false, true)
        {
            SocketClient = client;
        }
    }
}
