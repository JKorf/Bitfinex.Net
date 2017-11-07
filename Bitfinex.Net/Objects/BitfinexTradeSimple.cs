using System;
using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexTradeSimple
    {
        [BitfinexProperty(0)]
        public long Id { get; set; }
        [BitfinexProperty(1), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }
        [BitfinexProperty(2)]
        public double Amount { get; set; }
        [BitfinexProperty(3)]
        public double Price { get; set; }
    }
}
