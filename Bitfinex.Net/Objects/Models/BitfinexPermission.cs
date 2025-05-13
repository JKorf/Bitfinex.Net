using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Permission info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexPermission>))]
    [SerializationModel]
    public record BitfinexPermission
    {
        /// <summary>
        /// Permission scope
        /// </summary>
        [ArrayProperty(0)]
        public string Scope { get; set; } = string.Empty;
        /// <summary>
        /// Has read permission
        /// </summary>
        [ArrayProperty(1)]
        public bool Read { get; set; }
        /// <summary>
        /// Has write permission
        /// </summary>
        [ArrayProperty(2)]
        public bool Write { get; set; }
    }
}
