using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.SocketObjects
{
    internal class BitfinexAuthentication
    {
        [JsonProperty("event")]
        public string Event { get; set; } = string.Empty;
        [JsonProperty("apiKey")]
        public string ApiKey { get; set; } = string.Empty;
        [JsonProperty("authPayload")]
        public string Payload { get; set; } = string.Empty;
        [JsonProperty("authSig")]
        public string Signature { get; set; } = string.Empty;
        [JsonProperty("authNonce")]
        public string Nonce { get; set; } = string.Empty;
        [JsonProperty("filter")]
        public string[]? Filter { get; set; }
    }
}
