using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.RestV1Objects
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
        public IEnumerable<Bitfinex30DaySummaryVolumeEntry> TradeVolume { get; set; } = new List<Bitfinex30DaySummaryVolumeEntry>();
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
    /// Summary entry
    /// </summary>
    public class Bitfinex30DaySummaryVolumeEntry
    {
        /// <summary>
        /// The currency
        /// </summary>
        [JsonProperty("curr")]
        public string Currency { get; set; } = "";

        /// <summary>
        /// The volume
        /// </summary>
        [JsonProperty("vol")]
        public string Volume { get; set; } = "";
        /// <summary>
        /// The maker volume
        /// </summary>
        [JsonProperty("vol_maker")]
        public string VolumeMaker { get; set; } = "";
        /// <summary>
        /// The volume on Bitfinex
        /// </summary>
        [JsonProperty("vol_BFX")]
        public string VolumeBFX { get; set; } = "";
        /// <summary>
        /// The maker volume on Bitfinex
        /// </summary>
        [JsonProperty("vol_BFX_maker")]
        public string VolumeBFXMaker { get; set; } = "";
        /// <summary>
        /// The volume on EthFinex
        /// </summary>
        [JsonProperty("vol_ETHFX")]
        public string VolumeETHFX { get; set; } = "";
        /// <summary>
        /// The maker volume on EthFinex
        /// </summary>
        [JsonProperty("vol_ETHFX_maker")]
        public string VolumeETHFXMaker { get; set; } = "";
    }
}
