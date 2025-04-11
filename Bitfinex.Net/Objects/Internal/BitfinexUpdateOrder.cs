using Bitfinex.Net.Enums;

namespace Bitfinex.Net.Objects.Internal
{
    [SerializationModel]
    internal class BitfinexUpdateOrder
    {
        [JsonPropertyName("id")]
        public long OrderId { get; set; }
        [JsonPropertyName("amount"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonConverter(typeof(DecimalStringWriterConverter))]
        public decimal? Amount { get; set; }
        [JsonPropertyName("delta"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonConverter(typeof(DecimalStringWriterConverter))]
        public decimal? Delta { get; set; }
        [JsonPropertyName("price"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonConverter(typeof(DecimalStringWriterConverter))]
        public decimal? Price { get; set; }
        [JsonPropertyName("price_trailing"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? PriceTrailing { get; set; }
        [JsonPropertyName("price_aux_limit"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), ]
        public string? PriceAuxiliaryLimit { get; set; }        
        [JsonPropertyName("flags"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public OrderFlags? Flags { get; set; }
    }
}
