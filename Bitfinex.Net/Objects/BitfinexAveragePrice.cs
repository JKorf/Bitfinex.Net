using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    /// <summary>
    /// Average price info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexAveragePrice
    {
        /// <summary>
        /// The average price
        /// </summary>
        [ArrayProperty(0)]
        public decimal AverageRate { get; set; }

        /// <summary>
        /// The amount
        /// </summary>
        [ArrayProperty(1)]
        public decimal Amount { get; set; }
    }
}
