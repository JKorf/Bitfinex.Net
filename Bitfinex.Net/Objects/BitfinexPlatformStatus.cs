using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexPlatformStatus
    {
        [BitfinexProperty(0), JsonConverter(typeof(PlatformStatusConverter))]
        public PlatformStatus Status { get; set; }
    }
}
