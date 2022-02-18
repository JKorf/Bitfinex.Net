using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models.V1
{
    /// <summary>
    /// Result of withdrawing
    /// </summary>
    public class BitfinexWithdrawalResult
    {
        /// <summary>
        /// The status of the transfer
        /// </summary>
        [JsonConverter(typeof(StringToBoolConverter)), JsonProperty("status")]
        public bool Success { get; set; }
        /// <summary>
        /// Additional info
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// The id of the withdrawal
        /// </summary>
        [JsonProperty("withdrawal_id")]
        public long WithdrawalId { get; set; }
        /// <summary>
        /// The fees paid
        /// </summary>
        public decimal Fees { get; set; }
    }
}
