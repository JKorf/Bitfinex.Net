using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Frequency of updates
    /// </summary>
    public enum Frequency
    {
        /// <summary>
        /// Realtime
        /// </summary>
        [Map("F0")]
        Realtime,
        /// <summary>
        /// Two seconds
        /// </summary>
        [Map("F1")]
        TwoSeconds
    }
}
