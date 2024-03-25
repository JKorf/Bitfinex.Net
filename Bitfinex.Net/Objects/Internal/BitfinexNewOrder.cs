using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;
using Newtonsoft.Json;
using System;

namespace Bitfinex.Net.Objects.Internal
{
    internal class BitfinexNewOrder
    {
        [JsonConverter(typeof(OrderTypeConverter)), JsonProperty("type")]
        public OrderType OrderType { get; set; }

        [JsonProperty("symbol")] 
        public string Symbol { get; set; } = string.Empty;
        [JsonProperty("amount", DefaultValueHandling = DefaultValueHandling.Ignore), JsonConverter(typeof(DecimalAsStringConverter))]
        public decimal? Amount { get; set; }
        [JsonProperty("price", DefaultValueHandling = DefaultValueHandling.Ignore), JsonConverter(typeof(DecimalAsStringConverter))]
        public decimal? Price { get; set; }
        [JsonProperty("price_trailing", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public decimal? PriceTrailing { get; set; }
        [JsonProperty("price_aux_limit", DefaultValueHandling = DefaultValueHandling.Ignore), JsonConverter(typeof(DecimalAsStringConverter))]
        public decimal? PriceAuxiliaryLimit { get; set; }
        [JsonProperty("price_oco_stop", DefaultValueHandling = DefaultValueHandling.Ignore), JsonConverter(typeof(DecimalAsStringConverter))]
        public decimal? PriceOCOStop { get; set; }
        [JsonProperty("flags", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public OrderFlags? Flags { get; set; }
        [JsonProperty("gid", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? GroupId { get; set; }
        [JsonProperty("cid", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? ClientOrderId { get; set; }
        [JsonProperty("lev", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Leverage { get; set; }
        [JsonProperty("tif", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? CancelAfter { get; set; }
        [JsonProperty("meta", DefaultValueHandling = DefaultValueHandling.Ignore)]
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
        [JsonProperty("aff_code", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? AffiliateCode { get; set; }
    }
}
