using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Type of position
    /// </summary>
    [JsonConverter(typeof(EnumConverter<PositionType>))]
    [SerializationModel]
    public enum PositionType
    {
        /// <summary>
        /// Margin position
        /// </summary>
        [Map("0")]
        MarginPosition,
        /// <summary>
        /// Derivatives position
        /// </summary>
        [Map("1")]
        DerivativesPosition
    }
}
