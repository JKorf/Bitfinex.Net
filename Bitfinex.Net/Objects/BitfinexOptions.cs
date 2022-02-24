using System;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Interfaces.Clients;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;

namespace Bitfinex.Net.Objects
{
    /// <summary>
    /// Options for the BitfinexClient
    /// </summary>
    public class BitfinexClientOptions : BaseRestClientOptions
    {
        /// <summary>
        /// Default options for the client
        /// </summary>
        public static BitfinexClientOptions Default { get; set; } = new BitfinexClientOptions();
            
        /// <summary>
        /// Default affiliate code to use when placing orders
        /// </summary>
        public string? AffiliateCode { get; set; } = "kCCe-CNBO";

        /// <summary>
        /// Optional nonce provider for signing requests. Careful providing a custom provider; once a nonce is sent to the server, every request after that needs a higher nonce than that
        /// </summary>
        public INonceProvider? NonceProvider { get; set; }

        private RestApiClientOptions _spotApiOptions = new RestApiClientOptions(BitfinexApiAddresses.Default.RestClientAddress);
        /// <summary>
        /// Options for the spot API
        /// </summary>
        public RestApiClientOptions SpotApiOptions
        {
            get => _spotApiOptions;
            set => _spotApiOptions = new RestApiClientOptions(_spotApiOptions, value);
        }

        /// <summary>
        /// ctor
        /// </summary>
        public BitfinexClientOptions() : this(Default)
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="baseOn">Base the new options on other options</param>
        internal BitfinexClientOptions(BitfinexClientOptions baseOn) : base(baseOn)
        {
            if (baseOn == null)
                return;

            AffiliateCode = baseOn.AffiliateCode;
            NonceProvider = baseOn.NonceProvider;

            _spotApiOptions = new RestApiClientOptions(baseOn.SpotApiOptions, null);
        }
    }

    /// <summary>
    /// Options for the BitfinexSocketClient
    /// </summary>
    public class BitfinexSocketClientOptions: BaseSocketClientOptions
    {
        /// <summary>
        /// Default options for the client
        /// </summary>
        public static BitfinexSocketClientOptions Default { get; set; } = new BitfinexSocketClientOptions()
        {
            SocketSubscriptionsCombineTarget = 10,
            SocketNoDataTimeout = TimeSpan.FromSeconds(30)
        };

        /// <summary>
        /// Default affiliate code to use when placing orders
        /// </summary>
        public string? AffiliateCode { get; set; } = "kCCe-CNBO";

        /// <summary>
        /// Optional nonce provider for signing requests. Careful providing a custom provider; once a nonce is sent to the server, every request after that needs a higher nonce than that
        /// </summary>
        public INonceProvider? NonceProvider { get; set; }

        
        private ApiClientOptions _spotStreamsOptions = new ApiClientOptions(BitfinexApiAddresses.Default.SocketClientAddress);
        /// <summary>
        /// Options for the spot streams
        /// </summary>
        public ApiClientOptions SpotStreamsOptions
        {
            get => _spotStreamsOptions;
            set => _spotStreamsOptions = new ApiClientOptions(_spotStreamsOptions, value);
        }

        /// <summary>
        /// ctor
        /// </summary>
        public BitfinexSocketClientOptions() : this(Default)
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="baseOn">Base the new options on other options</param>
        internal BitfinexSocketClientOptions(BitfinexSocketClientOptions baseOn) : base(baseOn)
        {
            if (baseOn == null)
                return;

            AffiliateCode = baseOn.AffiliateCode;
            NonceProvider = baseOn.NonceProvider;

            _spotStreamsOptions = new ApiClientOptions(baseOn.SpotStreamsOptions, null);
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
        public IBitfinexSocketClient? SocketClient { get; set; }
        /// <summary>
        /// The precision level of the order book
        /// </summary>
        public Precision? Precision { get; set; }
        /// <summary>
        /// The limit of entries in the order book, either 25 or 100
        /// </summary>
        public int? Limit { get; set; }
    }
}
