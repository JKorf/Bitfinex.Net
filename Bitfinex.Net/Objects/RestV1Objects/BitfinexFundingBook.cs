using System;
using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.RestV1Objects
{
    /// <summary>
    /// Funding order book
    /// </summary>
    public class BitfinexFundingBook
    {
        /// <summary>
        /// The bids in the book
        /// </summary>
        public BitfinexFundingBookEntry[] Bids { get; set; }
        /// <summary>
        /// The asks in the book
        /// </summary>
        public BitfinexFundingBookEntry[] Asks { get; set; }
    }

    /// <summary>
    /// Funding order book entry
    /// </summary>
    public class BitfinexFundingBookEntry
    {
        /// <summary>
        /// The rate of the entry
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// The amount of the entry
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// The period in days
        /// </summary>
        public int Period { get; set; }
        /// <summary>
        /// The timestamp of the entry
        /// </summary>
        [JsonConverter(typeof(TimestampSecondsConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Whether the offer is at ffr
        /// </summary>
        [JsonConverter(typeof(StringToBoolConverter)), JsonProperty("frr")]
        public bool FlashReturnRate { get; set; }
    }
}
