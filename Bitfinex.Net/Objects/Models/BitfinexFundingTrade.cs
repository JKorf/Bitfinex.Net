using System;
using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Funding trade info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexFundingTrade
    {
        /// <summary>
        /// The id
        /// </summary>
        [ArrayProperty(0)]
        public long Id { get; set; }
        /// <summary>
        /// The asset
        /// </summary>
        [ArrayProperty(1)]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// The timestamp
        /// </summary>
        [ArrayProperty(2), JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// The offer id
        /// </summary>
        [ArrayProperty(3)]
        public long OfferId { get; set; }
        /// <summary>
        /// The trade quantity
        /// </summary>
        [ArrayProperty(4)]
        public decimal Quantity { get; set; }
        /// <summary>
        /// The rate
        /// </summary>
        [ArrayProperty(5)]
        public decimal Rate { get; set; }
        /// <summary>
        /// The period in days
        /// </summary>
        [ArrayProperty(6)]
        public int Period { get; set; }
        /// <summary>
        /// Whether the offer took liquidity off the funding book
        /// </summary>
        [ArrayProperty(7), JsonConverter(typeof(BoolToIntConverter))]
        public bool Maker { get; set; }
    }
}
