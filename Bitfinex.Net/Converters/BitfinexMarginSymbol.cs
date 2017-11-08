using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexMarginSymbol
    {
        [BitfinexProperty(0)]
        public string Type { get; set; }

        [BitfinexProperty(1)]
        public string Symbol { get; set; }

        [BitfinexProperty(2), JsonConverter(typeof(BitfinexResultConverter))]
        public BitfinexMarginSymbolInfo Data { get; set; }
    }

    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexMarginSymbolInfo
    {
        [BitfinexProperty(0)]
        public double TradeableBalance { get; set; }

        [BitfinexProperty(1)]
        public double GrossBalance { get; set; }

        [BitfinexProperty(2)]
        public double Buy { get; set; }

        [BitfinexProperty(3)]
        public double Sell { get; set; }
    }
}
