using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    /// <summary>
    /// Currency info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexCurrency
    {
        /// <summary>
        /// The shorthand name of the currency
        /// </summary>
        [ArrayProperty(0)]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// The full name of the currency
        /// </summary>
        [ArrayProperty(1)]
        public string FullName { get; set; } = string.Empty;
    }
}
