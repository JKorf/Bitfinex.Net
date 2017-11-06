using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexMarketAveragePrice
    {
        [BitfinexProperty(0)]
        public double AverageRate { get; set; }

        [BitfinexProperty(1)]
        public double Amount { get; set; }
    }
}
