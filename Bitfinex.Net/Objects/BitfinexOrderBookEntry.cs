using CryptoExchange.Net.Converters;
using CryptoExchange.Net.OrderBook;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexOrderBookBase { }

    public class BitfinexOrderBookEntry: BitfinexOrderBookBase, ISymbolOrderBookEntry
    {
        [ArrayProperty(0)]
        public decimal Price { get; set; }
        [ArrayProperty(1)]
        public int Count { get; set; }
        [ArrayProperty(2)]
        public decimal Quantity { get; set; }
    }

    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexRawOrderBookEntry : BitfinexOrderBookBase, ISymbolOrderBookEntry
    {
        [ArrayProperty(0)]
        public long OrderId { get; set; }
        [ArrayProperty(1)]
        public decimal Price { get; set; }
        [ArrayProperty(2)]
        public decimal Quantity { get; set; }
    }
}
