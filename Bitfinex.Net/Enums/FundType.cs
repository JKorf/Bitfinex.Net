namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Fund type
    /// </summary>
    public enum FundType
    {
        /// <summary>
        /// Credit
        /// </summary>
        [JsonPropertyName("credit")]
        Credit,
        /// <summary>
        /// Loan
        /// </summary>
        [JsonPropertyName("loan")]
        Loan
    }
}
