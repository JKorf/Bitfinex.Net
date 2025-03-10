using Bitfinex.Net.Enums;
using System;

namespace Bitfinex.Net.Objects.Internal
{
    [SerializationModel]
    internal class BitfinexNewOrder
    {
        [JsonPropertyName("type")]
        public OrderType OrderType { get; set; }

        [JsonPropertyName("symbol")] 
        public string Symbol { get; set; } = string.Empty;
        [JsonPropertyName("amount"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonConverter(typeof(DecimalStringWriterConverter))]
        public decimal? Amount { get; set; }
        [JsonPropertyName("price"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonConverter(typeof(DecimalStringWriterConverter))]
        public decimal? Price { get; set; }
        [JsonPropertyName("price_trailing"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public decimal? PriceTrailing { get; set; }
        [JsonPropertyName("price_aux_limit"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonConverter(typeof(DecimalStringWriterConverter))]
        public decimal? PriceAuxiliaryLimit { get; set; }
        [JsonPropertyName("price_oco_stop"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonConverter(typeof(DecimalStringWriterConverter))]
        public decimal? PriceOCOStop { get; set; }
        [JsonPropertyName("flags"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? Flags { get; set; }
        [JsonPropertyName("gid"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public long? GroupId { get; set; }
        [JsonPropertyName("cid"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public long? ClientOrderId { get; set; }
        [JsonPropertyName("lev"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? Leverage { get; set; }
        [JsonPropertyName("tif"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime? CancelAfter { get; set; }
        [JsonPropertyName("meta"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public BitfinexMeta? Meta { get; set; }
    }

    /// <summary>
    /// Order meta data
    /// </summary>
    public class BitfinexMeta
    {
        /// <summary>
        /// The affiliate code for the order
        /// </summary>
        [JsonPropertyName("aff_code"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? AffiliateCode { get; set; }
    }
}
