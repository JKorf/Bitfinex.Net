using CryptoExchange.Net.Authentication;

namespace Bitfinex.Net
{
    /// <summary>
    /// Bitfinex credentials
    /// </summary>
    public class BitfinexCredentials : ApiCredentials
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="apiKey">The API key</param>
        /// <param name="secret">The API secret</param>
        public BitfinexCredentials(string apiKey, string secret) : this(new HMACCredential(apiKey, secret)) { }
       
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="credential">The HMAC credentials</param>
        public BitfinexCredentials(HMACCredential credential) : base(credential) { }

        /// <inheritdoc />
        public override ApiCredentials Copy() => new BitfinexCredentials(Hmac!);
    }
}
