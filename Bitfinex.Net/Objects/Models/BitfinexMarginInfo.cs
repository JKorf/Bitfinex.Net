using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Margin info
    /// </summary>
    public record BitfinexMarginInfo
    {
        /// <summary>
        /// Conf
        /// </summary>
        public Dictionary<string, decimal> Conf { get; set; } = new Dictionary<string, decimal>();
        /// <summary>
        /// Initial
        /// </summary>
        public Dictionary<string, decimal> Initial { get; set; } = new Dictionary<string, decimal>();
        /// <summary>
        /// Minimum
        /// </summary>
        public Dictionary<string, decimal> Minimum { get; set; } = new Dictionary<string, decimal>();
    }
}
