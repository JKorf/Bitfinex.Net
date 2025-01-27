using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Stat types
    /// </summary>
    public enum StatKey
    {
        /// <summary>
        /// Total number of open positions
        /// </summary>
        [Map("pos.size")]
        TotalOpenPosition,
        /// <summary>
        /// Total active funding
        /// </summary>
        [Map("funding.size")]
        TotalActiveFunding,
        /// <summary>
        /// Active funding in positions
        /// </summary>
        [Map("credits.size")]
        ActiveFundingInPositions,
        /// <summary>
        /// Active funding positions per symbol
        /// </summary>
        [Map("credits.size.sym")]
        ActiveFundingInPositionsPerTradingSymbol
    }
}
