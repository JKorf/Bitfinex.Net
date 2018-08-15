using System;
using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexFundingCredit
    {
        [ArrayProperty(0)]
        public long Id { get; set; }

        [ArrayProperty(1)]
        public string Symbol { get; set; }

        [ArrayProperty(2), JsonConverter(typeof(FundingTypeConverter))]
        public FundingType Side { get; set; }

        [ArrayProperty(3), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampCreated { get; set; }

        [ArrayProperty(4), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampUpdated { get; set; }

        [ArrayProperty(5)]
        public decimal Amount { get; set; }

        [ArrayProperty(6)]
        public int? Flags { get; set; }

        [ArrayProperty(7), JsonConverter(typeof(OrderStatusConverter))]
        public OrderStatus Status { get; set; }
        [ArrayProperty(7)]
        public string StatusString { get; set; }

        [ArrayProperty(8)]
        public string PlaceHolder1 { get; set; }

        [ArrayProperty(9)]
        public string PlaceHolder2 { get; set; }

        [ArrayProperty(10)]
        public string PlaceHolder3 { get; set; }

        [ArrayProperty(11)]
        public decimal Rate { get; set; }

        [ArrayProperty(12)]
        public int Period { get; set; }

        [ArrayProperty(13), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampOpened { get; set; }

        [ArrayProperty(14), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampLastPayout { get; set; }

        [ArrayProperty(15), JsonConverter(typeof(BoolToIntConverter))]
        public bool Notify { get; set; }

        [ArrayProperty(16), JsonConverter(typeof(BoolToIntConverter))]
        public bool Hidden { get; set; }

        [ArrayProperty(17)]
        public string PlaceHolder6 { get; set; }

        [ArrayProperty(18), JsonConverter(typeof(BoolToIntConverter))]
        public bool Renew { get; set; }

        [ArrayProperty(19)]
        public decimal RateReal { get; set; }

        [ArrayProperty(20), JsonConverter(typeof(BoolToIntConverter))]
        public bool NoClose { get; set; }

        [ArrayProperty(21)]
        public string PositionPair { get; set; }
    }
}
