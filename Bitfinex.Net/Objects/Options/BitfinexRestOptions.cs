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
        internal static BitfinexRestOptions Default { get; set; } = new BitfinexRestOptions()
        {
            Environment = BitfinexEnvironment.Live
        };

        /// <summary>
        /// ctor
        /// </summary>
        public BitfinexRestOptions()
        {
            Default?.Set(this);
        }

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

        internal BitfinexRestOptions Set(BitfinexRestOptions targetOptions)
        {
            targetOptions = base.Set<BitfinexRestOptions>(targetOptions);
            targetOptions.AffiliateCode = AffiliateCode;
            targetOptions.NonceProvider = NonceProvider;
            targetOptions.SpotOptions = SpotOptions.Set(targetOptions.SpotOptions);
            return targetOptions;
        }
    }
}
