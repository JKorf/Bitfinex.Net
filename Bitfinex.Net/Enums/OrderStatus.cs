using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Status of an order
    /// </summary>
    [JsonConverter(typeof(EnumConverter<OrderStatus>))]
    [SerializationModel]
    public enum OrderStatus
    {
        /// <summary>
        /// ["<c>ACTIVE</c>"] Active
        /// </summary>
        [Map("ACTIVE")]
        Active,
        /// <summary>
        /// ["<c>EXECUTED</c>"] Fully filled
        /// </summary>
        [Map("EXECUTED")]
        Executed,
        /// <summary>
        /// ["<c>FORCED EXECUTED</c>"] Partially filled
        /// </summary>
        [Map("FORCED EXECUTED")]
        ForcefullyExecuted,
        /// <summary>
        /// ["<c>PARTIALLY FILLED</c>"] Partially filled
        /// </summary>
        [Map("PARTIALLY FILLED")]
        PartiallyFilled,
        /// <summary>
        /// ["<c>CANCELED</c>"] Canceled
        /// </summary>
        [Map("CANCELED")]
        Canceled,
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown
    }
}
