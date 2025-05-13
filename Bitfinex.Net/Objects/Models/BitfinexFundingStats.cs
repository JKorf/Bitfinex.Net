using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Converters;
using System;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Funding stats
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexFundingStats>))]
    [SerializationModel]
    public record BitfinexFundingStats
    {
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonConverter(typeof(DateTimeConverter))]
        [ArrayProperty(0)]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// 1/365th of Flash Return Rate (To get the daily rate, use: rate x 365. To get the daily rate as percentage use: rate x 365 x 100. To get APR as percentage use rate x 100 x 365 x 365.)
        /// </summary>
        [ArrayProperty(3)]
        public decimal FlashReturnRate { get; set; }
        /// <summary>
        /// Average period for funding provided
        /// </summary>
        [ArrayProperty(4)]
        public decimal AveragePeriod { get; set; }
        /// <summary>
        /// 	Total funding provided
        /// </summary>
        [ArrayProperty(7)]
        public decimal FundingAmount { get; set; }
        /// <summary>
        /// Total funding provided that is used in positions
        /// </summary>
        [ArrayProperty(8)]
        public decimal FundingAmountUsed { get; set; }
        /// <summary>
        /// Sum of open funding offers less than 0.75%
        /// </summary>
        [ArrayProperty(11)]
        public decimal FundingBelowThreshold { get; set; }
    }
}
