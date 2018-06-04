using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    public class BitfinexSymbolDetails
    {
        public string Pair { get; set; }
        [JsonProperty("price_precision")]
        public decimal PricePrecision { get; set; }
        [JsonProperty("initial_margin")]
        public decimal InitialMargin { get; set; }
        [JsonProperty("minimum_margin")]
        public decimal MinimumMargin { get; set; }
        [JsonProperty("maximum_order_size")]
        public decimal MaximumOrderSize { get; set; }
        [JsonProperty("minimum_order_size")]
        public decimal MinimumOrderSize { get; set; }
        public string Expiration { get; set; }
        public bool Margin { get; set; }
    }
}
