using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Internal
{
    internal class BitfinexUpdateOrder
    {
        [JsonProperty("id")]
        public long OrderId { get; set; }
        [JsonProperty("amount"), JsonConverter(typeof(DecimalAsStringConverter))]
        public decimal? Amount { get; set; }
        [JsonProperty("delta"), JsonConverter(typeof(DecimalAsStringConverter))]
        public decimal? Delta { get; set; }
        [JsonProperty("price"), JsonConverter(typeof(DecimalAsStringConverter))]
        public decimal? Price { get; set; }
        [JsonProperty("price_trailing")]
        public string? PriceTrailing { get; set; }
        [JsonProperty("price_aux_limit"), JsonConverter(typeof(DecimalAsStringConverter))]
        public string? PriceAuxiliaryLimit { get; set; }        
        [JsonProperty("flags")]
        public OrderFlags? Flags { get; set; }
    }
}
