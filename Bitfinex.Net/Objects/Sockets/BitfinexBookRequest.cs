namespace Bitfinex.Net.Objects.Sockets
{
    [SerializationModel]
    internal class BitfinexBookRequest: BitfinexRequest
    {
        [JsonPropertyName("prec")]
        public string? Precision { get; set; }
        [JsonPropertyName("freq")]
        public string? Frequency { get; set; }
        [JsonPropertyName("len")]
        public string? Length { get; set; }
        [JsonPropertyName("key")]
        public string? Key { get; set; }
    }
}
