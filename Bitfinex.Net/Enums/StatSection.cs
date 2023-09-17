using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Section of stats
    /// </summary>
    public enum StatSection
    {
        /// <summary>
        /// Last
        /// </summary>
        [Map("last")]
        Last,
        /// <summary>
        /// History
        /// </summary>
        [Map("hist")]
        History
    }
}
