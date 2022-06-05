using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models.Socket
{
    /// <summary>
    /// Market overview
    /// </summary>
    [JsonConverter(typeof(BitfinexStreamSymbolOverviewConverter))]
    public class BitfinexStreamSymbolOverview
    {
        /// <summary>
        /// FRR
        /// </summary>
        [ArrayProperty(1)]
        public decimal? Frr { get; set; }
        /// <summary>
        /// The best bid price
        /// </summary>
        [ArrayProperty(2)]
        public decimal BestBidPrice { get; set; }
        /// <summary>
        /// The best bid price period
        /// </summary>
        [ArrayProperty(3)]
        public decimal? BestBidPeriod { get; set; }
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
        /// The best bid price period
        /// </summary>
        [ArrayProperty(6)]
        public decimal? BestAskPeriod { get; set; }
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
        /// </summary>
        [ArrayProperty(14)]
        public string? PlaceHolder1 { get; set; }
        /// <summary>
        /// </summary>
        [ArrayProperty(15)]
        public string? PlaceHolder2 { get; set; }
        /// <summary>
        /// FRR amount available
        /// </summary>
        [ArrayProperty(16)]
        public decimal? FrrAmountAvailable { get; set; }
    }
}
