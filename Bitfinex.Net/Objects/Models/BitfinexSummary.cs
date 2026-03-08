using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Bitfinex fee summary
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexSummary>))]
    [SerializationModel]
    public record BitfinexSummary
    {
        /// <summary>
        /// Maker/taker fee info
        /// </summary>
        [ArrayProperty(4), JsonConversion]
        public BitfinexFees Fees { get; set; } = null!;
        /// <summary>
        /// 30 day volumes
        /// </summary>
        [ArrayProperty(5), JsonConversion]
        public BitfinexVolumes Volume30d { get; set; } = null!;
        /// <summary>
        /// 30 day funding earnings
        /// </summary>
        [ArrayProperty(6), JsonConversion]
        public BitfinexFundingEarnings FundingEarnings30d { get; set; } = null!;
        /// <summary>
        /// Leo info
        /// </summary>
        [ArrayProperty(9), JsonConversion]
        public BitfinexLeoInfo LeoInfo { get; set; } = null!;
    }

    /// <summary>
    /// Leo info
    /// </summary>
    [SerializationModel]
    public record BitfinexLeoInfo
    {
        /// <summary>
        /// ["<c>leo_lev</c>"] Leo discount level
        /// </summary>
        [JsonPropertyName("leo_lev")]
        public int LeoDiscountLevel { get; set; }
        /// <summary>
        /// ["<c>leo_amount_avg</c>"] Leo average quantity
        /// </summary>
        [JsonPropertyName("leo_amount_avg")]
        public decimal LeoAverageQuantity { get; set; }
    }

    /// <summary>
    /// Funding earnings
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexFundingEarnings>))]
    [SerializationModel]
    public record BitfinexFundingEarnings
    {
        /// <summary>
        /// Earnings per asset
        /// </summary>
        [ArrayProperty(1), JsonConversion]
        public Dictionary<string, decimal> Earnings { get; set; } = new Dictionary<string, decimal>();
        /// <summary>
        /// Earnings total in USD
        /// </summary>
        [ArrayProperty(2)]
        public decimal EarningsTotal { get; set; }
    }

    /// <summary>
    /// Volume info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexVolumes>))]
    [SerializationModel]
    public record BitfinexVolumes
    {
        /// <summary>
        /// Volumes
        /// </summary>
        [ArrayProperty(0), JsonConversion]
        public Bitfinex30DaySummaryVolumeEntry[] Volumes { get; set; } = [];
        /// <summary>
        /// Trading fees paid
        /// </summary>
        [ArrayProperty(1), JsonConversion]
        public Dictionary<string, decimal> TradingFees { get; set; } = new Dictionary<string, decimal>();
        /// <summary>
        /// Trading fees paid total in USD
        /// </summary>
        [ArrayProperty(2)]
        public decimal TradingFeesTotal { get; set; }
    }

    /// <summary>
    /// Fee info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexFees>))]
    [SerializationModel]
    public record BitfinexFees
    {
        /// <summary>
        /// Maker fees
        /// </summary>
        [ArrayProperty(0), JsonConversion]
        public BitfinexMakerFee MakerFees { get; set; } = null!;
        /// <summary>
        /// Taker fees
        /// </summary>
        [ArrayProperty(1), JsonConversion]
        public BitfinexTakerFee TakerFees { get; set; } = null!;
    }

    /// <summary>
    /// Maker fee info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexMakerFee>))]
    [SerializationModel]
    public record BitfinexMakerFee
    {
        /// <summary>
        /// Maker fee
        /// </summary>
        [ArrayProperty(0)]
        public decimal MakerFee { get; set; }
        /// <summary>
        /// Derivatives rebate
        /// </summary>
        [ArrayProperty(5)]
        public decimal DerivativeRebate { get; set; }
    }

    /// <summary>
    /// Taker fees info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexTakerFee>))]
    [SerializationModel]
    public record BitfinexTakerFee
    {
        /// <summary>
        /// Crypto trading taker fee
        /// </summary>
        [ArrayProperty(0)]
        public decimal TakerFeeCrypto { get; set; }
        /// <summary>
        /// Stable coin trading taker fee
        /// </summary>
        [ArrayProperty(1)]
        public decimal TakerFeeStable { get; set; }
        /// <summary>
        /// Fiat trading taker fee
        /// </summary>
        [ArrayProperty(2)]
        public decimal TakerFeeFiat { get; set; }
        /// <summary>
        /// Derivative trading taker fee
        /// </summary>
        [ArrayProperty(5)]
        public decimal DerivativeTakerFee { get; set; }
    }

    /// <summary>
    /// Summary entry
    /// </summary>
    [SerializationModel]
    public record Bitfinex30DaySummaryVolumeEntry
    {
        /// <summary>
        /// ["<c>curr</c>"] The asset
        /// </summary>
        [JsonPropertyName("curr")]
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// ["<c>vol</c>"] The volume
        /// </summary>
        [JsonPropertyName("vol")]
        public decimal Volume { get; set; }
        /// <summary>
        /// ["<c>vol_safe</c>"] The safe volume
        /// </summary>
        [JsonPropertyName("vol_safe")]
        public decimal VolumeSafe { get; set; }
        /// <summary>
        /// ["<c>vol_maker</c>"] The maker volume
        /// </summary>
        [JsonPropertyName("vol_maker")]
        public decimal VolumeMaker { get; set; }
        /// <summary>
        /// ["<c>vol_BFX</c>"] The volume on Bitfinex
        /// </summary>
        [JsonPropertyName("vol_BFX")]
        public decimal VolumeBFX { get; set; }
        /// <summary>
        /// ["<c>vol_BFX_safe</c>"] The safe volume on Bitfinex
        /// </summary>
        [JsonPropertyName("vol_BFX_safe")]
        public decimal VolumeBFXSafe { get; set; } 
        /// <summary>
        /// ["<c>vol_BFX_maker</c>"] The maker volume on Bitfinex
        /// </summary>
        [JsonPropertyName("vol_BFX_maker")]
        public decimal VolumeBFXMaker { get; set; } 
        /// <summary>
        /// ["<c>vol_ETHFX</c>"] The volume on EthFinex
        /// </summary>
        [JsonPropertyName("vol_ETHFX")]
        public decimal VolumeETHFX { get; set; }
        /// <summary>
        /// ["<c>vol_ETHFX_maker</c>"] The maker volume on EthFinex
        /// </summary>
        [JsonPropertyName("vol_ETHFX_maker")]
        public decimal VolumeETHFXMaker { get; set; }
    }
}
