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
        public double Open { get; set; }
        [BitfinexProperty(2)]
        public double Close { get; set; }
        [BitfinexProperty(3)]
        public double High { get; set; }
        [BitfinexProperty(4)]
        public double Low { get; set; }
        [BitfinexProperty(5)]
        public double Volume { get; set; }
    }
}
