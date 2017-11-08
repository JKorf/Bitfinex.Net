using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexMarginBase
    {
        [BitfinexProperty(0)]
        public string Type { get; set; }

        [BitfinexProperty(1), JsonConverter(typeof(BitfinexResultConverter))]
        public BitfinexMarginBaseInfo Data { get; set; }
    }
    
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexMarginBaseInfo
    {
        [BitfinexProperty(0)]
        public double UserProfitLoss { get; set; }

        [BitfinexProperty(1)]
        public double UserSwapsAmount { get; set; }

        [BitfinexProperty(2)]
        public double MarginBalance { get; set; }

        [BitfinexProperty(3)]
        public double MarginNet { get; set; }
    }
}
