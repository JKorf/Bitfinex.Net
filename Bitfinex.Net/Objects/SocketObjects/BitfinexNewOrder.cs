using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class BitfinexNewOrder
    {
        [JsonConverter(typeof(OrderTypeConverter)), JsonProperty("type")]
        public OrderType OrderType { get; set; }
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
        [JsonProperty("amount"), JsonConverter(typeof(DecimalAsStringConverter))]
        public decimal Amount { get; set; }
        [JsonProperty("price"), JsonConverter(typeof(DecimalAsStringConverter))]
        public decimal? Price { get; set; }
        [JsonProperty("price_trailing")]
        public decimal? PriceTrailing { get; set; }
        [JsonProperty("price_aux_limit")]
        public decimal? PriceAuxiliaryLimit { get; set; }
        [JsonProperty("price_oco_stop"), JsonConverter(typeof(DecimalAsStringConverter))]
        public decimal? PriceOCOStop { get; set; }
        [JsonProperty("flags")]
        public OrderFlags? Flags { get; set; }
        [JsonProperty("cid")]
        public int? GroupId { get; set; }
        [JsonProperty("gid")]
        public int? ClientOrderId { get; set; }
    }
}
