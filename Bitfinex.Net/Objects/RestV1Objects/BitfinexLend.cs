using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.RestV1Objects
{
    /// <summary>
    /// Lend info
    /// </summary>
    public class BitfinexLend
    {
        /// <summary>
        /// The rate of the lend
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// The amount that was lent
        /// </summary>
        [JsonProperty("amount_lent")]
        public decimal AmountLent { get; set; }
        /// <summary>
        /// The amount that is used
        /// </summary>
        [JsonProperty("amount_used")]
        public decimal AmountUsed { get; set; }
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonConverter(typeof(TimestampSecondsConverter))]
        public DateTime Timestamp { get; set; }
    }
}
