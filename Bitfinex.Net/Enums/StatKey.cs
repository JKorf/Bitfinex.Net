using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Stat types
    /// </summary>
    [JsonConverter(typeof(EnumConverter<StatKey>))]
    [SerializationModel]
    public enum StatKey
    {
        /// <summary>
        /// ["<c>pos.size</c>"] Total number of open positions
        /// </summary>
        [Map("pos.size")]
        TotalOpenPosition,
        /// <summary>
        /// ["<c>funding.size</c>"] Total active funding
        /// </summary>
        [Map("funding.size")]
        TotalActiveFunding,
        /// <summary>
        /// ["<c>credits.size</c>"] Active funding in positions
        /// </summary>
        [Map("credits.size")]
        ActiveFundingInPositions,
        /// <summary>
        /// ["<c>credits.size.sym</c>"] Active funding positions per symbol
        /// </summary>
        [Map("credits.size.sym")]
        ActiveFundingInPositionsPerTradingSymbol
    }
}
