﻿using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// History info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexTickerHistory
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [ArrayProperty(0)]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Best bid price at the time
        /// </summary>
        [ArrayProperty(1)]
        public decimal BestBidPrice { get; set; }
        /// <summary>
        /// Best ask price at the time
        /// </summary>
        [ArrayProperty(3)]
        public decimal BestAskPrice { get; set; }
        /// <summary>
        /// Timestamp
        /// </summary>
        [ArrayProperty(12)]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }
    }
}
