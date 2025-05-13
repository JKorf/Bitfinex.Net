using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
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
