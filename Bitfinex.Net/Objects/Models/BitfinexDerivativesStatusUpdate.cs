using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Converters;
using System;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Derivatives status
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexDerivativesStatusUpdate>))]
    [SerializationModel]
    public record BitfinexDerivativesStatusUpdate
    {
        /// <summary>
        /// Timestamp
        /// </summary>
        [ArrayProperty(0)]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Derivative book mid price
        /// </summary>
        [ArrayProperty(2)]
        public decimal Price { get; set; }
        /// <summary>
        /// Book mid price of the underlying Bitfinex spot trading pair
        /// </summary>
        [ArrayProperty(3)]
        public decimal SpotPrice { get; set; }
        /// <summary>
        /// The balance available to the liquidation engine to absorb losses
        /// </summary>
        [ArrayProperty(5)]
        public decimal InsuranceFundBalance { get; set; }
        /// <summary>
        /// Next funding event timestamp
        /// </summary>
        [ArrayProperty(7)]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime NextFundingTime { get; set; }
        /// <summary>
        /// Current accrued funding for next 8h period
        /// </summary>
        [ArrayProperty(8)]
        public decimal NextFundingAccrued { get; set; }
        /// <summary>
        /// Accruel counter
        /// </summary>
        [ArrayProperty(9)]
        public int NextFundingStep { get; set; }
        /// <summary>
        /// Funding applied in the current 8h period
        /// </summary>
        [ArrayProperty(11)]
        public decimal CurrentFunding { get; set; }
        /// <summary>
        /// Price based on the BFX Composite Index
        /// </summary>
        [ArrayProperty(14)]
        public decimal MarkPrice { get; set; }
        /// <summary>
        /// Total number of outstanding derivative contracts
        /// </summary>
        [ArrayProperty(17)]
        public decimal OpenInterest { get; set; }
        /// <summary>
        /// Range in the average spread that does not require a funding payment
        /// </summary>
        [ArrayProperty(21)]
        public decimal ClampMin { get; set; }
        /// <summary>
        /// Funding payment cap
        /// </summary>
        [ArrayProperty(22)]
        public decimal ClampMax { get; set; }
    }
}
