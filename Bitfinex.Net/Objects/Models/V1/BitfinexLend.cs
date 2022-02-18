using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models.V1
{
    /// <summary>
    /// Lend info
    /// </summary>
    public class BitfinexLend
    {
        /// <summary>
        /// The rate of the lend
        /// </summary>
        [JsonProperty("rate")]
        public decimal Price { get; set; }
        /// <summary>
        /// The quantity that was lent
        /// </summary>
        [JsonProperty("amount_lent")]
        public decimal QuantityLent { get; set; }
        /// <summary>
        /// The quantity that is used
        /// </summary>
        [JsonProperty("amount_used")]
        public decimal QuantityUsed { get; set; }
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }
    }
}
