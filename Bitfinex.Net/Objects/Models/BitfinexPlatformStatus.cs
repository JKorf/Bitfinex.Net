using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Platform status info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexPlatformStatus
    {
        /// <summary>
        /// The current platform status
        /// </summary>
        [ArrayProperty(0), JsonConverter(typeof(PlatformStatusConverter))]
        public PlatformStatus Status { get; set; }
    }
}
