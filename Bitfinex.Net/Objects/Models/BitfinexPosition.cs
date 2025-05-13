using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Position info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexPosition>))]
    [SerializationModel]
    public record BitfinexPosition
    {
        /// <summary>
        /// The symbol
        /// </summary>
        [ArrayProperty(0)]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The status of the position
        /// </summary>
        [ArrayProperty(1)]

        public PositionStatus Status { get; set; }

        /// <summary>
        /// The quantity
        /// </summary>
        [ArrayProperty(2)]
        public decimal Quantity { get; set; }

        /// <summary>
        /// The base price
        /// </summary>
        [ArrayProperty(3)]
        public decimal BasePrice { get; set; }

        /// <summary>
        /// The amount of funding being used
        /// </summary>
        [ArrayProperty(4)]
        public decimal MarginFunding { get; set; }

        /// <summary>
        /// The funding type
        /// </summary>
        [ArrayProperty(5)]

        public MarginFundingType MarginFundingType { get; set; }

        /// <summary>
        /// Profit / loss
        /// </summary>
        [ArrayProperty(6)]
        public decimal ProfitLoss { get; set; }

        /// <summary>
        /// Profit / loss percentage
        /// </summary>
        [ArrayProperty(7)]
        public decimal ProfitLossPercentage { get; set; }

        /// <summary>
        /// Liquidation price
        /// </summary>
        [ArrayProperty(8)]
        public decimal LiquidationPrice { get; set; }

        /// <summary>
        /// Beta value
        /// </summary>
        [ArrayProperty(9)]
        public decimal Leverage { get; set; }

        /// <summary>
        /// The id of the position
        /// </summary>
        [ArrayProperty(11)]
        public long Id { get; set; }

        /// <summary>
        /// The creation time
        /// </summary>
        [ArrayProperty(12), JsonConverter(typeof(DateTimeConverter))]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// The update time
        /// </summary>
        [ArrayProperty(13), JsonConverter(typeof(DateTimeConverter))]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// Type of position
        /// </summary>
        [ArrayProperty(15)]
        public PositionType? Type { get; set; }

        /// <summary>
        /// The amount of collateral applied to the open position
        /// </summary>
        [ArrayProperty(17)]
        public decimal? Collateral { get; set; }
        /// <summary>
        /// The minimum amount of collateral required for the position
        /// </summary>
        [ArrayProperty(18)]
        public decimal? MinCollateral { get; set; }
    }
}
