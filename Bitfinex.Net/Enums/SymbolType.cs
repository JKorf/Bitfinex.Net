using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Type of symbol
    /// </summary>
    [JsonConverter(typeof(EnumConverter<SymbolType>))]
    [SerializationModel]
    public enum SymbolType
    {
        /// <summary>
        /// ["<c>exchange</c>"] Exchange
        /// </summary>
        [Map("exchange")]
        Exchange,
        /// <summary>
        /// ["<c>margin</c>"] Margin
        /// </summary>
        [Map("margin")]
        Margin,
        /// <summary>
        /// ["<c>futures</c>"] Futures
        /// </summary>
        [Map("futures")]
        Futures,
        /// <summary>
        /// ["<c>securities</c>"] Securities
        /// </summary>
        [Map("securities")]
        Securities
    }
}
