using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;
using System;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models.Socket
{
    internal record BitfinexNotification
    {
        [ArrayProperty(0)]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }
        [ArrayProperty(1)]
        public string Event { get; set; } = string.Empty;
        [ArrayProperty(6)]
        public string Result { get; set; } = string.Empty;
        [ArrayProperty(7)]
        public string? ErrorMessage { get; set; }
    }

    [SerializationModel]
    internal record BitfinexNotification<T> : BitfinexNotification
    {
        [ArrayProperty(4)]
        [JsonConversion]
        public T Data { get; set; } = default!;
    }

    [JsonConverter(typeof(ArrayConverter<BitfinexOrderNotification>))]
    [SerializationModel]
    internal record BitfinexOrderNotification : BitfinexNotification<BitfinexOrder> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexOrdersNotification>))]
    [SerializationModel]
    internal record BitfinexOrdersNotification : BitfinexNotification<BitfinexOrder[]> { }

    [JsonConverter(typeof(ArrayConverter<BitfinexFundingOfferNotification>))]
    [SerializationModel]
    internal record BitfinexFundingOfferNotification : BitfinexNotification<BitfinexFundingOffer> { }
}
