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
        public IEnumerable<ISymbolOrderBookEntry> Bids { get; set; } = Array.Empty<ISymbolOrderBookEntry>();
        /// <summary>
        /// List of asks
        /// </summary>
        public IEnumerable<ISymbolOrderBookEntry> Asks { get; set; } = Array.Empty<ISymbolOrderBookEntry>();
    }
}
