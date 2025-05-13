using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Increase position info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexIncreasePositionInfo>))]
    [SerializationModel]
    public record BitfinexIncreasePositionInfo
    {
        /// <summary>
        /// Position quantity info
        /// </summary>
        [ArrayProperty(0)]
        public BitfinexIncreasePositionQuantity PositionQuantityInfo { get; set; } = null!;
        /// <summary>
        /// Balance info
        /// </summary>
        [ArrayProperty(1)]
        public BitfinexIncreasePositionBalances Balances { get; set; } = null!;
        /// <summary>
        /// Funding info
        /// </summary>
        [ArrayProperty(4)]
        public BitfinexIncreasePositionFundingInfo FundingInfo { get; set; } = null!;
        /// <summary>
        /// Funding asset info
        /// </summary>
        [ArrayProperty(5)]
        public BitfinexIncreasePositionFundingAssetInfo FundingAssetInfo { get; set; } = null!;
    }

    /// <summary>
    /// Position quantity info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexIncreasePositionQuantity>))]
    [SerializationModel]
    public record BitfinexIncreasePositionQuantity
    {
        /// <summary>
        /// Max position
        /// </summary>
        [ArrayProperty(0)]
        public decimal MaxPosition { get; set; }
        /// <summary>
        /// Current position
        /// </summary>
        [ArrayProperty(1)]
        public decimal CurrentPosition { get; set; }
    }

    /// <summary>
    /// Position balances
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexIncreasePositionBalances>))]
    [SerializationModel]
    public record BitfinexIncreasePositionBalances
    {
        /// <summary>
        /// Base asset balance
        /// </summary>
        [ArrayProperty(0)]
        public decimal BaseAssetBalance { get; set; }
        /// <summary>
        /// Tradable balances
        /// </summary>
        [ArrayProperty(1)]
        public BitfinexIncreasePositionBalance TradableBalances { get; set; } = null!;
        /// <summary>
        /// Funding available
        /// </summary>
        [ArrayProperty(2)]
        public decimal FundingAvailable { get; set; }
    }

    /// <summary>
    /// Balance info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexIncreasePositionBalance>))]
    [SerializationModel]
    public record BitfinexIncreasePositionBalance
    {
        /// <summary>
        /// Current tradable quote balance
        /// </summary>
        [ArrayProperty(0)]
        public decimal TradableBalanceQuoteCurrent { get; set; }
        /// <summary>
        /// Total tradable quote balance
        /// </summary>
        [ArrayProperty(1)]
        public decimal TradableBalanceQuoteTotal { get; set; }
        /// <summary>
        /// Current tradable base balance
        /// </summary>
        [ArrayProperty(2)]
        public decimal TradableBalanceBaseCurrent { get; set; }
        /// <summary>
        /// Total tradable base balance
        /// </summary>
        [ArrayProperty(3)]
        public decimal TradableBalanceBaseTotal { get; set; }
    }

    /// <summary>
    /// Funding info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexIncreasePositionFundingInfo>))]
    [SerializationModel]
    public record BitfinexIncreasePositionFundingInfo
    {
        /// <summary>
        /// Funding value
        /// </summary>
        [ArrayProperty(0)]
        public decimal FundingValue { get; set; }
        /// <summary>
        /// Funding required
        /// </summary>
        [ArrayProperty(1)]
        public decimal FundingRequired { get; set; }
    }

    /// <summary>
    /// Funding asset info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexIncreasePositionFundingAssetInfo>))]
    [SerializationModel]
    public record BitfinexIncreasePositionFundingAssetInfo
    {
        /// <summary>
        /// Funding value asset
        /// </summary>
        [ArrayProperty(0)]
        public string FundingValueAsset { get; set; } = string.Empty;
        /// <summary>
        /// Funding required asset
        /// </summary>
        [ArrayProperty(1)]
        public string FundingRequiredAsset { get; set; } = string.Empty;
    }
}
