using System;
using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
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
        /// The symbol of the offer
        /// </summary>
        [ArrayProperty(1)]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The creation timestamp
        /// </summary>
        [ArrayProperty(2), JsonConverter(typeof(DateTimeConverter))]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// The last update timestamp
        /// </summary>
        [ArrayProperty(3), JsonConverter(typeof(DateTimeConverter))]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// The quantity of the offer
        /// </summary>
        [ArrayProperty(4)]
        public decimal Quantity { get; set; }

        /// <summary>
        /// The original quantity
        /// </summary>
        [ArrayProperty(5)]
        public decimal QuantityOriginal { get; set; }

        /// <summary>
        /// The funding type
        /// </summary>
        [ArrayProperty(6), JsonConverter(typeof(FundingOfferTypeConverter))]
        public FundingOfferType FundingType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(7)]
        public string PlaceHolder1 { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(8)]
        public string PlaceHolder2 { get; set; } = string.Empty;
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
        public string StatusString { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(11)]
        public string PlaceHolder3 { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(12)]
        public string PlaceHolder4 { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(13)]
        public string PlaceHolder5 { get; set; } = string.Empty;

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
        public string PlaceHolder6 { get; set; } = string.Empty;
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
