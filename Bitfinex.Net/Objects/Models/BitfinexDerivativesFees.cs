using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Derivatives fee info
    /// </summary>
    public class BitfinexDerivativesFees
    {
        /// <summary>
        /// Perpetual derivatives fee config
        /// </summary>
        [JsonProperty("deriv_perp")]
        public BitfinexPerpDerivativesFees PerpetualDerivatives { get; set; } = null!;
    }

    /// <summary>
    /// Perp derivatives fees
    /// </summary>
    public class BitfinexPerpDerivativesFees
    {
        /// <summary>
        /// Correction clamp min
        /// </summary>
        [JsonProperty("correction_clamp_min")]
        public decimal CorrectionClampMin { get; set; }
        /// <summary>
        /// Correction clamp insurance
        /// </summary>
        [JsonProperty("correction_clamp_insurance")]
        public decimal CorrectionClampInsurance { get; set; }
        /// <summary>
        /// Correction slope
        /// </summary>
        [JsonProperty("correction_slope")]
        public decimal CorrectionSlope { get; set; }
    }
}
