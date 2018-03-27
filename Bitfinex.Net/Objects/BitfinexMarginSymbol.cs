using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexMarginSymbol
    {

        [ArrayProperty(0)]
        public string Symbol { get; set; }

        [ArrayProperty(1), JsonConverter(typeof(ArrayConverter))]
        public BitfinexMarginSymbolInfo Data { get; set; }
    }

    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexMarginSymbolInfo
    {
        [ArrayProperty(0)]
        public decimal TradeableBalance { get; set; }

        [ArrayProperty(1)]
        public decimal GrossBalance { get; set; }

        [ArrayProperty(2)]
        public decimal Buy { get; set; }

        [ArrayProperty(3)]
        public decimal Sell { get; set; }
    }
}
