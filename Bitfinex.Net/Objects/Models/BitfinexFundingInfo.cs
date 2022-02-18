using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Funding info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexFundingInfo
    {
        /// <summary>
        /// Type
        /// </summary>
        [ArrayProperty(0)]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// The symbol
        /// </summary>
        [ArrayProperty(1)]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Data
        /// </summary>
        [ArrayProperty(2), JsonConverter(typeof(ArrayConverter))]
        public BitfinexFundingInfoDetails Data { get; set; } = default!;
    }

    /// <summary>
    /// Funding info details
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexFundingInfoDetails
    {
        /// <summary>
        /// The weighted average rate for taken funding
        /// </summary>
        [ArrayProperty(0)]
        public decimal YieldLoan { get; set; }
        /// <summary>
        /// The weighted average rate for provided funding
        /// </summary>
        [ArrayProperty(1)]
        public decimal YieldLend { get; set; }
        /// <summary>
        /// The weighted average duration for taken funding
        /// </summary>
        [ArrayProperty(2)]
        public decimal DurationLoan { get; set; }
        /// <summary>
        /// The weighted average duration for provided funding
        /// </summary>
        [ArrayProperty(3)]
        public decimal DurationLend { get; set; }
    }
}
