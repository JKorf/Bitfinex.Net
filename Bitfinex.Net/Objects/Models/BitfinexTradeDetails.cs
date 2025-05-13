using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Details of a trade
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexTradeDetails>))]
    [SerializationModel]
    public record BitfinexTradeDetails
    {
        /// <summary>
        /// The id of the trade
        /// </summary>
        [ArrayProperty(0)]
        public long Id { get; set; }

        /// <summary>
        /// The pair the trade is for
        /// </summary>
        [ArrayProperty(1)]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The time the trade was created
        /// </summary>
        [ArrayProperty(2), JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The id of the order
        /// </summary>
        [ArrayProperty(3)]
        public long OrderId { get; set; }

        /// <summary>
        /// The executed quantity
        /// </summary>
        [JsonIgnore]
        public decimal Quantity => Math.Abs(QuantityRaw);

        /// <summary>
        /// The quantity as returned by the API, will be negative if it's a sell
        /// </summary>
        [ArrayProperty(4)]
        public decimal QuantityRaw { get; set; }

        /// <summary>
        /// The price of the trade
        /// </summary>
        [ArrayProperty(5)]
        public decimal Price { get; set; }

        /// <summary>
        /// The type of the order
        /// </summary>
        [ArrayProperty(6)]

        public OrderType? OrderType { get; set; }

        /// <summary>
        /// The price of the order
        /// </summary>
        [ArrayProperty(7)]
        public decimal? OrderPrice { get; set; }

        /// <summary>
        /// If was maker
        /// </summary>
        [ArrayProperty(8)]
        public bool? Maker { get; set; }

        /// <summary>
        /// The fee
        /// </summary>
        [ArrayProperty(9)]
        public decimal Fee { get; set; }

        /// <summary>
        /// The asset the fee is in
        /// </summary>
        [ArrayProperty(10)]
        public string FeeAsset { get; set; } = string.Empty;

        /// <summary>
        /// The client order id
        /// </summary>
        [ArrayProperty(11)]
        public long? ClientOrderId { get; set; }
    }
}
