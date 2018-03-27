using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexMarginBase
    {
        [ArrayProperty(0)]
        public string Type { get; set; }

        [ArrayProperty(1), JsonConverter(typeof(ArrayConverter))]
        public BitfinexMarginBaseInfo Data { get; set; }
    }
    
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexMarginBaseInfo
    {
        [ArrayProperty(0)]
        public decimal UserProfitLoss { get; set; }

        [ArrayProperty(1)]
        public decimal UserSwapsAmount { get; set; }

        [ArrayProperty(2)]
        public decimal MarginBalance { get; set; }

        [ArrayProperty(3)]
        public decimal MarginNet { get; set; }
    }
}
