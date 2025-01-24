using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Funding type
    /// </summary>
    public enum FundingType
    {
        /// <summary>
        /// Lend
        /// </summary>
        [Map("lend")] 
        Lend,
        /// <summary>
        /// Loan
        /// </summary>
        [Map("loan")]
        Loan
    }
}
