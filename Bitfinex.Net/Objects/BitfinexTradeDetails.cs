using System;
using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexTradeDetails
    {
        [BitfinexProperty(0)]
        public long Id { get; set; }

        [BitfinexProperty(1)]
        public string Pair { get; set; }

        [BitfinexProperty(2), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampCreated { get; set; }

        [BitfinexProperty(3)]
        public long OrderId { get; set; }

        [BitfinexProperty(4)]
        public double ExecutedAmount { get; set; }

        [BitfinexProperty(5)]
        public double ExecutedPrice { get; set; }

        [BitfinexProperty(6), JsonConverter(typeof(OrderTypeConverter))]
        public OrderType OrderType { get; set; }

        [BitfinexProperty(7)]
        public double OrderPrice { get; set; }

        [BitfinexProperty(8)]
        public bool Maker { get; set; }

        [BitfinexProperty(9)]
        public double Fee { get; set; }

        [BitfinexProperty(10)]
        public string FeeCurrency { get; set; }
    }
}
