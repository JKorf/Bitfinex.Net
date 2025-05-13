using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects.Internal;
using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Order info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexOrder>))]
    [SerializationModel]
    public record BitfinexOrder
    {
        /// <summary>
        /// The id of the order
        /// </summary>
        [ArrayProperty(0)]
        public long Id { get; set; }

        /// <summary>
        /// The group id of the order
        /// </summary>
        [ArrayProperty(1)]
        public long? GroupId { get; set; }

        /// <summary>
        /// The client order id
        /// </summary>
        [ArrayProperty(2)]
        public long? ClientOrderId { get; set; }

        /// <summary>
        /// The symbol of the order
        /// </summary>
        [ArrayProperty(3)]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The creation time of the order
        /// </summary>
        [ArrayProperty(4), JsonConverter(typeof(DateTimeConverter))]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// The last update time
        /// </summary>
        [ArrayProperty(5), JsonConverter(typeof(DateTimeConverter))]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// The original quantity
        /// </summary>
        [JsonIgnore]
        public decimal QuantityRemaining => Math.Abs(QuantityRemainingRaw);

        /// <summary>
        /// The quantity left
        /// </summary>
        [ArrayProperty(6)]
        public decimal QuantityRemainingRaw { get; set; }

        /// <summary>
        /// The side of the order
        /// </summary>
        [JsonIgnore]
        public OrderSide Side => QuantityRaw < 0 ? OrderSide.Sell : OrderSide.Buy;

        /// <summary>
        /// The original quantity
        /// </summary>
        [JsonIgnore]
        public decimal Quantity => Math.Abs(QuantityRaw);

        /// <summary>
        /// The original quantity as returned by the API, will be negative if it's a sell
        /// </summary>
        [ArrayProperty(7)]
        public decimal QuantityRaw { get; set; }

        /// <summary>
        /// The order type
        /// </summary>
        [ArrayProperty(8)]

        public OrderType Type { get; set; }

        /// <summary>
        /// The previous order type
        /// </summary>
        [ArrayProperty(9)]
        public OrderType? TypePrevious { get; set; }

        /// <summary>
        /// Tif timestamp
        /// </summary>
        [ArrayProperty(10), JsonConverter(typeof(DateTimeConverter))]
        public DateTime? TimestampTif { get; set; }

        /// <summary>
        /// Order flags
        /// </summary>
        [ArrayProperty(12)]
        public OrderFlags? Flags { get; set; }

        /// <summary>
        /// The order status
        /// </summary>
        [ArrayProperty(13)]
        [JsonConverter(typeof(OrderStatusConverter))]
        public OrderStatus Status { get; set; }

        /// <summary>
        /// The raw status string
        /// </summary>
        [ArrayProperty(13)]
        public string StatusString { get; set; } = string.Empty;

        /// <summary>
        /// The price of the order
        /// </summary>
        [ArrayProperty(16)]
        public decimal Price { get; set; }

        /// <summary>
        /// The average price of the order (market orders)
        /// </summary>
        [ArrayProperty(17)]
        public decimal? PriceAverage { get; set; }

        /// <summary>
        /// The trailing price of the order
        /// </summary>
        [ArrayProperty(18)]
        public decimal PriceTrailing { get; set; }

        /// <summary>
        /// The aux limit price
        /// </summary>
        [ArrayProperty(19)]
        public decimal PriceAuxilliaryLimit { get; set; }

        /// <summary>
        /// True if operations on order must trigger a notification, false if operations on order must not trigger a notification
        /// </summary>
        [ArrayProperty(23)]
        public bool Notify { get; set; }
        /// <summary>
        /// Whether the order is hidden
        /// </summary>
        [ArrayProperty(24)]
        public bool Hidden { get; set; }
        /// <summary>
        /// If another order caused this order to be placed (OCO) this will be that other order's ID
        /// </summary>
        [ArrayProperty(25)]
        public long? PlacedId { get; set; }
        /// <summary>
        /// Indicates origin of action: BFX, API>BFX
        /// </summary>
        [ArrayProperty(28)]
        public string? Routing { get; set; }
        /// <summary>
        /// Meta data for the order
        /// </summary>
        [ArrayProperty(31)]
        [JsonConversion]
        public BitfinexMeta? Meta { get; set; }
    }
}
