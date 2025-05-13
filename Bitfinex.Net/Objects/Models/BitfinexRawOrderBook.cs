using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Order book
    /// </summary>
    [SerializationModel]
    public record BitfinexRawOrderBook
    {
        /// <summary>
        /// List of bids
        /// </summary>
        public BitfinexRawOrderBookEntry[] Bids { get; set; } = Array.Empty<BitfinexRawOrderBookEntry>();
        /// <summary>
        /// List of asks
        /// </summary>
        public BitfinexRawOrderBookEntry[] Asks { get; set; } = Array.Empty<BitfinexRawOrderBookEntry>();
    }
}
