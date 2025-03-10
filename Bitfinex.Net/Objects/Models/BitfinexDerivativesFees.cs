using CryptoExchange.Net.Converters.SystemTextJson;
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
        /// Correction clamp min
        /// </summary>
        [JsonPropertyName("correction_clamp_min")]
        public decimal CorrectionClampMin { get; set; }
        /// <summary>
        /// Correction clamp insurance
        /// </summary>
        [JsonPropertyName("correction_clamp_insurance")]
        public decimal CorrectionClampInsurance { get; set; }
        /// <summary>
        /// Correction slope
        /// </summary>
        [JsonPropertyName("correction_slope")]
        public decimal CorrectionSlope { get; set; }
    }
}
