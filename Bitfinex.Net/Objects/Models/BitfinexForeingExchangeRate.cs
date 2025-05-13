using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Exchange rate in a foreign currency
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexForeignExchangeRate>))]
    [SerializationModel]
    public record BitfinexForeignExchangeRate
    {
        /// <summary>
        /// The current exchange rate
        /// </summary>
        [ArrayProperty(0)]
        public decimal CurrentRate { get; set; }
    }
}
