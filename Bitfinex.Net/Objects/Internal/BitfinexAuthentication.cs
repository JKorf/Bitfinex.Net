namespace Bitfinex.Net.Objects.Internal
{
    [SerializationModel]
    internal class BitfinexAuthentication
    {
        [JsonPropertyName("event")]
        public string Event { get; set; } = string.Empty;
        [JsonPropertyName("apiKey")]
        public string ApiKey { get; set; } = string.Empty;
        [JsonPropertyName("authPayload")]
        public string Payload { get; set; } = string.Empty;
        [JsonPropertyName("authSig")]
        public string Signature { get; set; } = string.Empty;
        [JsonPropertyName("authNonce")]
        public string Nonce { get; set; } = string.Empty;
        [JsonPropertyName("filter")]
        public string[]? Filter { get; set; }
    }
}
