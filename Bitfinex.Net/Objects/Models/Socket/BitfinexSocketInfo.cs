using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;

namespace Bitfinex.Net.Objects.Models.Socket
{
    internal record BitfinexSocketInfo
    {
        [JsonPropertyName("event")]
        public string Event { get; set; } = string.Empty;
        [JsonPropertyName("version")]
        public int? Version { get; set; }
        [JsonPropertyName("serverId")]
        public string? ServerId { get; set; } = string.Empty;
        [JsonPropertyName("platform")]
        public BitfinexSocketInfoDetails? Platform { get; set; } = null!;
        [JsonPropertyName("code")]
        public int? Code { get; set; }
        [JsonPropertyName("msg")]
        public string? Message { get; set; }
    }

    internal record BitfinexSocketInfoDetails
    {
        [JsonPropertyName("status")]
        [JsonConverter(typeof(EnumConverter))]
        public PlatformStatus Status { get; set; }
    }
}
