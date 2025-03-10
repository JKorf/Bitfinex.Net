using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Section of stats
    /// </summary>
    [JsonConverter(typeof(EnumConverter<StatSection>))]
    [SerializationModel]
    public enum StatSection
    {
        /// <summary>
        /// Last
        /// </summary>
        [Map("last")]
        Last,
        /// <summary>
        /// History
        /// </summary>
        [Map("hist")]
        History
    }
}
