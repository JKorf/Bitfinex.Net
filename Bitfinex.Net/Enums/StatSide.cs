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
        /// ["<c>long</c>"] Long
        /// </summary>
        [Map("long")]
        Long,
        /// <summary>
        /// ["<c>short</c>"] Short
        /// </summary>
        [Map("short")]
        Short
    }
}
