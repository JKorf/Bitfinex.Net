using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexMarketOverview
    {
        [ArrayProperty(0)]
        public decimal Bid { get; set; }
        [ArrayProperty(1)]
        public decimal BidSize { get; set; }
        [ArrayProperty(2)]
        public decimal Ask { get; set; }
        [ArrayProperty(3)]
        public decimal AskSize { get; set; }
        [ArrayProperty(4)]
        public decimal DailyChange { get; set; }
        [ArrayProperty(5)]
        public decimal DailtyChangePercentage { get; set; }
        [ArrayProperty(6)]
        public decimal LastPrice { get; set; }
        [ArrayProperty(7)]
        public decimal Volume { get; set; }
        [ArrayProperty(8)]
        public decimal High { get; set; }
        [ArrayProperty(9)]
        public decimal Low { get; set; }

    }
}
