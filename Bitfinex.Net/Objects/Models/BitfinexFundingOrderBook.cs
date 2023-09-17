using System;
using System.Collections.Generic;
using CryptoExchange.Net.Interfaces;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Order book
    /// </summary>
    public class BitfinexFundingOrderBook
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
