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
        public decimal? Frr { get; set; }
        /// <summary>
        /// The best bid price
        /// </summary>
        public decimal BestBidPrice { get; set; }
        /// <summary>
        /// The best bid price period
        /// </summary>
        public decimal? BestBidPeriod { get; set; }
        /// <summary>
        /// The best bid quantity
        /// </summary>
        public decimal BestBidQuantity { get; set; }
        /// <summary>
        /// The best ask price
        /// </summary>
        public decimal BestAskPrice { get; set; }
        /// <summary>
        /// The best bid price period
        /// </summary>
        public decimal? BestAskPeriod { get; set; }
        /// <summary>
        /// The best ask quantity
        /// </summary>
        public decimal BestAskQuantity { get; set; }
        /// <summary>
        /// Change versus 24 hours ago
        /// </summary>
        public decimal DailyChange { get; set; }
        /// <summary>
        /// Change percentage versus 24 hours ago
        /// </summary>
        public decimal DailyChangePercentage { get; set; }
        /// <summary>
        /// The last trade price
        /// </summary>
        public decimal LastPrice { get; set; }
        /// <summary>
        /// The 24 hour volume
        /// </summary>
        public decimal Volume { get; set; }
        /// <summary>
        /// The 24 hour high price
        /// </summary>
        public decimal HighPrice { get; set; }
        /// <summary>
        /// The 24 hour low price
        /// </summary>
        public decimal LowPrice { get; set; }
        /// <summary>
        /// </summary>
        public string? PlaceHolder1 { get; set; }
        /// <summary>
        /// </summary>
        public string? PlaceHolder2 { get; set; }
        /// <summary>
        /// FRR amount available
        /// </summary>
        public decimal? FrrAmountAvailable { get; set; }
    }
}
