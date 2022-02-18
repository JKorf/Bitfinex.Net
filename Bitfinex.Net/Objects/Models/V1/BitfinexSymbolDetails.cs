using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models.V1
{
    /// <summary>
    /// Symbol details
    /// </summary>
    public class BitfinexSymbolDetails
    {
        /// <summary>
        /// The symbol pair
        /// </summary>
        [JsonProperty("pair")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// The price precision of the pair
        /// </summary>
        [JsonProperty("price_precision")]
        public int PricePrecision { get; set; }
        /// <summary>
        /// The initial margin required to open a position
        /// </summary>
        [JsonProperty("initial_margin")]
        public decimal InitialMargin { get; set; }
        /// <summary>
        /// The minimal margin to maintain
        /// </summary>
        [JsonProperty("minimum_margin")]
        public decimal MinimumMargin { get; set; }
        /// <summary>
        /// The maximum order size
        /// </summary>
        [JsonProperty("maximum_order_size")]
        public decimal MaximumOrderQuantity { get; set; }
        /// <summary>
        /// The minimum order size
        /// </summary>
        [JsonProperty("minimum_order_size")]
        public decimal MinimumOrderQuantity { get; set; }

        /// <summary>
        /// Expiration
        /// </summary>
        public string Expiration { get; set; } = string.Empty;
        /// <summary>
        /// If margin trading is enabled for the pair
        /// </summary>
        public bool Margin { get; set; }
    }
}
