using CryptoExchange.Net.Authentication;
using System;

namespace Bitfinex.Net
{
    /// <summary>
    /// Bitfinex API credentials
    /// </summary>
    public class BitfinexCredentials : HMACCredential
    {
        /// <summary>
        /// Create new credentials
        /// </summary>
        public BitfinexCredentials() { }

        /// <summary>
        /// Create new credentials providing credentials in HMAC format
        /// </summary>
        /// <param name="key">API key</param>
        /// <param name="secret">API secret</param>
        public BitfinexCredentials(string key, string secret) : base(key, secret)
        {
        }

        /// <summary>
        /// Create new credentials providing HMAC credentials
        /// </summary>
        /// <param name="credential">Credentials</param>
        public BitfinexCredentials(HMACCredential credential) : base(credential.Key, credential.Secret)
        {
        }

        /// <summary>
        /// Specify the HMAC credentials
        /// </summary>
        /// <param name="key">API key</param>
        /// <param name="secret">API secret</param>
        public BitfinexCredentials WithHMAC(string key, string secret)
        {
            if (!string.IsNullOrEmpty(Key)) throw new InvalidOperationException("Credentials already set");

            Key = key;
            Secret = secret;
            return this;
        }

        /// <inheritdoc />
        public override ApiCredentials Copy() => new BitfinexCredentials(this);
    }
}
