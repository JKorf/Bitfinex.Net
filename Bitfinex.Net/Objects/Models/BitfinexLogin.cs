﻿using CryptoExchange.Net.Converters;
using System;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Login info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public record BitfinexLogin
    {
        /// <summary>
        /// Login id
        /// </summary>
        [ArrayProperty(0)]
        public long Id { get; set; }
        /// <summary>
        /// Timestamp
        /// </summary>
        [ArrayProperty(2), JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Ip
        /// </summary>
        [ArrayProperty(4)]
        public string Ip { get; set; } = string.Empty;
        /// <summary>
        /// Extra info object
        /// </summary>
        [ArrayProperty(7)]
        public string ExtraInfo { get; set; } = string.Empty;
    }
}
