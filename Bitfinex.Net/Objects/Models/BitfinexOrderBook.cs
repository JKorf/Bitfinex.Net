using System;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Order book
    /// </summary>
    [SerializationModel]
    public record BitfinexOrderBook
    {
        /// <summary>
        /// List of bids
        /// </summary>
        public BitfinexOrderBookEntry[] Bids { get; set; } = Array.Empty<BitfinexOrderBookEntry>();
        /// <summary>
        /// List of asks
        /// </summary>
        public BitfinexOrderBookEntry[] Asks { get; set; } = Array.Empty<BitfinexOrderBookEntry>();
    }
}
