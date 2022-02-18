using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Exchange rate in a foreign currency
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexForeignExchangeRate
    {
        /// <summary>
        /// The current exchange rate
        /// </summary>
        [ArrayProperty(0)]
        public decimal CurrentRate { get; set; }
    }
}
