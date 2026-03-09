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
        /// ["<c>0</c>"] Margin position
        /// </summary>
        [Map("0")]
        MarginPosition,
        /// <summary>
        /// ["<c>1</c>"] Derivatives position
        /// </summary>
        [Map("1")]
        DerivativesPosition
    }
}
