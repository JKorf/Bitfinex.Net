using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Sockets
{
    internal class BitfinexBookRequest: BitfinexRequest
    {
        [JsonProperty("prec")]
        public string? Precision { get; set; }
        [JsonProperty("freq")]
        public string? Frequency { get; set; }
        [JsonProperty("len")]
        public string? Length { get; set; }
        [JsonProperty("key")]
        public string? Key { get; set; }
    }
}
