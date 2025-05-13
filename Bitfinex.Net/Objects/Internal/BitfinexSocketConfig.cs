namespace Bitfinex.Net.Objects.Internal
{
    [SerializationModel]
    internal class BitfinexSocketConfig
    {
        [JsonPropertyName("event")]
        public string Event { get; set; } = string.Empty;

        [JsonPropertyName("flags")]
        public int Flags { get; set; }
    }
}
