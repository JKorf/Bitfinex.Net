using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects.Options;
using CryptoExchange.Net.SharedApis;

namespace Bitfinex.Net.Objects.Options
{
    /// <summary>
    /// Bitfinex options
    /// </summary>
    public class BitfinexOptions : LibraryOptions<BitfinexRestOptions, BitfinexSocketOptions, BitfinexCredentials, BitfinexEnvironment>
    {
        /// <summary>
        /// Options for Shared API usage
        /// </summary>
        public SharedApiOptions SharedApi { get; set; } = new();
    }
}
