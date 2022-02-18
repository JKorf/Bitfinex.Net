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

        private readonly RestApiClientOptions _spotApiOptions = new RestApiClientOptions(BitfinexApiAddresses.Default.RestClientAddress);
        /// <summary>
        /// Options for the spot API
        /// </summary>
        public RestApiClientOptions SpotApiOptions
        {
            get => _spotApiOptions;
            set => _spotApiOptions.Copy(_spotApiOptions, value);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public BitfinexClientOptions()
        {
            if (Default == null)
                return;

            Copy(this, Default);
        }

        /// <summary>
        /// Copy the values of the def to the input
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="def"></param>
        public new void Copy<T>(T input, T def) where T : BitfinexClientOptions
        {
            base.Copy(input, def);

            input.AffiliateCode = def.AffiliateCode;
            input.NonceProvider = def.NonceProvider;

            input.SpotApiOptions = new RestApiClientOptions(def.SpotApiOptions);
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

        
        private readonly ApiClientOptions _spotStreamsOptions = new ApiClientOptions(BitfinexApiAddresses.Default.SocketClientAddress);
        /// <summary>
        /// Options for the spot streams
        /// </summary>
        public ApiClientOptions SpotStreamsOptions
        {
            get => _spotStreamsOptions;
            set => _spotStreamsOptions.Copy(_spotStreamsOptions, value);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public BitfinexSocketClientOptions()
        {
            if (Default == null)
                return;

            Copy(this, Default);
        }

        /// <summary>
        /// Copy the values of the def to the input
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="def"></param>
        public new void Copy<T>(T input, T def) where T : BitfinexSocketClientOptions
        {
            base.Copy(input, def);

            input.AffiliateCode = def.AffiliateCode;
            input.NonceProvider = def.NonceProvider;

            input.SpotStreamsOptions = new ApiClientOptions(def.SpotStreamsOptions);
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
