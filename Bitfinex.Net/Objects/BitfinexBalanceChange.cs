using System;
using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    public class BitfinexBalanceChange
    {
        public string Currency { get; set; }
        public double Amount { get; set; }
        [JsonProperty("balance")]
        public string BalanceLeft { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(TimestampSecondsConverter))]
        public DateTime Timestamp { get; set; }
    }
}
