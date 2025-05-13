using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Trade info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexTradeSimple>))]
    [SerializationModel]
    public record BitfinexTradeSimple
    {
        /// <summary>
        /// The id of the trade
        /// </summary>
        [ArrayProperty(0)]
        public long Id { get; set; }
        /// <summary>
        /// The timestamp of the trade
        /// </summary>
        [ArrayProperty(1), JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// The quantity of the trade
        /// </summary>
        [ArrayProperty(2)]
        public decimal Quantity { get; set; }
        /// <summary>
        /// The price of the trade
        /// </summary>
        [ArrayProperty(3)]
        public decimal Price { get; set; }
        /// <summary>
        /// Amount of time the funding transaction was for
        /// </summary>
        [ArrayProperty(4)]
        public int? Period { get; set; }
        /// <summary>
        /// Quantity as a positive number
        /// </summary>
        public decimal QuantityAbs => Math.Abs(Quantity);
    }
}
