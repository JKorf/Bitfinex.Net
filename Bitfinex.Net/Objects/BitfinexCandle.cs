using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexCandle
    {
        [ArrayProperty(0), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }
        [ArrayProperty(1)]
        public decimal Open { get; set; }
        [ArrayProperty(2)]
        public decimal Close { get; set; }
        [ArrayProperty(3)]
        public decimal High { get; set; }
        [ArrayProperty(4)]
        public decimal Low { get; set; }
        [ArrayProperty(5)]
        public decimal Volume { get; set; }
    }
}
