using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexMarketAveragePrice
    {
        [BitfinexProperty(0)]
        public decimal AverageRate { get; set; }

        [BitfinexProperty(1)]
        public decimal Amount { get; set; }
    }
}
