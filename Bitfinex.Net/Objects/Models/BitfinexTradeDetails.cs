using System;
using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Details of a trade
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexTradeDetails
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
        [ArrayProperty(4)]
        public decimal Quantity { get; set; }

        /// <summary>
        /// The price of the trade
        /// </summary>
        [ArrayProperty(5)]
        public decimal Price { get; set; }

        /// <summary>
        /// The type of the order
        /// </summary>
        [ArrayProperty(6), JsonConverter(typeof(OrderTypeConverter))]
        public OrderType? OrderType { get; set; }
        
        /// <summary>
        /// The price of the order
        /// </summary>
        [ArrayProperty(7)]
        public decimal? OrderPrice { get; set; }

        /// <summary>
        /// If was maker
        /// </summary>
        [ArrayProperty(8), JsonConverter(typeof(BoolToIntConverter))]
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
    }
}
