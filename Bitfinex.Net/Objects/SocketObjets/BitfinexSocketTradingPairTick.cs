using Bitfinex.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitfinex.Net.Objects.SocketObjets
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexSocketTradingPairTick
    {
        [BitfinexProperty(0)]
        public double Bid { get; set; }
        [BitfinexProperty(1)]
        public double BidSize { get; set; }
        [BitfinexProperty(2)]
        public double Ask { get; set; }
        [BitfinexProperty(3)]
        public double AskSize { get; set; }
        [BitfinexProperty(4)]
        public double DailyChange { get; set; }
        [BitfinexProperty(5)]
        public double DailyChangePercentage { get; set; }
        [BitfinexProperty(6)]
        public double LastPrice { get; set; }
        [BitfinexProperty(7)]
        public double Volume { get; set; }
        [BitfinexProperty(8)]
        public double High { get; set; }
        [BitfinexProperty(9)]
        public double Low { get; set; }
    }
}
