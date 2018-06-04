using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexAvailableBalance
    {
        [ArrayProperty(0)]
        public decimal AvailableBalance { get; set; }
    }
}
