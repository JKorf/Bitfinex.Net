using CryptoExchange.Net.Converters.SystemTextJson;
namespace Bitfinex.Net.Objects.Models.V1
{
    /// <summary>
    /// Result of withdrawing
    /// </summary>
    [SerializationModel]
    public record BitfinexWithdrawalResult
    {
        /// <summary>
        /// The status of the transfer
        /// </summary>
        [JsonIgnore]
        public bool Success => Status == "success";
        /// <summary>
        /// Status string
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// Additional info
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// The id of the withdrawal
        /// </summary>
        [JsonPropertyName("withdrawal_id")]
        public long WithdrawalId { get; set; }
        /// <summary>
        /// The fees paid
        /// </summary>
        [JsonPropertyName("fees")]
        public decimal Fees { get; set; }
    }
}
