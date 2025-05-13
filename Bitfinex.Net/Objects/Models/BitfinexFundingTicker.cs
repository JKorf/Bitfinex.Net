using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Funding ticker information
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexFundingTicker>))]
    [SerializationModel]
    public record BitfinexFundingTicker
    {
        /// <summary>
        /// The symbol
        /// </summary>
        [ArrayProperty(0)]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Flash Return Rate - average of all fixed rate funding over the last hour
        /// </summary>
        [ArrayProperty(1)]
        public decimal FlashReturnRate { get; set; }
        /// <summary>
        /// The best bid price
        /// </summary>
        [ArrayProperty(2)]
        public decimal BestBidPrice { get; set; }
        /// <summary>
        /// Bid period covered in days
        /// </summary>
        [ArrayProperty(3)]
        public int BidPeriod { get; set; }
        /// <summary>
        /// The best bid quantity
        /// </summary>
        [ArrayProperty(4)]
        public decimal BestBidQuantity { get; set; }
        /// <summary>
        /// The best ask price
        /// </summary>
        [ArrayProperty(5)]
        public decimal BestAskPrice { get; set; }
        /// <summary>
        /// Ask period covered in days
        /// </summary>
        [ArrayProperty(6)]
        public int AskPeriod { get; set; }
        /// <summary>
        /// The best ask quantity
        /// </summary>
        [ArrayProperty(7)]
        public decimal BestAskQuantity { get; set; }
        /// <summary>
        /// Change versus 24 hours ago
        /// </summary>
        [ArrayProperty(8)]
        public decimal DailyChange { get; set; }
        /// <summary>
        /// Change percentage versus 24 hours ago
        /// </summary>
        [ArrayProperty(9)]
        public decimal DailyChangePercentage { get; set; }
        /// <summary>
        /// The last trade price
        /// </summary>
        [ArrayProperty(10)]
        public decimal LastPrice { get; set; }
        /// <summary>
        /// The 24 hour volume
        /// </summary>
        [ArrayProperty(11)]
        public decimal Volume { get; set; }
        /// <summary>
        /// The 24 hour high price
        /// </summary>
        [ArrayProperty(12)]
        public decimal HighPrice { get; set; }
        /// <summary>
        /// The 24 hour low price
        /// </summary>
        [ArrayProperty(13)]
        public decimal LowPrice { get; set; }
        /// <summary>
        /// The amount of funding that is available at the Flash Return Rate
        /// </summary>
        [ArrayProperty(16)]
        public decimal FlashReturnRateAvailable { get; set; }
    }
}
