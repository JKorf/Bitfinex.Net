using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    /// <summary>
    /// Market overview
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexSymbolOverview
    {
        /// <summary>
        /// The symbol
        /// </summary>
        [ArrayProperty(0)]
        public string Symbol { get; set; } = "";
        /// <summary>
        /// The best bid price
        /// </summary>
        [ArrayProperty(1)]
        public decimal Bid { get; set; }
        /// <summary>
        /// The best bid size
        /// </summary>
        [ArrayProperty(2)]
        public decimal BidSize { get; set; }
        /// <summary>
        /// The best ask price
        /// </summary>
        [ArrayProperty(3)]
        public decimal Ask { get; set; }
        /// <summary>
        /// The best ask size
        /// </summary>
        [ArrayProperty(4)]
        public decimal AskSize { get; set; }
        /// <summary>
        /// Change versus 24 hours ago
        /// </summary>
        [ArrayProperty(5)]
        public decimal DailyChange { get; set; }
        /// <summary>
        /// Change percentage versus 24 hours ago
        /// </summary>
        [ArrayProperty(6)]
        public decimal DailyChangePercentage { get; set; }
        /// <summary>
        /// The last trade price
        /// </summary>
        [ArrayProperty(7)]
        public decimal LastPrice { get; set; }
        /// <summary>
        /// The 24 hour volume
        /// </summary>
        [ArrayProperty(8)]
        public decimal Volume { get; set; }
        /// <summary>
        /// The 24 hour high price
        /// </summary>
        [ArrayProperty(9)]
        public decimal High { get; set; }
        /// <summary>
        /// The 24 hour low price
        /// </summary>
        [ArrayProperty(10)]
        public decimal Low { get; set; }
    }
}
