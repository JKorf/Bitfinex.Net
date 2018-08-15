using System;
using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexOrder
    {
        [ArrayProperty(0)]
        public long Id { get; set; }

        [ArrayProperty(1)]
        public long? GroupId { get; set; }

        [ArrayProperty(2)]
        public long? ClientOrderId { get; set; }

        [ArrayProperty(3)]
        public string Symbol { get; set; }

        [ArrayProperty(4), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampCreated { get; set; }

        [ArrayProperty(5), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampUpdated { get; set; }

        [ArrayProperty(6)]
        public decimal Amount { get; set; }

        [ArrayProperty(7)]
        public decimal AmountOriginal { get; set; }

        [ArrayProperty(8), JsonConverter(typeof(OrderTypeConverter))]
        public OrderType Type { get; set; }

        [ArrayProperty(9), JsonConverter(typeof(OrderTypeConverter))]
        public OrderType? TypePrevious { get; set; }

        [ArrayProperty(10)]
        public string PlaceHolder1 { get; set; }

        [ArrayProperty(11)]
        public string PlaceHolder2 { get; set; }

        [ArrayProperty(12)]
        public int? Flags { get; set; }

        [ArrayProperty(13), JsonConverter(typeof(OrderStatusConverter))]
        public OrderStatus Status { get; set; }
        [ArrayProperty(13)]
        public string StatusString { get; set; }
        [ArrayProperty(14)]
        public string PlaceHolder3 { get; set; }

        [ArrayProperty(15)]
        public string PlaceHolder4 { get; set; }

        [ArrayProperty(16)]
        public decimal Price { get; set; }

        [ArrayProperty(17)]
        public decimal? PriceAverage { get; set; }

        [ArrayProperty(18)]
        public decimal PriceTrailing { get; set; }

        [ArrayProperty(19)]
        public decimal PriceAuxilliaryLimit { get; set; }
        [ArrayProperty(20)]
        public string PlaceHolder5 { get; set; }
        [ArrayProperty(21)]
        public string PlaceHolder6 { get; set; }
        [ArrayProperty(22)]
        public string PlaceHolder7 { get; set; }

        [ArrayProperty(23)]
        public bool Notify { get; set; }
        [ArrayProperty(24)]
        public bool Hidden { get; set; }
        [ArrayProperty(25)]
        public long? PlacedId { get; set; }
    }
}
