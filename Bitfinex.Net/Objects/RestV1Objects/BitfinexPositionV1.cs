using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.RestV1Objects
{
    /// <summary>
    /// Position object of the V1 API
    /// </summary>
    public class BitfinexPositionV1
    {
        /// <summary>
        /// The id of the position
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// The symbol of the position
        /// </summary>
        public string Symbol { get; set; } = "";
        /// <summary>
        /// The status of the position
        /// </summary>
        public string Status { get; set; } = "";
        /// <summary>
        /// The base 
        /// </summary>
        public decimal Base { get; set; }
        /// <summary>
        /// The amount of the position
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// The timestamp
        /// </summary>
        [JsonConverter(typeof(TimestampSecondsConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Swap
        /// </summary>
        public decimal Swap { get; set; }
        /// <summary>
        /// The profit loss
        /// </summary>
        [JsonProperty("pl")]
        public decimal ProfitLoss { get; set; }
    }
}
