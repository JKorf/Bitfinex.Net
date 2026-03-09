using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Order
    /// </summary>
    [JsonConverter(typeof(EnumConverter<Sorting>))]
    [SerializationModel]
    public enum Sorting
    {
        /// <summary>
        /// ["<c>-1</c>"] Newest first
        /// </summary>
        [Map("-1")]
        NewFirst,
        /// <summary>
        /// ["<c>1</c>"] Oldest first
        /// </summary>
        [Map("1")]
        OldFirst
    }
}
