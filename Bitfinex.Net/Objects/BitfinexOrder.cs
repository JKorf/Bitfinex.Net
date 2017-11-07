using System;
using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexOrder
    {
        [BitfinexProperty(0)]
        public long Id { get; set; }

        [BitfinexProperty(1)]
        public int? GroupId { get; set; }

        [BitfinexProperty(2)]
        public long ClientOrderId { get; set; }

        [BitfinexProperty(3)]
        public string Symbol { get; set; }

        [BitfinexProperty(4), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampCreated { get; set; }

        [BitfinexProperty(5), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampUpdated { get; set; }

        [BitfinexProperty(6)]
        public double Amount { get; set; }

        [BitfinexProperty(7)]
        public double AmountOriginal { get; set; }

        [BitfinexProperty(8), JsonConverter(typeof(OrderTypeConverter))]
        public OrderType Type { get; set; }

        [BitfinexProperty(9), JsonConverter(typeof(OrderTypeConverter))]
        public OrderType? TypePrevious { get; set; }

        [BitfinexProperty(10)]
        internal string PlaceHolder1 { get; set; }

        [BitfinexProperty(11)]
        internal string PlaceHolder2 { get; set; }

        [BitfinexProperty(12)]
        public int? Flags { get; set; }

        [BitfinexProperty(13), JsonConverter(typeof(OrderStatusConverter))]
        public OrderStatus Status { get; set; }
        [BitfinexProperty(14)]
        internal string PlaceHolder3 { get; set; }

        [BitfinexProperty(15)]
        internal string PlaceHolder4 { get; set; }

        [BitfinexProperty(16)]
        public double Price { get; set; }

        [BitfinexProperty(17)]
        public double? PriceAverage { get; set; }

        [BitfinexProperty(18)]
        public double PriceTrailing { get; set; }

        [BitfinexProperty(19)]
        public double PriceAuxilliaryLimit { get; set; }
        [BitfinexProperty(20)]
        internal string PlaceHolder5 { get; set; }
        [BitfinexProperty(21)]
        internal string PlaceHolder6 { get; set; }
        [BitfinexProperty(22)]
        internal string PlaceHolder7 { get; set; }

        [BitfinexProperty(23)]
        public bool Notify { get; set; }
        [BitfinexProperty(24)]
        public bool Hidden { get; set; }
        [BitfinexProperty(25)]
        public int? PlacedId { get; set; }
    }
}
