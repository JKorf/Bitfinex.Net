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
        /// ["<c>last</c>"] Last
        /// </summary>
        [Map("last")]
        Last,
        /// <summary>
        /// ["<c>hist</c>"] History
        /// </summary>
        [Map("hist")]
        History
    }
}
