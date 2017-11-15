using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    public class BitfinexTakenFund
    {
        [JsonProperty("position_pair")]
        public string PositionPair { get; set; }
        [JsonProperty("total_swaps")]
        public double TotalSwaps { get; set; }
    }
}
