using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexStats
    {
        /// <summary>
        /// The timestamp
        /// </summary>
        [ArrayProperty(0), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The value
        /// </summary>
        [ArrayProperty(1)]
        public decimal Value { get; set; }
    }
}
