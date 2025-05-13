using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
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
