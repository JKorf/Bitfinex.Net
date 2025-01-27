using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Order
    /// </summary>
    public enum Sorting
    {
        /// <summary>
        /// Newest first
        /// </summary>
        [Map("-1")]
        NewFirst,
        /// <summary>
        /// Oldest first
        /// </summary>
        [Map("1")]
        OldFirst
    }
}
