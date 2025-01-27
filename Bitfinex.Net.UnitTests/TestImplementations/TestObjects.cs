using System.Text.Json.Serialization;

namespace Binance.Net.UnitTests.TestImplementations
{
    public abstract class SubscriptionResponse
    {
        [JsonPropertyName("event")]
        public string Event { get; set; }
        [JsonPropertyName("channel")]
        public string Channel { get; set; }
        [JsonPropertyName("chanId")]
        public int ChannelId { get; set; }
    }

    public class BookSubscriptionResponse : SubscriptionResponse
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }
        public string Pair { get; set; }
        [JsonPropertyName("prec")]
        public string Precision { get; set; }
        [JsonPropertyName("freq")]
        public string Frequency { get; set; }
        [JsonPropertyName("len")]
        public int Length { get; set; }
    }

    public class CandleSubscriptionResponse : SubscriptionResponse
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }
        [JsonPropertyName("key")]
        public string Key { get; set; }
    }

    public class TickerSubscriptionResponse : SubscriptionResponse
    {
        public string Pair { get; set; }
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }
    }

    public class TradesSubscriptionResponse : SubscriptionResponse
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }
        public string Pair { get; set; }
    }
}
