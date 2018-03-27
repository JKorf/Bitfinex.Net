using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexMarketAveragePrice
    {
        [ArrayProperty(0)]
        public decimal AverageRate { get; set; }

        [ArrayProperty(1)]
        public decimal Amount { get; set; }
    }
}
