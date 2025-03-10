using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Side for stats
    /// </summary>
    [JsonConverter(typeof(EnumConverter<StatSide>))]
    [SerializationModel]
    public enum StatSide
    {
        /// <summary>
        /// Long
        /// </summary>
        [Map("long")]
        Long,
        /// <summary>
        /// Short
        /// </summary>
        [Map("short")]
        Short
    }
}
