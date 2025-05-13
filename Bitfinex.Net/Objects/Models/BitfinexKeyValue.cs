using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    [JsonConverter(typeof(ArrayConverter<BitfinexStringKeyValue>))]
    [SerializationModel]
    internal record BitfinexStringKeyValue
    {
        [ArrayProperty(0)]
        public string Key { get; set; } = string.Empty;
        [ArrayProperty(1)]
        [JsonConversion]
        public string Value { get; set; } = default!;
    }

    [JsonConverter(typeof(ArrayConverter<BitfinexStringArrayKeyValue>))]
    [SerializationModel]
    internal record BitfinexStringArrayKeyValue
    {
        [ArrayProperty(0)]
        public string Key { get; set; } = string.Empty;
        [ArrayProperty(1)]
        [JsonConversion]
        public string[] Value { get; set; } = [];
    }

    [JsonConverter(typeof(ArrayConverter<BitfinexDecimalArrayKeyValue>))]
    [SerializationModel]
    internal record BitfinexDecimalArrayKeyValue
    {
        [ArrayProperty(0)]
        public string Key { get; set; } = string.Empty;
        [ArrayProperty(1)]
        [JsonConversion]
        public decimal[] Value { get; set; } = [];
    }

    [JsonConverter(typeof(ArrayConverter<BitfinexSymbolInfoKeyValue>))]
    [SerializationModel]
    internal record BitfinexSymbolInfoKeyValue
    {
        [ArrayProperty(0)]
        public string Key { get; set; } = string.Empty;
        [ArrayProperty(1)]
        [JsonConversion]
        public BitfinexSymbolInfo Value { get; set; } = default!;
    }
}
