using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Stat info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexStats>))]
    [SerializationModel]
    public record BitfinexStats
    {
        /// <summary>
        /// The timestamp
        /// </summary>
        [ArrayProperty(0), JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The value
        /// </summary>
        [ArrayProperty(1)]
        public decimal Value { get; set; }
    }
}
