using Bitfinex.Net.Enums;
using CryptoExchange.Net.Objects.Options;
using System;

namespace Bitfinex.Net.Objects.Options
{
    /// <summary>
    /// Options for the Bitfinex SymbolOrderBook
    /// </summary>
    public class BitfinexOrderBookOptions : OrderBookOptions
    {
        /// <summary>
        /// Default options for new order books
        /// </summary>
        public static BitfinexOrderBookOptions Default { get; set; } = new BitfinexOrderBookOptions();

        /// <summary>
        /// The precision level of the order book
        /// </summary>
        public Precision? Precision { get; set; }
        /// <summary>
        /// The limit of entries in the order book, either 1, 25, 100 or 250
        /// </summary>
        public int? Limit { get; set; }
        /// <summary>
        /// After how much time we should consider the connection dropped if no data is received for this time after the initial subscriptions
        /// </summary>
        public TimeSpan? InitialDataTimeout { get; set; }

        internal BitfinexOrderBookOptions Copy()
        {
            var options = Copy<BitfinexOrderBookOptions>();
            options.Precision = Precision;
            options.Limit = Limit;
            options.InitialDataTimeout = InitialDataTimeout;
            return options;
        }
    }
}
