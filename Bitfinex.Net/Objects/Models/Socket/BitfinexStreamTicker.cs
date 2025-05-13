using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models.Socket
{
    /// <summary>
    /// Ticker information
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexStreamTicker>))]
    [SerializationModel]
    public record BitfinexStreamTicker
    {
        /// <summary>
        /// The best bid price
        /// </summary>
        [ArrayProperty(0)]
        public decimal BestBidPrice { get; set; }
        /// <summary>
        /// The best bid quantity
        /// </summary>
        [ArrayProperty(1)]
        public decimal BestBidQuantity { get; set; }
        /// <summary>
        /// The best ask price
        /// </summary>
        [ArrayProperty(2)]
        public decimal BestAskPrice { get; set; }
        /// <summary>
        /// The best ask quantity
        /// </summary>
        [ArrayProperty(3)]
        public decimal BestAskQuantity { get; set; }
        /// <summary>
        /// Change versus 24 hours ago
        /// </summary>
        [ArrayProperty(4)]
        public decimal DailyChange { get; set; }
        /// <summary>
        /// Change percentage versus 24 hours ago
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
