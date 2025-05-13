using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Frequency of updates
    /// </summary>
    [JsonConverter(typeof(EnumConverter<Frequency>))]
    [SerializationModel]
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
