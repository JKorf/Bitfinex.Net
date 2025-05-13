using CryptoExchange.Net.Converters.SystemTextJson;
using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Interfaces;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Base for order book
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexOrderBookBase>))]
    [SerializationModel]
    public record BitfinexOrderBookBase { }

    /// <summary>
    /// Order book entry
    /// </summary>
    [JsonConverter(typeof(OrderBookEntryConverter))]
    [SerializationModel]
    public record BitfinexOrderBookEntry: BitfinexOrderBookBase, ISymbolOrderBookEntry
    {
        /// <summary>
        /// The price of this entry
        /// </summary>
        [ArrayProperty(0)]
        public decimal Price { get; set; }
        /// <summary>
        /// The amount of orders for this price
        /// </summary>
        [ArrayProperty(1)]
        public int Count { get; set; }
        /// <summary>
        /// The total quantity for this price
        /// </summary>
        [ArrayProperty(2)]
        public decimal Quantity { get; set; }

        internal string RawPrice { get; set; } = string.Empty;

        internal string RawQuantity { get; set; } = string.Empty;
    }

    /// <summary>
    /// Order book funding entry
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexOrderBookFundingEntry>))]
    [SerializationModel]
    public record BitfinexOrderBookFundingEntry : BitfinexOrderBookBase, ISymbolOrderBookEntry
    {
        /// <summary>
        /// The price of this entry
        /// </summary>
        [ArrayProperty(0)]
        public decimal Price { get; set; }
        /// <summary>
        /// Period level
        /// </summary>
        [ArrayProperty(1)]
        public int Period { get; set; }
        /// <summary>
        /// The amount of orders for this price
        /// </summary>
        [ArrayProperty(2)]
        public int Count { get; set; }
        /// <summary>
        /// The total quantity for this price
        /// </summary>
        [ArrayProperty(3)]
        public decimal Quantity { get; set; }

        internal string RawPrice { get; set; } = string.Empty;

        internal string RawQuantity { get; set; } = string.Empty;
    }

    /// <summary>
    /// Raw order book entry
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexRawOrderBookEntry>))]
    [SerializationModel]
    public record BitfinexRawOrderBookEntry : BitfinexOrderBookBase, ISymbolOrderBookEntry
    {
        /// <summary>
        /// The id of this order
        /// </summary>
        [ArrayProperty(0)]
        public long OrderId { get; set; }
        /// <summary>
        /// The price for this order
        /// </summary>
        [ArrayProperty(1)]
        public decimal Price { get; set; }
        /// <summary>
        /// The quantity of this order
        /// </summary>
        [ArrayProperty(2)]
        public decimal Quantity { get; set; }
    }

    /// <summary>
    /// Raw order book entry
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexRawOrderBookFundingEntry>))]
    [SerializationModel]
    public record BitfinexRawOrderBookFundingEntry : BitfinexOrderBookBase, ISymbolOrderBookEntry
    {
        /// <summary>
        /// The id of this order
        /// </summary>
        [ArrayProperty(0)]
        public long OrderId { get; set; }
        /// <summary>
        /// Period level
        /// </summary>
        [ArrayProperty(1)]
        public int Period { get; set; }
        /// <summary>
        /// The price for this order
        /// </summary>
        [ArrayProperty(2)]
        public decimal Price { get; set; }
        /// <summary>
        /// The quantity of this order
        /// </summary>
        [ArrayProperty(3)]
        public decimal Quantity { get; set; }
    }
}
