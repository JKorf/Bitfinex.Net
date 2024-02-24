using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
{
    [JsonConverter(typeof(ArrayConverter))]
    internal class BitfinexKeyValue<T>
    {
        [ArrayProperty(0)]
        public string Key { get; set; } = string.Empty;
        [ArrayProperty(1)]
        [JsonConversion]
        public T Value { get; set; } = default!;
    }
}
