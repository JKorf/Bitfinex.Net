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
        public decimal UserProfitLoss { get; set; }

        [BitfinexProperty(1)]
        public decimal UserSwapsAmount { get; set; }

        [BitfinexProperty(2)]
        public decimal MarginBalance { get; set; }

        [BitfinexProperty(3)]
        public decimal MarginNet { get; set; }
    }
}
