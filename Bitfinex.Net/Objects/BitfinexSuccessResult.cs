using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexSuccessResult
    {
        [BitfinexProperty(0)]
        public bool Success { get; set; }
    }
}
