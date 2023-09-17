using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Side for stats
    /// </summary>
    public enum StatSide
    {
        /// <summary>
        /// Long
        /// </summary>
        [Map("long")]
        Long,
        /// <summary>
        /// Short
        /// </summary>
        [Map("short")]
        Short
    }
}
