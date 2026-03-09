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
        /// ["<c>F0</c>"] Realtime
        /// </summary>
        [Map("F0")]
        Realtime,
        /// <summary>
        /// ["<c>F1</c>"] Two seconds
        /// </summary>
        [Map("F1")]
        TwoSeconds
    }
}
