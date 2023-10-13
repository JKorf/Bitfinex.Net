using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Symbol info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexSymbolInfo
    {
        /// <summary>
        /// Maximum number of significant digits for price in this pair
        /// </summary>
        [ArrayProperty(1)]
        public int PricePrecision { get; set; }
        /// <summary>
        /// Min order quantity
        /// </summary>
        [ArrayProperty(3)]
        public decimal MinOrderQuantity { get; set; }
        /// <summary>
        /// Max order quantity
        /// </summary>
        [ArrayProperty(4)]
        public decimal MaxOrderQuantity { get; set; }
        /// <summary>
        /// Initial margin
        /// </summary>
        [ArrayProperty(8)]
        public decimal? InitialMargin { get; set; }
        /// <summary>
        /// Min margin
        /// </summary>
        [ArrayProperty(9)]
        public decimal? MinMargin { get; set; }
    }
}
