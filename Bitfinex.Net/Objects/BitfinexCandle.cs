using System;
using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexCandle
    {
        [BitfinexProperty(0), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }
        [BitfinexProperty(1)]
        public decimal Open { get; set; }
        [BitfinexProperty(2)]
        public decimal Close { get; set; }
        [BitfinexProperty(3)]
        public decimal High { get; set; }
        [BitfinexProperty(4)]
        public decimal Low { get; set; }
        [BitfinexProperty(5)]
        public decimal Volume { get; set; }
    }
}
