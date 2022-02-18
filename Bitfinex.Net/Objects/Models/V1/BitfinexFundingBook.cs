using System;
using System.Collections.Generic;
using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models.V1
{
    /// <summary>
    /// Funding order book
    /// </summary>
    public class BitfinexFundingBook
    {
        /// <summary>
        /// The bids in the book
        /// </summary>
        public IEnumerable<BitfinexFundingBookEntry> Bids { get; set; } = Array.Empty<BitfinexFundingBookEntry>();
        /// <summary>
        /// The asks in the book
        /// </summary>
        public IEnumerable<BitfinexFundingBookEntry> Asks { get; set; } = Array.Empty<BitfinexFundingBookEntry>();
    }

    /// <summary>
    /// Funding order book entry
    /// </summary>
    public class BitfinexFundingBookEntry
    {
        /// <summary>
        /// The price of the entry
        /// </summary>
        [JsonProperty("rate")]
        public decimal Price { get; set; }
        /// <summary>
        /// The quantity of the entry
        /// </summary>
        [JsonProperty("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// The period in days
        /// </summary>
        public int Period { get; set; }
        /// <summary>
        /// The timestamp of the entry
        /// </summary>
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Whether the offer is at ffr
        /// </summary>
        [JsonConverter(typeof(StringToBoolConverter)), JsonProperty("frr")]
        public bool FlashReturnRate { get; set; }
    }
}
