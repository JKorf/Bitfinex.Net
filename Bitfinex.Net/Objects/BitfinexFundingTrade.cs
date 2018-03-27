using System;
using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexFundingTrade
    {
        [ArrayProperty(0)]
        public long Id { get; set; }
        [ArrayProperty(1)]
        public string Currency { get; set; }
        [ArrayProperty(2), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }
        [ArrayProperty(3)]
        public long OfferId { get; set; }
        [ArrayProperty(4)]
        public decimal Amount { get; set; }
        [ArrayProperty(5)]
        public decimal Rate { get; set; }
        [ArrayProperty(6)]
        public int Period { get; set; }
        [ArrayProperty(7), JsonConverter(typeof(BoolToIntConverter))]
        public bool Maker { get; set; }
    }
}
