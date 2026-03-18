using CryptoExchange.Net.Authentication;

namespace Bitfinex.Net
{
    /// <summary>
    /// Bitfinex API credentials
    /// </summary>
    public class BitfinexCredentials : HMACCredential
    {
        /// <summary>
        /// Create new credentials providing only credentials in HMAC format
        /// </summary>
        /// <param name="key">API key</param>
        /// <param name="secret">API secret</param>
        public BitfinexCredentials(string key, string secret) : base(key, secret)
        {
        }
    }
}
