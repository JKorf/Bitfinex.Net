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
        /// ["<c>0</c>"] In maintenance
        /// </summary>
        [Map("0")]
        Maintenance,
        /// <summary>
        /// ["<c>1</c>"] Working normally
        /// </summary>
        [Map("1")]
        Operative
    }
}
