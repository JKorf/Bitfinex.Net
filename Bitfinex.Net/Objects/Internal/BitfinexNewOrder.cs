using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Internal
{
    internal class BitfinexNewOrder
    {
        [JsonConverter(typeof(OrderTypeConverter)), JsonProperty("type")]
        public OrderType OrderType { get; set; }

        [JsonProperty("symbol")] 
        public string Symbol { get; set; } = string.Empty;
        [JsonProperty("amount"), JsonConverter(typeof(DecimalAsStringConverter))]
        public decimal? Amount { get; set; }
        [JsonProperty("price"), JsonConverter(typeof(DecimalAsStringConverter))]
        public decimal? Price { get; set; }
        [JsonProperty("price_trailing")]
        public decimal? PriceTrailing { get; set; }
        [JsonProperty("price_aux_limit"), JsonConverter(typeof(DecimalAsStringConverter))]
        public decimal? PriceAuxiliaryLimit { get; set; }
        [JsonProperty("price_oco_stop"), JsonConverter(typeof(DecimalAsStringConverter))]
        public decimal? PriceOCOStop { get; set; }
        [JsonProperty("flags")]
        public OrderFlags? Flags { get; set; }
        [JsonProperty("gid")]
        public long? GroupId { get; set; }
        [JsonProperty("cid")]
        public long? ClientOrderId { get; set; }
        [JsonProperty("meta")]
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
        [JsonProperty("aff_code")]
        public string? AffiliateCode { get; set; }
    }
}
