using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Margin funding type
    /// </summary>
    public enum MarginFundingType
    {
        /// <summary>
        /// Daily
        /// </summary>
        [Map("0")]
        Daily,
        /// <summary>
        /// Term
        /// </summary>
        [Map("1")]
        Term
    }
}
