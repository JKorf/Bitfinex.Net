using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Sockets
{
    internal class BitfinexResponse
    {
        [JsonProperty("code")]
        public int? Code { get; set; }
        [JsonProperty("event")]
        public string Event { get; set; } = string.Empty;
        [JsonProperty("msg")]
        public string Message { get; set; } = string.Empty;
        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;
        [JsonProperty("channel")]
        public string Channel { get; set; } = string.Empty;
        [JsonProperty("symbol")]
        public string? Symbol { get; set; }
        [JsonProperty("pair")]
        public string? Pair { get; set; }
        [JsonProperty("chanId")]
        public int? ChannelId { get; set; }
    }
}
