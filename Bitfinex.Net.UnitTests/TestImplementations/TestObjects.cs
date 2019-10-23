using Newtonsoft.Json;

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
        [JsonProperty("symbol")]
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
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
        [JsonProperty("key")]
        public string Key { get; set; }
    }

    public class TickerSubscriptionResponse : SubscriptionResponse
    {
        public string Pair { get; set; }
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
    }

    public class TradesSubscriptionResponse : SubscriptionResponse
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
        public string Pair { get; set; }
    }
}
