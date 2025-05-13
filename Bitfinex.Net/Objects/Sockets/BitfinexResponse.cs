namespace Bitfinex.Net.Objects.Sockets
{
    [SerializationModel]
    internal class BitfinexResponse
    {
        [JsonPropertyName("code")]
        public int? Code { get; set; }
        [JsonPropertyName("event")]
        public string Event { get; set; } = string.Empty;
        [JsonPropertyName("msg")]
        public string Message { get; set; } = string.Empty;
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        [JsonPropertyName("channel")]
        public string Channel { get; set; } = string.Empty;
        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }
        [JsonPropertyName("pair")]
        public string? Pair { get; set; }
        [JsonPropertyName("chanId")]
        public int? ChannelId { get; set; }
    }
}
