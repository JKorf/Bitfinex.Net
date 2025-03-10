namespace Bitfinex.Net.Objects.Sockets
{
    [SerializationModel]
    internal class BitfinexRequest
    {
        [JsonPropertyName("event")]
        public string Event { get; set; } = string.Empty;
        [JsonPropertyName("channel")]
        public string Channel { get; set; } = string.Empty;
        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }
    }
}
