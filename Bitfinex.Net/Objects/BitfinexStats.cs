using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexStats
    {
        [BitfinexProperty(0), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }

        [BitfinexProperty(1)]
        public double Value { get; set; }
    }
}
