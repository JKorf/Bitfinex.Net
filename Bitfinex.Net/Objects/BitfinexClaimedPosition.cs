using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    public class BitfinexClaimedPosition
    {
        public long Id { get; set; }
        public string Symbol { get; set; }
        public string Status { get; set; }
        public double Base { get; set; }
        public double Amount { get; set; }
        [JsonConverter(typeof(TimestampSecondsConverter))]
        public DateTime Timestamp { get; set; }
        [JsonProperty("pl")]
        public double ProfitLoss { get; set; }
        public double Swap { get; set; }
    }
}
