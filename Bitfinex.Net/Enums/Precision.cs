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
        /// ["<c>P0</c>"] 0
        /// </summary>
        [Map("P0")]
        PrecisionLevel0,
        /// <summary>
        /// ["<c>P1</c>"] 1
        /// </summary>
        [Map("P1")]
        PrecisionLevel1,
        /// <summary>
        /// ["<c>P2</c>"] 2
        /// </summary>
        [Map("P2")]
        PrecisionLevel2,
        /// <summary>
        /// ["<c>P3</c>"] 3
        /// </summary>
        [Map("P3")]
        PrecisionLevel3,
        /// <summary>
        /// ["<c>R0</c>"] R0
        /// </summary>
        [Map("R0")]
        R0
    }
}
