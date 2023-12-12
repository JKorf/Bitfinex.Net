using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models.Socket
{
    internal class BitfinexSocketInfo
    {
        [JsonProperty("event")]
        public string Event { get; set; } = string.Empty;
        [JsonProperty("version")]
        public int Version { get; set; }
        [JsonProperty("serverId")]
        public string ServerId { get; set; } = string.Empty;
        [JsonProperty("platform")]
        public BitfinexSocketInfoDetails Platform { get; set; } = null!;
    }

    internal class BitfinexSocketInfoDetails
    {
        [JsonProperty("status")]
        [JsonConverter(typeof(PlatformStatusConverter))]
        public PlatformStatus Status { get; set; }
    }
}
