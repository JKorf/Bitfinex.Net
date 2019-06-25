using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
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
