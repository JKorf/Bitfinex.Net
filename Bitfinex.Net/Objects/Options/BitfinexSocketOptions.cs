using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects.Options;
using System;

namespace Bitfinex.Net.Objects.Options
{
    /// <summary>
    /// Options for the BinanceSocketClient
    /// </summary>
    public class BitfinexSocketOptions : SocketExchangeOptions<BitfinexEnvironment>
    {
        /// <summary>
        /// Default options for new clients
        /// </summary>
        public static BitfinexSocketOptions Default { get; set; } = new BitfinexSocketOptions()
        {
            Environment = BitfinexEnvironment.Live,
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

        /// <summary>
        /// Spot API options
        /// </summary>
        public SocketApiOptions SpotOptions { get; private set; } = new SocketApiOptions();

        internal BitfinexSocketOptions Copy()
        {
            var options = Copy<BitfinexSocketOptions>();
            options.AffiliateCode = AffiliateCode;
            options.NonceProvider = NonceProvider;
            options.SpotOptions = SpotOptions.Copy<SocketApiOptions>();
            return options;
        }
    }
}
