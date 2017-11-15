using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    public class BitfinexSymbol
    {
        [JsonProperty("pair")]
        public string Pair { get; set; }

        [JsonProperty("price_precision")]
        public int PricePrecision { get; set; }

        [JsonProperty("intial_margin")]
        public double InitialMargin { get; set; }

        [JsonProperty("minimum_margin")]
        public double MinimumMargin { get; set; }

        [JsonProperty("maximum_order_size")]
        public double MaximumOrderSize { get; set; }

        [JsonProperty("minimum_order_size")]
        public double MinimumOrderSize { get; set; }

        [JsonProperty("expiration")]
        public string Expiration { get; set; }
    }
}
