﻿using CryptoExchange.Net.Converters;
using System;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Account change log
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public record BitfinexChangeLog
    {
        /// <summary>
        /// Change timestamp
        /// </summary>
        [ArrayProperty(0), JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Event
        /// </summary>
        [ArrayProperty(2)]
        public string Log { get; set; } = string.Empty;
        /// <summary>
        /// IP address
        /// </summary>
        [ArrayProperty(5)]
        public string IpAddress { get; set; } = string.Empty;
        /// <summary>
        /// User agent
        /// </summary>
        [ArrayProperty(6)]
        public string UserAgent { get; set; } = string.Empty;
    }
}
