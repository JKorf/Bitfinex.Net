using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.SocketObjects
{
    internal class BitfinexSocketConfig
    {
        [JsonProperty("event")] public string Event { get; set; } = string.Empty;

        [JsonProperty("flags")]
        public int Flags { get; set; }
    }
}
