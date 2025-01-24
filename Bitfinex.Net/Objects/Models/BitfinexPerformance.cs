﻿using System;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Performance info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public record BitfinexPerformance
    {
        /// <summary>
        /// The timestamp of the calculation
        /// </summary>
        [ArrayProperty(0), JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Performance rating
        /// </summary>
        [ArrayProperty(1)]
        public decimal Performance { get; set; }
    }
}
