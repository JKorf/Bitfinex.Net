using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
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
