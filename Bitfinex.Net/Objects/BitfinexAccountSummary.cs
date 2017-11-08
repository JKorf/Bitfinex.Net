using System;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    public class BitfinexAccountSummary
    {
        [JsonProperty("time")]
        public DateTime Timestamp { get; set; }
        [JsonProperty("trade_vol_30d")]
        public BitfinexTradeVolume[] TradeVolume30Days { get; set; }
        [JsonProperty("funding_profit_30d")]
        public BitfinexFundingProfit[] FundingProfit30Days { get; set; }
        [JsonProperty("maker_fee")]
        public double MakerFee { get; set; }
        [JsonProperty("taker_fee")]
        public double TakerFee { get; set; }
    }

    public class BitfinexTradeVolume
    {
        [JsonProperty("curr")]
        public string Currency { get; set; }
        [JsonProperty("vol")]
        public double Volume { get; set; }
    }

    public class BitfinexFundingProfit
    {
        [JsonProperty("curr")]
        public string Currency { get; set; }
        [JsonProperty("amount")]
        public double Amount { get; set; }
    }
}
