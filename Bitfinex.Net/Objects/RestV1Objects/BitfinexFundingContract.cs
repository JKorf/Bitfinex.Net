using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.RestV1Objects
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
        public long PositionId { get; set; }
        /// <summary>
        /// The currency of the contract
        /// </summary>
        public string Currency { get; set; } = "";
        /// <summary>
        /// The rate of the contract
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// The period in days
        /// </summary>
        public int Period { get; set; }
        /// <summary>
        /// The amount
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// The timestamp
        /// </summary>
        [JsonConverter(typeof(TimestampSecondsConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// If it is auto close
        /// </summary>
        [JsonProperty("auto_close")]
        public bool AutoClose { get; set; }
    }
}
