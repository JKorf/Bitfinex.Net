﻿using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Position info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public record BitfinexPositionBasic
    {
        /// <summary>
        /// The symbol
        /// </summary>
        [ArrayProperty(0)]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The quantity
        /// </summary>
        [ArrayProperty(2)]
        public decimal Quantity { get; set; }

        /// <summary>
        /// The base price
        /// </summary>
        [ArrayProperty(3)]
        public decimal BasePrice { get; set; }
    }
}
