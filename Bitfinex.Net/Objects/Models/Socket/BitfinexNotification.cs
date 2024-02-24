using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;

namespace Bitfinex.Net.Objects.Models.Socket
{
    [JsonConverter(typeof(ArrayConverter))]
    internal class BitfinexNotification<T>
    {
        [ArrayProperty(0)]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }
        [ArrayProperty(1)]
        public string Event { get; set; } = string.Empty;
        [ArrayProperty(4)]
        [JsonConversion]
        public T Data { get; set; } = default!;
        [ArrayProperty(6)]
        public string Result { get; set; } = string.Empty;
        [ArrayProperty(7)]
        public string? ErrorMessage { get; set; }
    }
}
