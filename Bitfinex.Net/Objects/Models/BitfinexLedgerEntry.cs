using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
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
        /// The asset
        /// </summary>
        [ArrayProperty(1)]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// The timestamp of the event
        /// </summary>
        [ArrayProperty(3), JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// The change quantity
        /// </summary>
        [ArrayProperty(5)]
        public decimal Quantity { get; set; }
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
