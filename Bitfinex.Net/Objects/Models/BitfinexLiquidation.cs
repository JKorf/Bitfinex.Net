using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Converters;
using System;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Liquidation info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexLiquidation>))]
    [SerializationModel]
    public record BitfinexLiquidation
    {
        /// <summary>
        /// Position id
        /// </summary>
        [ArrayProperty(1)]
        public long PositionId { get; set; }
        /// <summary>
        /// Timestamp
        /// </summary>
        [ArrayProperty(2)]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// The symbol
        /// </summary>
        [ArrayProperty(4)]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Size of the position. Positive values means a long position, negative values means a short position.
        /// </summary>
        [ArrayProperty(5)]
        public decimal Quantity { get; set; }
        /// <summary>
        /// The price at which user entered the position
        /// </summary>
        [ArrayProperty(6)]
        public decimal BasePrice { get; set; }
        /// <summary>
        /// False: initial liquidation trigger, true: market execution
        /// </summary>
        [ArrayProperty(8)]
        public bool IsMatch { get; set; }
        /// <summary>
        /// False: position acquired by the system, true: direct sell into the market
        /// </summary>
        [ArrayProperty(9)]
        public bool IsMarketSold { get; set; }
        /// <summary>
        /// The price at which the position has been acquired
        /// </summary>
        [ArrayProperty(11)]
        public decimal AquirePrice { get; set; }
    }
}
