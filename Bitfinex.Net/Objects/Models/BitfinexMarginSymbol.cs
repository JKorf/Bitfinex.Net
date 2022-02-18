using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Margin symbol info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexMarginSymbol
    {
        /// <summary>
        /// The topic
        /// </summary>
        [ArrayProperty(0)]
        public string Topic { get; set; } = string.Empty;

        /// <summary>
        /// The symbol
        /// </summary>
        [ArrayProperty(1)]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Data
        /// </summary>
        [ArrayProperty(2), JsonConverter(typeof(ArrayConverter))]
        public BitfinexMarginSymbolInfo Data { get; set; } = default!;
    }

    /// <summary>
    /// Margin symbol info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexMarginSymbolInfo
    {
        /// <summary>
        /// Trade-able balance
        /// </summary>
        [ArrayProperty(0)]
        public decimal TradeableBalance { get; set; }

        /// <summary>
        /// Gross balance
        /// </summary>
        [ArrayProperty(1)]
        public decimal GrossBalance { get; set; }

        /// <summary>
        /// Buy quantity
        /// </summary>
        [ArrayProperty(2)]
        public decimal Buy { get; set; }

        /// <summary>
        /// Sell quantity
        /// </summary>
        [ArrayProperty(3)]
        public decimal Sell { get; set; }
    }
}
