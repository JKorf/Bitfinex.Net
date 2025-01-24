using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Platform status info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public record BitfinexPlatformStatus
    {
        /// <summary>
        /// The current platform status
        /// </summary>
        [ArrayProperty(0)]
        [JsonConverter(typeof(EnumConverter))]
        public PlatformStatus Status { get; set; }
    }
}
