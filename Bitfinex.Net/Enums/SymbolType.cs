using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Type of symbol
    /// </summary>
    [JsonConverter(typeof(EnumConverter<SymbolType>))]
    [SerializationModel]
    public enum SymbolType
    {
        /// <summary>
        /// Exchange
        /// </summary>
        [Map("exchange")]
        Exchange,
        /// <summary>
        /// Margin
        /// </summary>
        [Map("margin")]
        Margin,
        /// <summary>
        /// Futures
        /// </summary>
        [Map("futures")]
        Futures,
        /// <summary>
        /// Securities
        /// </summary>
        [Map("securities")]
        Securities
    }
}
