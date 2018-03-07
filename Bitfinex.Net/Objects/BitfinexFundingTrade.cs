using System;
using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexFundingTrade
    {
        [BitfinexProperty(0)]
        public long Id { get; set; }
        [BitfinexProperty(1)]
        public string Currency { get; set; }
        [BitfinexProperty(2), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }
        [BitfinexProperty(3)]
        public long OfferId { get; set; }
        [BitfinexProperty(4)]
        public decimal Amount { get; set; }
        [BitfinexProperty(5)]
        public decimal Rate { get; set; }
        [BitfinexProperty(6)]
        public int Period { get; set; }
        [BitfinexProperty(7), JsonConverter(typeof(BoolToIntConverter))]
        public bool Maker { get; set; }
    }
}
