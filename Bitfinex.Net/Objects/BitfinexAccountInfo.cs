using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    public class BitfinexAccountInfo
    {
        [JsonProperty("maker_fees")]
        public double MakerFees { get; set; }
        [JsonProperty("taker_fees")]
        public double TakerFees { get; set; }
        [JsonProperty("fees")]
        public List<BitfinexFee> Fees { get; set; }
    }

    public class BitfinexFee
    {
        [JsonProperty("pairs")]
        public string Pairs { get; set; }
        [JsonProperty("maker_fees")]
        public double MakerFees { get; set; }
        [JsonProperty("taker_fees")]
        public double TakerFees { get; set; }
    }
}
