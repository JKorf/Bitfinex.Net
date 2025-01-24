using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Status of Bitfinex
    /// </summary>
    public enum PlatformStatus
    {
        /// <summary>
        /// In maintenance
        /// </summary>
        [Map("0")]
        Maintenance,
        /// <summary>
        /// Working normally
        /// </summary>
        [Map("1")]
        Operative
    }
}
