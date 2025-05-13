using CryptoExchange.Net.Converters.SystemTextJson;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Platform status info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexPlatformStatus>))]
    [SerializationModel]
    public record BitfinexPlatformStatus
    {
        /// <summary>
        /// The current platform status
        /// </summary>
        [ArrayProperty(0)]

        public PlatformStatus Status { get; set; }
    }
}
