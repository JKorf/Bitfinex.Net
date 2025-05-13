using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Converters;
using System;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Derivatives status
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexDerivativesStatus>))]
    [SerializationModel]
    public record BitfinexDerivativesStatus
    {
        /// <summary>
        /// The derivative symbol
        /// </summary>
        [ArrayProperty(0)]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Timestamp
        /// </summary>
        [ArrayProperty(1)]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Derivative book mid price
        /// </summary>
        [ArrayProperty(3)]
        public decimal Price { get; set; }
        /// <summary>
        /// Book mid price of the underlying Bitfinex spot trading pair
        /// </summary>
        [ArrayProperty(4)]
        public decimal SpotPrice { get; set; }
        /// <summary>
        /// The balance available to the liquidation engine to absorb losses
        /// </summary>
        [ArrayProperty(6)]
        public decimal InsuranceFundBalance { get; set; }
        /// <summary>
        /// Next funding event timestamp
        /// </summary>
        [ArrayProperty(8)]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime NextFundingTime { get; set; }
        /// <summary>
        /// Current accrued funding for next 8h period
        /// </summary>
        [ArrayProperty(9)]
        public decimal NextFundingAccrued { get; set; }
        /// <summary>
        /// Accruel counter
        /// </summary>
        [ArrayProperty(10)]
        public int NextFundingStep { get; set; }
        /// <summary>
        /// Funding applied in the current 8h period
        /// </summary>
        [ArrayProperty(12)]
        public decimal CurrentFunding { get; set; }
        /// <summary>
        /// Price based on the BFX Composite Index
        /// </summary>
        [ArrayProperty(15)]
        public decimal MarkPrice { get; set; }
        /// <summary>
        /// Total number of outstanding derivative contracts
        /// </summary>
        [ArrayProperty(18)]
        public decimal OpenInterest { get; set; }
        /// <summary>
        /// Range in the average spread that does not require a funding payment
        /// </summary>
        [ArrayProperty(22)]
        public decimal ClampMin { get; set; }
        /// <summary>
        /// Funding payment cap
        /// </summary>
        [ArrayProperty(23)]
        public decimal ClampMax { get; set; }
    }
}
