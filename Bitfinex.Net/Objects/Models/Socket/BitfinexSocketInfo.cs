using CryptoExchange.Net.Converters.SystemTextJson;
using Bitfinex.Net.Enums;

namespace Bitfinex.Net.Objects.Models.Socket
{
    [SerializationModel]
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

    [SerializationModel]
    internal record BitfinexSocketInfoDetails
    {
        [JsonPropertyName("status")]

        public PlatformStatus Status { get; set; }
    }
}
