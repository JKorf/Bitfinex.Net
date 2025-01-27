using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects.Options;

namespace Bitfinex.Net.Objects.Options
{
    /// <summary>
    /// Bitfinex options
    /// </summary>
    public class BitfinexOptions : LibraryOptions<BitfinexRestOptions, BitfinexSocketOptions, ApiCredentials, BitfinexEnvironment>
    {
    }
}
