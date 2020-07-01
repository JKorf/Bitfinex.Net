using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    /// <summary>
    /// Performance info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexPerformance
    {
        /// <summary>
        /// The timestamp of the calculation
        /// </summary>
        [ArrayProperty(0), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Performance rating
        /// </summary>
        [ArrayProperty(1)]
        public decimal Performance { get; set; }
    }
}
