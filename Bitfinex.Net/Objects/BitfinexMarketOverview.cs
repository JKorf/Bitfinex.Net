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
        public decimal Bid { get; set; }
        [BitfinexProperty(2)]
        public decimal BidSize { get; set; }
        [BitfinexProperty(3)]
        public decimal Ask { get; set; }
        [BitfinexProperty(4)]
        public decimal AskSize { get; set; }
        [BitfinexProperty(5)]
        public decimal DailyChange { get; set; }
        [BitfinexProperty(6)]
        public decimal DailtyChangePercentage { get; set; }
        [BitfinexProperty(7)]
        public decimal LastPrice { get; set; }
        [BitfinexProperty(8)]
        public decimal Volume { get; set; }
        [BitfinexProperty(9)]
        public decimal High { get; set; }
        [BitfinexProperty(10)]
        public decimal Low { get; set; }

    }
}
