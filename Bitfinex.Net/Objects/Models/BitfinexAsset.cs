using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Asset info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexAsset>))]
    [SerializationModel]
    public record BitfinexAsset
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
