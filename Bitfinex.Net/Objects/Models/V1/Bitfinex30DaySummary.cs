using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models.V1
{
    /// <summary>
    /// 30 day summary info
    /// </summary>
    public class Bitfinex30DaySummary
    {
        /// <summary>
        /// The timestamp of the data
        /// </summary>
        [JsonProperty("time")]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Trade volume data
        /// </summary>
        [JsonProperty("trade_vol_30d")]
        public IEnumerable<Bitfinex30DaySummaryVolumeEntry> TradeVolume { get; set; } = Array.Empty<Bitfinex30DaySummaryVolumeEntry>();
        /// <summary>
        /// Trade volume data
        /// </summary>
        [JsonProperty("funding_profit_30d")]
        public IEnumerable<Bitfinex30DayFundingProfitEntry> FundingProfit { get; set; } = Array.Empty<Bitfinex30DayFundingProfitEntry>();
        /// <summary>
        /// Current maker fee
        /// </summary>
        [JsonProperty("maker_fee")]
        public decimal MakerFee { get; set; }
        /// <summary>
        /// Current taker fee
        /// </summary>
        [JsonProperty("taker_fee")]
        public decimal TakerFee { get; set; }
    }

    /// <summary>
    /// Funding profit entry
    /// </summary>
    public class Bitfinex30DayFundingProfitEntry 
    {
        /// <summary>
        /// The asset
        /// </summary>
        [JsonProperty("curr")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// The asset
        /// </summary>
        [JsonProperty("amount")]
        public string Quantity { get; set; } = string.Empty;
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
