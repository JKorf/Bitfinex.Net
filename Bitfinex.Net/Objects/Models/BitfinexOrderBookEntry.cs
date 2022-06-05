using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Interfaces;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Base for order book
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexOrderBookBase { }

    /// <summary>
    /// Order book entry
    /// </summary>
    [JsonConverter(typeof(OrderBookEntryConverter))]
    public class BitfinexOrderBookEntry: BitfinexOrderBookBase, ISymbolOrderBookEntry
    {
        /// <summary>
        /// The price of this entry
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// The period of this entry
        /// </summary>
        public decimal? Period { get; set; }
        /// <summary>
        /// The amount of orders for this price
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// The total quantity for this price
        /// </summary>
        public decimal Quantity { get; set; }

        internal string RawPrice { get; set; } = string.Empty;

        internal string RawQuantity { get; set; } = string.Empty;
    }

    /// <summary>
    /// Raw order book entry
    /// </summary>
    [JsonConverter(typeof(RawOrderBookEntryConverter))]
    public class BitfinexRawOrderBookEntry : BitfinexOrderBookBase, ISymbolOrderBookEntry
    {
        /// <summary>
        /// The id of this order
        /// </summary>
        public long OrderId { get; set; }
        /// <summary>
        /// The price for this order
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// The period of this entry
        /// </summary>
        public decimal? Period { get; set; }
        /// <summary>
        /// The quantity of this order
        /// </summary>
        public decimal Quantity { get; set; }
    }
}
