using System;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Order book
    /// </summary>
    public class BitfinexRawOrderBook
    {
        /// <summary>
        /// List of bids
        /// </summary>
        public IEnumerable<BitfinexRawOrderBookEntry> Bids { get; set; } = Array.Empty<BitfinexRawOrderBookEntry>();
        /// <summary>
        /// List of asks
        /// </summary>
        public IEnumerable<BitfinexRawOrderBookEntry> Asks { get; set; } = Array.Empty<BitfinexRawOrderBookEntry>();
    }
}
