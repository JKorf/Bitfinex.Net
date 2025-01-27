using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Position status
    /// </summary>
    public enum PositionStatus
    {
        /// <summary>
        /// Active
        /// </summary>
        [Map("ACTIVE")]
        Active,
        /// <summary>
        /// Closed
        /// </summary>
        [Map("CLOSED")]
        Closed
    }
}
