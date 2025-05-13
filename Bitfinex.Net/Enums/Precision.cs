using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Precision level
    /// </summary>
    [JsonConverter(typeof(EnumConverter<Precision>))]
    [SerializationModel]
    public enum Precision
    {
        /// <summary>
        /// 0
        /// </summary>
        [Map("P0")]
        PrecisionLevel0,
        /// <summary>
        /// 1
        /// </summary>
        [Map("P1")]
        PrecisionLevel1,
        /// <summary>
        /// 2
        /// </summary>
        [Map("P2")]
        PrecisionLevel2,
        /// <summary>
        /// 3
        /// </summary>
        [Map("P3")]
        PrecisionLevel3,
        /// <summary>
        /// R0
        /// </summary>
        [Map("R0")]
        R0
    }
}
