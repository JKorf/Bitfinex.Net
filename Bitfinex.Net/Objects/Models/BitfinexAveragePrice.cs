using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Average price info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexAveragePrice>))]
    [SerializationModel]
    public record BitfinexAveragePrice
    {
        /// <summary>
        /// The average price
        /// </summary>
        [ArrayProperty(0)]
        public decimal AverageRate { get; set; }

        /// <summary>
        /// The quantity
        /// </summary>
        [ArrayProperty(1)]
        public decimal Quantity { get; set; }
    }
}
