using CryptoExchange.Net.Authentication;
using System;

namespace Bitfinex.Net
{
    /// <summary>
    /// Bitfinex credentials
    /// </summary>
    public class BitfinexCredentials : ApiCredentials
    {
        /// <summary>
        /// </summary>
        [Obsolete("Parameterless constructor is only for deserialization purposes and should not be used directly. Use parameterized constructor instead.")]
        public BitfinexCredentials() { }

        /// <summary>
        /// Create credentials using an HMAC key and secret.
        /// </summary>
        /// <param name="apiKey">API key</param>
        /// <param name="secret">API secret</param>
        public BitfinexCredentials(string apiKey, string secret) : this(new HMACCredential(apiKey, secret)) { }

        /// <summary>
        /// Create credentials using HMAC credentials
        /// </summary>
        /// <param name="credential">HMAC credentials</param>
        public BitfinexCredentials(HMACCredential credential) : base(credential) { }

        /// <inheritdoc />
#pragma warning disable CS0618 // Type or member is obsolete
        public override ApiCredentials Copy() => new BitfinexCredentials { CredentialPairs = CredentialPairs };
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
