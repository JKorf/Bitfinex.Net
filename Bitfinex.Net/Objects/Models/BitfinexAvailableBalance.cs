using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Available balance
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexAvailableBalance>))]
    [SerializationModel]
    public record BitfinexAvailableBalance
    {
        /// <summary>
        /// The available balance
        /// </summary>
        [ArrayProperty(0)]
        public decimal AvailableBalance { get; set; }
    }
}
