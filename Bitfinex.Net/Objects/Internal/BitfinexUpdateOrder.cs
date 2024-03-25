using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Internal
{
    internal class BitfinexUpdateOrder
    {
        [JsonProperty("id")]
        public long OrderId { get; set; }
        [JsonProperty("amount", DefaultValueHandling = DefaultValueHandling.Ignore), JsonConverter(typeof(DecimalAsStringConverter))]
        public decimal? Amount { get; set; }
        [JsonProperty("delta", DefaultValueHandling = DefaultValueHandling.Ignore), JsonConverter(typeof(DecimalAsStringConverter))]
        public decimal? Delta { get; set; }
        [JsonProperty("price", DefaultValueHandling = DefaultValueHandling.Ignore), JsonConverter(typeof(DecimalAsStringConverter))]
        public decimal? Price { get; set; }
        [JsonProperty("price_trailing", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? PriceTrailing { get; set; }
        [JsonProperty("price_aux_limit", DefaultValueHandling = DefaultValueHandling.Ignore), JsonConverter(typeof(DecimalAsStringConverter))]
        public string? PriceAuxiliaryLimit { get; set; }        
        [JsonProperty("flags", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public OrderFlags? Flags { get; set; }
    }
}
