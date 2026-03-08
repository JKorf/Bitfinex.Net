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
        /// ["<c>conf</c>"] Conf
        /// </summary>
        [JsonPropertyName("conf")]
        public Dictionary<string, decimal> Conf { get; set; } = new Dictionary<string, decimal>();
        /// <summary>
        /// ["<c>initial</c>"] Initial
        /// </summary>
        [JsonPropertyName("initial")]
        public Dictionary<string, decimal> Initial { get; set; } = new Dictionary<string, decimal>();
        /// <summary>
        /// ["<c>minimum</c>"] Minimum
        /// </summary>
        [JsonPropertyName("minimum")]
        public Dictionary<string, decimal> Minimum { get; set; } = new Dictionary<string, decimal>();
    }
}
