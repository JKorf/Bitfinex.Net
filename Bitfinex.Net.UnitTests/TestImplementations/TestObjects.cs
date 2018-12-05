using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binance.Net.UnitTests.TestImplementations
{
    public abstract class SubscriptionResponse
    {
        [JsonProperty("event")]
        public string Event { get; set; }
        [JsonProperty("channel")]
        public string Channel { get; set; }
        [JsonProperty("chanId")]
        public int ChannelId { get; set; }
    }

    public class BookSubscriptionResponse : SubscriptionResponse
    {
        public string Symbol { get; set; }
        public string Pair { get; set; }
        [JsonProperty("prec")]
        public string Precision { get; set; }
        [JsonProperty("freq")]
        public string Frequency { get; set; }
        [JsonProperty("len")]
        public int Length { get; set; }
    }

    public class CandleSubscriptionResponse : SubscriptionResponse
    {
        public string Symbol { get; set; }
        public string Key { get; set; }
    }

    public class TickerSubscriptionResponse : SubscriptionResponse
    {
        public string Pair { get; set; }
        public string Symbol { get; set; }
    }

    public class TradesSubscriptionResponse : SubscriptionResponse
    {
        public string Symbol { get; set; }
        public string Pair { get; set; }
    }
}
