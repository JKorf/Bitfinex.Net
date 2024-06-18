using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Balance
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public record BitfinexBalance
    {
        /// <summary>
        /// Total Assets Under Management
        /// </summary>
        [ArrayProperty(0)]
        public decimal TotalAssets { get; set; }

        /// <summary>
        /// Net Assets Under Management (total assets - total liabilities)
        /// </summary>
        [ArrayProperty(1)]
        public decimal NetAssets { get; set; }
    }
}
