using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Status of an order
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Active
        /// </summary>
        [Map("ACTIVE")]
        Active,
        /// <summary>
        /// Fully filled
        /// </summary>
        [Map("EXECUTED")]
        Executed,
        /// <summary>
        /// Partially filled
        /// </summary>
        [Map("FORCED EXECUTED")]
        ForcefullyExecuted,
        /// <summary>
        /// Partially filled
        /// </summary>
        [Map("PARTIALLY FILLED")]
        PartiallyFilled,
        /// <summary>
        /// Canceled
        /// </summary>
        [Map("CANCELED")]
        Canceled,
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown
    }
}
