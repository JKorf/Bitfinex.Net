using CryptoExchange.Net.Converters.SystemTextJson;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Margin info
    /// </summary>
    [SerializationModel]
    public record BitfinexMarginInfo
    {
        /// <summary>
        /// Conf
        /// </summary>
        [JsonPropertyName("conf")]
        public Dictionary<string, decimal> Conf { get; set; } = new Dictionary<string, decimal>();
        /// <summary>
        /// Initial
        /// </summary>
        [JsonPropertyName("initial")]
        public Dictionary<string, decimal> Initial { get; set; } = new Dictionary<string, decimal>();
        /// <summary>
        /// Minimum
        /// </summary>
        [JsonPropertyName("minimum")]
        public Dictionary<string, decimal> Minimum { get; set; } = new Dictionary<string, decimal>();
    }
}
