using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
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
        /// Newest first
        /// </summary>
        [Map("-1")]
        NewFirst,
        /// <summary>
        /// Oldest first
        /// </summary>
        [Map("1")]
        OldFirst
    }
}
