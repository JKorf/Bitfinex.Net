using System;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Order book
    /// </summary>
    public class BitfinexRawFundingOrderBook
    {
        /// <summary>
        /// List of bids
        /// </summary>
        public IEnumerable<BitfinexRawOrderBookFundingEntry> Bids { get; set; } = Array.Empty<BitfinexRawOrderBookFundingEntry>();
        /// <summary>
        /// List of asks
        /// </summary>
        public IEnumerable<BitfinexRawOrderBookFundingEntry> Asks { get; set; } = Array.Empty<BitfinexRawOrderBookFundingEntry>();
    }
}
