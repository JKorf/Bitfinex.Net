using System;
using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    public class BitfinexOffer
    {
        public long Id { get; set; }
        public string Currency { get; set; }
        public double Rate { get; set; }
        public int Period { get; set; }
        [JsonConverter(typeof(FundingTypeConverter)), JsonProperty("direction")]
        public FundingType Type { get; set; }
        [JsonConverter(typeof(TimestampSecondsConverter))]
        public DateTime Timestamp { get; set; }
        [JsonProperty("is_live")]
        public bool Live { get; set; }
        [JsonProperty("is_cancelled")]
        public bool Canceled { get; set; }
        [JsonProperty("original_amount")]
        public double OriginalAmount { get; set; }
        [JsonProperty("remaining_amount")]
        public double RemainingAmount { get; set; }
        [JsonProperty("offer_id")]
        public long OfferId { get; set; }
    }
}
