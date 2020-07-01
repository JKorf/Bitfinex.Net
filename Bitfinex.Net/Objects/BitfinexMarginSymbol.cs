using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    /// <summary>
    /// Margin symbol info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexMarginSymbol
    {
        /// <summary>
        /// The symbol
        /// </summary>
        [ArrayProperty(0)]
        public string Symbol { get; set; } = "";

        /// <summary>
        /// Data
        /// </summary>
        [ArrayProperty(1), JsonConverter(typeof(ArrayConverter))]
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
        /// Buy amount
        /// </summary>
        [ArrayProperty(2)]
        public decimal Buy { get; set; }

        /// <summary>
        /// Sell amount
        /// </summary>
        [ArrayProperty(3)]
        public decimal Sell { get; set; }
    }
}
