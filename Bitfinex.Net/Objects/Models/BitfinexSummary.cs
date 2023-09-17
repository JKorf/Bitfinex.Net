using Bitfinex.Net.Objects.Models.V1;
using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Bitfinex fee summary
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexSummary
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
    public class BitfinexLeoInfo
    {
        /// <summary>
        /// Leo discount level
        /// </summary>
        [JsonProperty("leo_lev")]
        public int LeoDiscountLevel { get; set; }
        /// <summary>
        /// Leo average quantity
        /// </summary>
        [JsonProperty("leo_amount_avg")]
        public decimal LeoAverageQuantity { get; set; }
    }

    /// <summary>
    /// Funding earnings
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexFundingEarnings
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
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexVolumes
    {
        /// <summary>
        /// Volumes
        /// </summary>
        [ArrayProperty(0), JsonConversion]
        public List<Bitfinex30DaySummaryVolumeEntry> Volumes { get; set; } = new List<Bitfinex30DaySummaryVolumeEntry>();
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
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexFees
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
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexMakerFee
    {
        /// <summary>
        /// Maker fee
        /// </summary>
        [ArrayProperty(0)]
        public decimal MakerFee { get; set; }
        /// <summary>
        /// Derivatie rebate
        /// </summary>
        [ArrayProperty(5)]
        public decimal DerivativeRebate { get; set; }
    }

    /// <summary>
    /// Taker fees info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexTakerFee
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
    public class Bitfinex30DaySummaryVolumeEntry
    {
        /// <summary>
        /// The asset
        /// </summary>
        [JsonProperty("curr")]
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// The volume
        /// </summary>
        [JsonProperty("vol")]
        public string Volume { get; set; } = string.Empty;
        /// <summary>
        /// The maker volume
        /// </summary>
        [JsonProperty("vol_maker")]
        public string VolumeMaker { get; set; } = string.Empty;
        /// <summary>
        /// The volume on Bitfinex
        /// </summary>
        [JsonProperty("vol_BFX")]
        public string VolumeBFX { get; set; } = string.Empty;
        /// <summary>
        /// The maker volume on Bitfinex
        /// </summary>
        [JsonProperty("vol_BFX_maker")]
        public string VolumeBFXMaker { get; set; } = string.Empty;
        /// <summary>
        /// The volume on EthFinex
        /// </summary>
        [JsonProperty("vol_ETHFX")]
        public string VolumeETHFX { get; set; } = string.Empty;
        /// <summary>
        /// The maker volume on EthFinex
        /// </summary>
        [JsonProperty("vol_ETHFX_maker")]
        public string VolumeETHFXMaker { get; set; } = string.Empty;
    }
}
