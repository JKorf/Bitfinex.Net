using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects.Options;

namespace Bitfinex.Net.Objects.Options
{
    /// <summary>
    /// Options for the BitfinexRestClient
    /// </summary>
    public class BitfinexRestOptions : RestExchangeOptions<BitfinexEnvironment>
    {
        /// <summary>
        /// Default options for new clients
        /// </summary>
        public static BitfinexRestOptions Default { get; set; } = new BitfinexRestOptions()
        {
            Environment = BitfinexEnvironment.Live
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
        public RestApiOptions SpotOptions { get; private set; } = new RestApiOptions();

        internal BitfinexRestOptions Copy()
        {
            var options = Copy<BitfinexRestOptions>();
            options.AffiliateCode = AffiliateCode;
            options.NonceProvider = NonceProvider;
            options.SpotOptions = SpotOptions.Copy<RestApiOptions>();
            return options;
        }
    }
}
