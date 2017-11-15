using System;
using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    public class BitfinexActiveMarginFund
    {
        public long Id { get; set; }
        [JsonProperty("position_id")]
        public long PositionId { get; set; }
        public double Rate { get; set; }
        public int Period { get; set; }
        public double Amount { get; set; }
        [JsonConverter(typeof(TimestampSecondsConverter))]
        public DateTime Timestamp { get; set; }
        [JsonProperty("auto_close")]
        public bool AutoClose { get; set; }
    }
}
