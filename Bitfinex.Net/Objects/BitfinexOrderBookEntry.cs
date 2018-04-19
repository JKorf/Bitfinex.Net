using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexOrderBookBase { }

    public class BitfinexOrderBookEntry: BitfinexOrderBookBase
    {
        [ArrayProperty(0)]
        public decimal Price { get; set; }
        [ArrayProperty(1)]
        public int Count { get; set; }
        [ArrayProperty(2)]
        public decimal Amount { get; set; }
    }

    public class BitfinexRawOrderBookEntry : BitfinexOrderBookBase
    {
        [ArrayProperty(0)]
        public long OrderId { get; set; }
        [ArrayProperty(1)]
        public decimal Price { get; set; }
        [ArrayProperty(2)]
        public decimal Amount { get; set; }
    }
}
