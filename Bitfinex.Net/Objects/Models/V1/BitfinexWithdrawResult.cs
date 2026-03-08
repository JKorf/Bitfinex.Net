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
        /// ["<c>status</c>"] Status string
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>message</c>"] Additional info
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>withdrawal_id</c>"] The id of the withdrawal
        /// </summary>
        [JsonPropertyName("withdrawal_id")]
        public long WithdrawalId { get; set; }
        /// <summary>
        /// ["<c>fees</c>"] The fees paid
        /// </summary>
        [JsonPropertyName("fees")]
        public decimal Fees { get; set; }
    }
}
