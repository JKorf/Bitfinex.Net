using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Available balance
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexAvailableBalance
    {
        /// <summary>
        /// The available balance
        /// </summary>
        [ArrayProperty(0)]
        public decimal AvailableBalance { get; set; }
    }
}
