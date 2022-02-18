using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models.V1
{
    /// <summary>
    /// Funding contract info
    /// </summary>
    public class BitfinexFundingContract
    {
        /// <summary>
        /// The id of the contract
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// The id of the position
        /// </summary>
        [JsonProperty("position_id")]
        public long? PositionId { get; set; }
        /// <summary>
        /// The asset of the contract
        /// </summary>
        [JsonProperty("currency")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// The rate of the contract
        /// </summary>
        [JsonProperty("rate")]
        public decimal Price { get; set; }
        /// <summary>
        /// The period in days
        /// </summary>
        public int Period { get; set; }
        /// <summary>
        /// The quantity
        /// </summary>
        [JsonProperty("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// The timestamp
        /// </summary>
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// If it is auto close
        /// </summary>
        [JsonProperty("auto_close")]
        public bool AutoClose { get; set; }
    }
}
