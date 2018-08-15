using System;
using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexFundingOffer
    {
        [ArrayProperty(0)]
        public long Id { get; set; }

        [ArrayProperty(1)]
        public string Symbol { get; set; }

        [ArrayProperty(2), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampCreated { get; set; }

        [ArrayProperty(3), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampUpdated { get; set; }

        [ArrayProperty(4)]
        public decimal Amount { get; set; }

        [ArrayProperty(5)]
        public decimal AmountOriginal { get; set; }

        [ArrayProperty(6), JsonConverter(typeof(FundingTypeConverter))]
        public FundingType FundingType { get; set; }

        [ArrayProperty(7)]
        public string PlaceHolder1 { get; set; }

        [ArrayProperty(8)]
        public string PlaceHolder2 { get; set; }

        [ArrayProperty(9)]
        public int? Flags { get; set; }

        [ArrayProperty(10), JsonConverter(typeof(OrderStatusConverter))]
        public OrderStatus Status { get; set; }
        [ArrayProperty(10)]
        public string StatusString { get; set; }

        [ArrayProperty(11)]
        public string PlaceHolder3 { get; set; }

        [ArrayProperty(12)]
        public string PlaceHolder4 { get; set; }

        [ArrayProperty(13)]
        public string PlaceHolder5 { get; set; }

        [ArrayProperty(14)]
        public decimal Rate { get; set; }

        [ArrayProperty(15)]
        public int Period { get; set; }

        [ArrayProperty(16), JsonConverter(typeof(BoolToIntConverter))]
        public bool Notify { get; set; }

        [ArrayProperty(17), JsonConverter(typeof(BoolToIntConverter))]
        public bool Hidden { get; set; }

        [ArrayProperty(18)]
        public string PlaceHolder6 { get; set; }

        [ArrayProperty(19), JsonConverter(typeof(BoolToIntConverter))]
        public bool Renew { get; set; }

        [ArrayProperty(20)]
        public decimal RateReal { get; set; }
    }
}
