using System;
using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    /// <summary>
    /// Funding offer info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexFundingOffer
    {
        /// <summary>
        /// The offer id
        /// </summary>
        [ArrayProperty(0)]
        public long Id { get; set; }

        /// <summary>
        /// The currency of the offer
        /// </summary>
        [ArrayProperty(1)]
        public string Symbol { get; set; } = "";

        /// <summary>
        /// The creation timestamp
        /// </summary>
        [ArrayProperty(2), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampCreated { get; set; }

        /// <summary>
        /// The last update timestamp
        /// </summary>
        [ArrayProperty(3), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampUpdated { get; set; }

        /// <summary>
        /// The amount of the offer
        /// </summary>
        [ArrayProperty(4)]
        public decimal Amount { get; set; }

        /// <summary>
        /// The original amount
        /// </summary>
        [ArrayProperty(5)]
        public decimal AmountOriginal { get; set; }

        /// <summary>
        /// The funding type
        /// </summary>
        [ArrayProperty(6), JsonConverter(typeof(FundingTypeConverter))]
        public FundingType FundingType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(7)]
        public string PlaceHolder1 { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(8)]
        public string PlaceHolder2 { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(9)]
        public int? Flags { get; set; }
        /// <summary>
        /// The status
        /// </summary>
        [JsonIgnore]
        public OrderStatus Status => new OrderStatusConverter().FromString(StatusString);
        /// <summary>
        /// The raw status string
        /// </summary>
        [ArrayProperty(10)]
        public string StatusString { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(11)]
        public string PlaceHolder3 { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(12)]
        public string PlaceHolder4 { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(13)]
        public string PlaceHolder5 { get; set; } = "";

        /// <summary>
        /// The rate of the order
        /// </summary>
        [ArrayProperty(14)]
        public decimal Rate { get; set; }

        /// <summary>
        /// The period of the offer in days
        /// </summary>
        [ArrayProperty(15)]
        public int Period { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(16), JsonConverter(typeof(BoolToIntConverter))]
        public bool Notify { get; set; }

        /// <summary>
        /// Whether the offer is hidden
        /// </summary>
        [ArrayProperty(17), JsonConverter(typeof(BoolToIntConverter))]
        public bool Hidden { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(18)]
        public string PlaceHolder6 { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(19), JsonConverter(typeof(BoolToIntConverter))]
        public bool Renew { get; set; }

        /// <summary>
        /// The calculated rate
        /// </summary>
        [ArrayProperty(20)]
        public decimal RateReal { get; set; }
    }
}
