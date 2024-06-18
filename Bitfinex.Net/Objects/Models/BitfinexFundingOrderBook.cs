using System;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Order book
    /// </summary>
    public record BitfinexFundingOrderBook
    {
        /// <summary>
        /// List of bids
        /// </summary>
        public IEnumerable<BitfinexOrderBookFundingEntry> Bids { get; set; } = Array.Empty<BitfinexOrderBookFundingEntry>();
        /// <summary>
        /// List of asks
        /// </summary>
        public IEnumerable<BitfinexOrderBookFundingEntry> Asks { get; set; } = Array.Empty<BitfinexOrderBookFundingEntry>();
    }
}
