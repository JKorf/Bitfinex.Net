using System;
using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    /// <summary>
    /// Funding info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexFunding
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
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The side of the funding
        /// </summary>
        [ArrayProperty(2), JsonConverter(typeof(FundingTypeConverter))]
        public FundingType Side { get; set; }

        /// <summary>
        /// The creation timestamp
        /// </summary>
        [ArrayProperty(3), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampCreated { get; set; }

        /// <summary>
        /// The last update timestamp
        /// </summary>
        [ArrayProperty(4), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampUpdated { get; set; }

        /// <summary>
        /// The amount of the offer
        /// </summary>
        [ArrayProperty(5)]
        public decimal Amount { get; set; }
        /// <summary>
        /// </summary>
        [ArrayProperty(6)]
        public int? Flags { get; set; }

        /// <summary>
        /// The order status
        /// </summary>
        [JsonIgnore]
        public OrderStatus Status => new OrderStatusConverter().FromString(StatusString);
        /// <summary>
        /// The raw order status string
        /// </summary>
        [ArrayProperty(7)]
        public string StatusString { get; set; } = string.Empty;
        /// <summary>
        /// </summary>
        [ArrayProperty(8)]
        public string PlaceHolder1 { get; set; } = string.Empty;
        /// <summary>
        /// </summary>
        [ArrayProperty(9)]
        public string PlaceHolder2 { get; set; } = string.Empty;
        /// <summary>
        /// </summary>
        [ArrayProperty(10)]
        public string PlaceHolder3 { get; set; } = string.Empty;

        /// <summary>
        /// The rate of the offer
        /// </summary>
        [ArrayProperty(11)]
        public decimal Rate { get; set; }

        /// <summary>
        /// The period of the offer in days
        /// </summary>
        [ArrayProperty(12)]
        public int Period { get; set; }

        /// <summary>
        /// The timestamp when the funding was opened
        /// </summary>
        [ArrayProperty(13), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampOpened { get; set; }

        /// <summary>
        /// The timestamp of the last payout
        /// </summary>
        [ArrayProperty(14), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampLastPayout { get; set; }

        /// <summary>
        /// If notify
        /// </summary>
        [ArrayProperty(15), JsonConverter(typeof(BoolToIntConverter))]
        public bool Notify { get; set; }

        /// <summary>
        /// If hidden
        /// </summary>
        [ArrayProperty(16), JsonConverter(typeof(BoolToIntConverter))]
        public bool Hidden { get; set; }
        /// <summary>
        /// </summary>
        [ArrayProperty(17)]
        public string PlaceHolder6 { get; set; } = string.Empty;
        /// <summary>
        /// </summary>
        [ArrayProperty(18), JsonConverter(typeof(BoolToIntConverter))]
        public bool Renew { get; set; }

        /// <summary>
        /// The calculated rate
        /// </summary>
        [ArrayProperty(19)]
        public decimal RateReal { get; set; }

        /// <summary>
        /// Whether the funding should be closed when position is closed
        /// </summary>
        [ArrayProperty(20), JsonConverter(typeof(BoolToIntConverter))]
        public bool NoClose { get; set; }

        
    }

    /// <summary>
    /// Funding credit info
    /// </summary>
    public class BitfinexFundingCredit: BitfinexFunding
    {
        /// <summary>
        /// The pair the currency was used for
        /// </summary>
        [ArrayProperty(21)]
        public string PositionPair { get; set; } = string.Empty;
    }
}
