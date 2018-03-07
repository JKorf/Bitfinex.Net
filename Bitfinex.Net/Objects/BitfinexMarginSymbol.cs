using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexMarginSymbol
    {

        [BitfinexProperty(0)]
        public string Symbol { get; set; }

        [BitfinexProperty(1), JsonConverter(typeof(BitfinexResultConverter))]
        public BitfinexMarginSymbolInfo Data { get; set; }
    }

    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexMarginSymbolInfo
    {
        [BitfinexProperty(0)]
        public decimal TradeableBalance { get; set; }

        [BitfinexProperty(1)]
        public decimal GrossBalance { get; set; }

        [BitfinexProperty(2)]
        public decimal Buy { get; set; }

        [BitfinexProperty(3)]
        public decimal Sell { get; set; }
    }
}
