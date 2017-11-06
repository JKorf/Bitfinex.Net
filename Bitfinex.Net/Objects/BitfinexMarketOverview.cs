using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexMarketOverview
    {
        [BitfinexProperty(0)]
        public string Symbol { get; set; }
        [BitfinexProperty(1)]
        public double Bid { get; set; }
        [BitfinexProperty(2)]
        public double BidSize { get; set; }
        [BitfinexProperty(3)]
        public double Ask { get; set; }
        [BitfinexProperty(4)]
        public double AskSize { get; set; }
        [BitfinexProperty(5)]
        public double DailyChange { get; set; }
        [BitfinexProperty(6)]
        public double DailtyChangePercentage { get; set; }
        [BitfinexProperty(7)]
        public double LastPrice { get; set; }
        [BitfinexProperty(8)]
        public double Volume { get; set; }
        [BitfinexProperty(9)]
        public double High { get; set; }
        [BitfinexProperty(10)]
        public double Low { get; set; }

    }
}
