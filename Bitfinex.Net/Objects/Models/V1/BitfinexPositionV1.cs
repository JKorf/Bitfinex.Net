using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models.V1
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
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// The status of the position
        /// </summary>
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// The base 
        /// </summary>
        public decimal Base { get; set; }
        /// <summary>
        /// The quantity of the position
        /// </summary>
        [JsonProperty("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// The timestamp
        /// </summary>
        [JsonConverter(typeof(DateTimeConverter))]
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
