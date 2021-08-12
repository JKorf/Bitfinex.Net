using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.RestV1Objects
{
    /// <summary>
    /// Account info
    /// </summary>
    public class BitfinexAccountInfo
    {
        /// <summary>
        /// Maker trade fee
        /// </summary>
        [JsonProperty("maker_fees")]
        public decimal MakerFee { get; set; }
        /// <summary>
        /// Taker trade fee
        /// </summary>
        [JsonProperty("taker_fees")]
        public decimal TakerFee { get; set; }
        /// <summary>
        /// Symbol specific fees
        /// </summary>
        public IEnumerable<BitfinexFee> Fees { get; set; } = Array.Empty<BitfinexFee>();
    }

    /// <summary>
    /// Fee info
    /// </summary>
    public class BitfinexFee
    {
        /// <summary>
        /// The fee pair
        /// </summary>
        public string Pairs { get; set; } = string.Empty;
        /// <summary>
        /// Maker trade fee
        /// </summary>
        [JsonProperty("maker_fees")]
        public decimal MakerFee { get; set; }
        /// <summary>
        /// Taker trade fee
        /// </summary>
        [JsonProperty("taker_fees")]
        public decimal TakerFee { get; set; }
    }
}
