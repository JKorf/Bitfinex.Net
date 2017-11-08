using System;
using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexPerformance
    {
        [BitfinexProperty(0), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }

        [BitfinexProperty(1)]
        public double Performance { get; set; }
    }
}
