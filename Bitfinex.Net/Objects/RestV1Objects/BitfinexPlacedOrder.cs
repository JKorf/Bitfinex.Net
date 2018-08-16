using System;
using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.RestV1Objects
{
    public class BitfinexPlacedOrder
    {
        public long Id { get; set; }
        public string Symbol { get; set; }
        public string Exchange { get; set; }
        public decimal Price { get; set; }
        [JsonProperty("avg_execution_price")]
        public decimal AverageExecutionPrice { get; set; }
        [JsonConverter(typeof(OrderSideConverter))]
        public OrderSide Side { get; set; }
        [JsonConverter(typeof(OrderTypeV1Converter))]
        public OrderTypeV1 Type { get; set; }
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
        public decimal OriginalAmount { get; set; }
        [JsonProperty("remaining_amount")]
        public decimal RemainingAmount { get; set; }
        [JsonProperty("executed_amount")]
        public decimal ExecutedAmount { get; set; }
        [JsonProperty("gid")]
        public long? GroupId { get; set; }
        [JsonProperty("cid")]
        public long ClientOrderId { get; set; }
        [JsonProperty("cid_date")]
        public string ClientOrderDate { get; set; }

        [JsonProperty("src")]
        public string Source { get; set; }
        [JsonProperty("oco_order")]
        public string OcoOrder { get; set; }
    }
}
