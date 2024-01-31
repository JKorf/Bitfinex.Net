using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Sockets
{
    internal class BitfinexRequest
    {
        [JsonProperty("event")]
        public string Event { get; set; } = string.Empty;
        [JsonProperty("channel")]
        public string Channel { get; set; } = string.Empty;
        [JsonProperty("symbol")]
        public string? Symbol { get; set; }
    }
}
