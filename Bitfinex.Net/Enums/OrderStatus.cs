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
        Active,
        /// <summary>
        /// Fully filled
        /// </summary>
        Executed,
        /// <summary>
        /// Partially filled
        /// </summary>
        PartiallyFilled,
        /// <summary>
        /// Canceled
        /// </summary>
        Canceled,
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown
    }
}
