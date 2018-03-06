using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.RestV1Objects
{
    public class BitfinexAccountInfo
    {
        [JsonProperty("maker_fees")]
        public decimal MakerFee { get; set; }
        [JsonProperty("taker_fees")]
        public decimal TakerFee { get; set; }
        public BitfinexFee[] Fees { get; set; }
    }

    public class BitfinexFee
    {
        public string Pairs { get; set; }
        [JsonProperty("maker_fees")]
        public decimal MakerFee { get; set; }
        [JsonProperty("taker_fees")]
        public decimal TakerFee { get; set; }
    }
}
