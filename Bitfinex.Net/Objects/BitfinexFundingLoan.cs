using System;
using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexFundingLoan
    {
        [BitfinexProperty(0)]
        public long Id { get; set; }

        [BitfinexProperty(1)]
        public string Symbol { get; set; }

        [BitfinexProperty(2), JsonConverter(typeof(FundingTypeConverter))]
        public FundingType Side { get; set; }

        [BitfinexProperty(3), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampCreated { get; set; }

        [BitfinexProperty(4), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampUpdated { get; set; }

        [BitfinexProperty(5)]
        public double Amount { get; set; }

        [BitfinexProperty(6)]
        public int? Flags { get; set; }

        [BitfinexProperty(7), JsonConverter(typeof(FundingTypeConverter))]
        public OrderStatus Status { get; set; }

        [BitfinexProperty(8)]
        internal string PlaceHolder1 { get; set; }

        [BitfinexProperty(9)]
        internal string PlaceHolder2 { get; set; }

        [BitfinexProperty(10)]
        internal string PlaceHolder3 { get; set; }
        
        [BitfinexProperty(11)]
        public double Rate { get; set; }

        [BitfinexProperty(12)]
        public int Period { get; set; }

        [BitfinexProperty(13), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampOpened { get; set; }

        [BitfinexProperty(14), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampLastPayout { get; set; }

        [BitfinexProperty(15)]
        public bool Notify { get; set; }

        [BitfinexProperty(16)]
        public bool Hidden { get; set; }

        [BitfinexProperty(17)]
        internal string PlaceHolder6 { get; set; }

        [BitfinexProperty(18)]
        public bool Renew { get; set; }

        [BitfinexProperty(19)]
        public double RateReal { get; set; }

        [BitfinexProperty(20)]
        public bool NoClose { get; set; }
    }
}
