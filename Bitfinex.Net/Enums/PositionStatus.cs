using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Position status
    /// </summary>
    [JsonConverter(typeof(EnumConverter<PositionStatus>))]
    [SerializationModel]
    public enum PositionStatus
    {
        /// <summary>
        /// ["<c>ACTIVE</c>"] Active
        /// </summary>
        [Map("ACTIVE")]
        Active,
        /// <summary>
        /// ["<c>CLOSED</c>"] Closed
        /// </summary>
        [Map("CLOSED")]
        Closed
    }
}
