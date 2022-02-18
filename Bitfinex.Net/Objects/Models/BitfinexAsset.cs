using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Asset info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexAsset
    {
        /// <summary>
        /// The shorthand name of the asset
        /// </summary>
        [ArrayProperty(0)]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// The full name of the asset
        /// </summary>
        [ArrayProperty(1)]
        public string FullName { get; set; } = string.Empty;
    }
}
