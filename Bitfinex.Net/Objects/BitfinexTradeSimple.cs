using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexTradeSimple
    {
        [ArrayProperty(0)]
        public long Id { get; set; }
        [ArrayProperty(1), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }
        [ArrayProperty(2)]
        public decimal Amount { get; set; }
        [ArrayProperty(3)]
        public decimal Price { get; set; }
    }
}
