using Newtonsoft.Json;

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
        [JsonProperty("credit")]
        Credit,
        /// <summary>
        /// Loan
        /// </summary>
        [JsonProperty("loan")]
        Loan
    }
}
