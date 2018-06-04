using System;
using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexTradeDetails
    {
        [ArrayProperty(0)]
        public long Id { get; set; }

        [ArrayProperty(1)]
        public string Pair { get; set; }

        [ArrayProperty(2), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampCreated { get; set; }

        [ArrayProperty(3)]
        public long OrderId { get; set; }

        [ArrayProperty(4)]
        public decimal ExecutedAmount { get; set; }

        [ArrayProperty(5)]
        public decimal ExecutedPrice { get; set; }

        [ArrayProperty(6), JsonConverter(typeof(OrderTypeConverter))]
        public OrderType? OrderType { get; set; }

        [ArrayProperty(7)]
        public decimal? OrderPrice { get; set; }

        [ArrayProperty(8), JsonConverter(typeof(BoolToIntConverter))]
        public bool? Maker { get; set; }

        [ArrayProperty(9)]
        public decimal Fee { get; set; }

        [ArrayProperty(10)]
        public string FeeCurrency { get; set; }
    }
}
