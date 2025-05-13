using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Funding info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexFunding>))]
    [SerializationModel]
    public record BitfinexFunding
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
        /// The side of the funding
        /// </summary>
        [ArrayProperty(2)]

        public FundingSide Side { get; set; }

        /// <summary>
        /// The creation timestamp
        /// </summary>
        [ArrayProperty(3), JsonConverter(typeof(DateTimeConverter))]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// The last update timestamp
        /// </summary>
        [ArrayProperty(4), JsonConverter(typeof(DateTimeConverter))]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// The quantity of the offer
        /// </summary>
        [ArrayProperty(5)]
        public decimal Quantity { get; set; }
        /// <summary>
        /// </summary>
        [ArrayProperty(6)]
        public int? Flags { get; set; }

        /// <summary>
        /// The order status
        /// </summary>
        [ArrayProperty(7)]

        public OrderStatus Status { get; set; }
        /// <summary>
        /// The raw order status string
        /// </summary>
        [ArrayProperty(7)]
        public string StatusString { get; set; } = string.Empty;

        /// <summary>
        /// The price of the offer
        /// </summary>
        [ArrayProperty(11)]
        public decimal Price { get; set; }

        /// <summary>
        /// The period of the offer in days
        /// </summary>
        [ArrayProperty(12)]
        public int Period { get; set; }

        /// <summary>
        /// The timestamp when the funding was opened
        /// </summary>
        [ArrayProperty(13), JsonConverter(typeof(DateTimeConverter))]
        public DateTime OpenTime { get; set; }

        /// <summary>
        /// The timestamp of the last payout
        /// </summary>
        [ArrayProperty(14), JsonConverter(typeof(DateTimeConverter))]
        public DateTime LastPayoutTime { get; set; }

        /// <summary>
        /// If notify
        /// </summary>
        [ArrayProperty(15)]
        public bool Notify { get; set; }

        /// <summary>
        /// If hidden
        /// </summary>
        [ArrayProperty(16)]
        public bool Hidden { get; set; }

        /// <summary>
        /// </summary>
        [ArrayProperty(18)]
        public bool Renew { get; set; }

        /// <summary>
        /// The calculated rate
        /// </summary>
        [ArrayProperty(19)]
        public decimal? RateReal { get; set; }

        /// <summary>
        /// Whether the funding should be closed when position is closed
        /// </summary>
        [ArrayProperty(20)]
        public bool NoClose { get; set; }

        
    }

    /// <summary>
    /// Funding credit info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexFundingCredit>))]
    [SerializationModel]
    public record BitfinexFundingCredit: BitfinexFunding
    {
        /// <summary>
        /// The pair the currency was used for
        /// </summary>
        [ArrayProperty(21)]
        public string PositionPair { get; set; } = string.Empty;
    }
}
