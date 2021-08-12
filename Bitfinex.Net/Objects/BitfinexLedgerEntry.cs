using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    /// <summary>
    /// Ledger entry
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexLedgerEntry
    {
        /// <summary>
        /// The id of the entry
        /// </summary>
        [ArrayProperty(0)]
        public long Id { get; set; }
        /// <summary>
        /// The currency
        /// </summary>
        [ArrayProperty(1)]
        public string Currency { get; set; } = string.Empty;
        /// <summary>
        /// The timestamp of the event
        /// </summary>
        [ArrayProperty(3), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// The change amount
        /// </summary>
        [ArrayProperty(5)]
        public decimal Amount { get; set; }
        /// <summary>
        /// The new balance
        /// </summary>
        [ArrayProperty(6)]
        public decimal NewBalance { get; set; }
        /// <summary>
        /// The description of the event
        /// </summary>
        [ArrayProperty(8)]
        public string Description { get; set; } = string.Empty;
    }
}
