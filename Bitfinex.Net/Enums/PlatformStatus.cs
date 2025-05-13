using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Status of Bitfinex
    /// </summary>
    [JsonConverter(typeof(EnumConverter<PlatformStatus>))]
    [SerializationModel]
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
