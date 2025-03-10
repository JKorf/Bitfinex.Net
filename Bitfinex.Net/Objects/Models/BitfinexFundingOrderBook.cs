using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Order book
    /// </summary>
    [SerializationModel]
    public record BitfinexFundingOrderBook
    {
        /// <summary>
        /// List of bids
        /// </summary>
        public BitfinexOrderBookFundingEntry[] Bids { get; set; } = Array.Empty<BitfinexOrderBookFundingEntry>();
        /// <summary>
        /// List of asks
        /// </summary>
        public BitfinexOrderBookFundingEntry[] Asks { get; set; } = Array.Empty<BitfinexOrderBookFundingEntry>();
    }
}
