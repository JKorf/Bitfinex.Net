using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models.Socket
{
    /// <summary>
    /// Market overview
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexStreamSymbolOverview
    {
        /// <summary>
        /// Best bid price
        /// </summary>
        [ArrayProperty(0)]
        public decimal BestBidPrice { get; set; }
        /// <summary>
        /// Best bid quantity
        /// </summary>
        [ArrayProperty(1)]
        public decimal BestBidQuantity { get; set; }
        /// <summary>
        /// Best ask price
        /// </summary>
        [ArrayProperty(2)]
        public decimal BestAskPrice { get; set; }
        /// <summary>
        /// Best ask quantity
        /// </summary>
        [ArrayProperty(3)]
        public decimal BestAskQuantity { get; set; }
        /// <summary>
        /// Change versus 24 hours ago
        /// </summary>
        [ArrayProperty(4)]
        public decimal DailyChange { get; set; }
        /// <summary>
        /// Change factor versus 24 hours ago. x100 for percentage.
        /// </summary>
        [ArrayProperty(5)]
        public decimal DailyChangePercentage { get; set; }
        /// <summary>
        /// The last trade price
        /// </summary>
        [ArrayProperty(6)]
        public decimal LastPrice { get; set; }
        /// <summary>
        /// The 24 hour volume
        /// </summary>
        [ArrayProperty(7)]
        public decimal Volume { get; set; }
        /// <summary>
        /// The 24 hour high price
        /// </summary>
        [ArrayProperty(8)]
        public decimal HighPrice { get; set; }
        /// <summary>
        /// The 24 hour low price
        /// </summary>
        [ArrayProperty(9)]
        public decimal LowPrice { get; set; }

    }
}
