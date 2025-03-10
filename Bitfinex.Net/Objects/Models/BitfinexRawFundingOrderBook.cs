using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Order book
    /// </summary>
    [SerializationModel]
    public record BitfinexRawFundingOrderBook
    {
        /// <summary>
        /// List of bids
        /// </summary>
        public BitfinexRawOrderBookFundingEntry[] Bids { get; set; } = Array.Empty<BitfinexRawOrderBookFundingEntry>();
        /// <summary>
        /// List of asks
        /// </summary>
        public BitfinexRawOrderBookFundingEntry[] Asks { get; set; } = Array.Empty<BitfinexRawOrderBookFundingEntry>();
    }
}
