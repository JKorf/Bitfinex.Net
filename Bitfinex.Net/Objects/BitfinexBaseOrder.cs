using System;
using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    public class BitfinexBaseOrder
    {
        public long Id { get; set; }
        public long CId { get; set; }
        [JsonProperty("gid")]
        public long? GroupId { get; set; }
        public string Symbol { get; set; }
        public string Exchange { get; set; }
        public double Price { get; set; }
        [JsonProperty("avg_execution_price")]
        public double AverageExecutionPrice { get; set; }
        [JsonConverter(typeof(OrderSideConverter))]
        public OrderSide Side { get; set; }
        [JsonConverter(typeof(OrderType2Converter))]
        public OrderType2 Type { get; set; }
        [JsonConverter(typeof(TimestampSecondsConverter))]
        public DateTime Timestamp { get; set; }
        [JsonProperty("is_live")]
        public bool Live { get; set; }
        [JsonProperty("is_cancelled")]
        public bool Canceled { get; set; }
        [JsonProperty("is_hidden")]
        public bool Hidden { get; set; }
        [JsonProperty("was_forced")]
        public bool Forced { get; set; }
        [JsonProperty("original_amount")]
        public double OriginalAmount { get; set; }
        [JsonProperty("remaining_amount")]
        public double RemainingAmount { get; set; }
        [JsonProperty("executed_amount")]
        public double ExecutedAmount { get; set; }
        [JsonProperty("src")]
        public string Source { get; set; }
    }
}
