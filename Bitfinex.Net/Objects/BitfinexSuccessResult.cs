using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexSuccessResult
    {
        [ArrayProperty(0)]
        public bool Success { get; set; }
    }
}
