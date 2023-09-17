using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Type of symbol
    /// </summary>
    public enum SymbolType
    {
        /// <summary>
        /// Exchange
        /// </summary>
        [Map("exchange")]
        Exchange,
        /// <summary>
        /// Margin
        /// </summary>
        [Map("margin")]
        Margin,
        /// <summary>
        /// Futures
        /// </summary>
        [Map("futures")]
        Futures,
        /// <summary>
        /// Securities
        /// </summary>
        [Map("securities")]
        Securities
    }
}
