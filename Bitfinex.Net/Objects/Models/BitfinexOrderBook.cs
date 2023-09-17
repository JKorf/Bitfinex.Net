using System;
using System.Collections.Generic;
using CryptoExchange.Net.Interfaces;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Order book
    /// </summary>
    public class BitfinexOrderBook
    {
        /// <summary>
        /// List of bids
        /// </summary>
        public IEnumerable<BitfinexOrderBookEntry> Bids { get; set; } = Array.Empty<BitfinexOrderBookEntry>();
        /// <summary>
        /// List of asks
        /// </summary>
        public IEnumerable<BitfinexOrderBookEntry> Asks { get; set; } = Array.Empty<BitfinexOrderBookEntry>();
    }
}
