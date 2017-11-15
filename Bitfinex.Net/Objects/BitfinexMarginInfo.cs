using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    public class BitfinexMarginInfo
    {
        [JsonProperty("margin_balance")]
        public double MarginBalance { get; set; }
        [JsonProperty("unrealized_pl")]
        public double UnrealizedProfitLoss { get; set; }
        [JsonProperty("unrealized_swap")]
        public double UnrealizedSwap { get; set; }
        [JsonProperty("net_value")]
        public double NetValue { get; set; }
        [JsonProperty("required_margin")]
        public double RequiredMargin { get; set; }
        [JsonProperty("leverage")]
        public double Leverage { get; set; }
        [JsonProperty("margin_limits")]
        public BitfinexMarginInfoLimitDetails[] MarginLimits { get; set; }
    }

    public class BitfinexMarginInfoLimitDetails
    {
        [JsonProperty("on_pair")]
        public string OnPair { get; set; }
        [JsonProperty("initial_margin")]
        public double InitialMargin { get; set; }
        [JsonProperty("margin_requirement")]
        public double MarginRequirement { get; set; }
        [JsonProperty("tradable_balance")]
        public double TradableBalance { get; set; }
    }
}
