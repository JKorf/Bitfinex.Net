namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Derivatives fee info
    /// </summary>
    [SerializationModel]
    public record BitfinexDerivativesFees
    {
        /// <summary>
        /// Perpetual derivatives fee config
        /// </summary>
        [JsonPropertyName("deriv_perp")]
        public BitfinexPerpDerivativesFees PerpetualDerivatives { get; set; } = null!;
    }

    /// <summary>
    /// Perp derivatives fees
    /// </summary>
    [SerializationModel]
    public record BitfinexPerpDerivativesFees
    {
        /// <summary>
        /// ["<c>correction_clamp_min</c>"] Correction clamp min
        /// </summary>
        [JsonPropertyName("correction_clamp_min")]
        public decimal CorrectionClampMin { get; set; }
        /// <summary>
        /// ["<c>correction_clamp_insurance</c>"] Correction clamp insurance
        /// </summary>
        [JsonPropertyName("correction_clamp_insurance")]
        public decimal CorrectionClampInsurance { get; set; }
        /// <summary>
        /// ["<c>correction_slope</c>"] Correction slope
        /// </summary>
        [JsonPropertyName("correction_slope")]
        public decimal CorrectionSlope { get; set; }
    }
}
