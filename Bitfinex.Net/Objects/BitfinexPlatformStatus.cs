using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
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
