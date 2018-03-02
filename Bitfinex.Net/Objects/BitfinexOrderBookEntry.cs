using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexOrderBookEntry
    {
        [BitfinexProperty(0)]
        public decimal Price { get; set; }
        [BitfinexProperty(1)]
        public int Count { get; set; }
        [BitfinexProperty(2)]
        public decimal Amount { get; set; }
    }
}
