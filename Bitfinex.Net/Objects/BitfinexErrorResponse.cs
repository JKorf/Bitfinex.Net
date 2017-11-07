using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexErrorResponse
    {
        [BitfinexProperty(1)]
        public int ErrorCode { get; set; }
        [BitfinexProperty(2)]
        public string ErrorMessage { get; set; }
    }
}
