using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitfinex.Net.Objects.SocketObjets;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.SocketObjects2
{
    [SubscriptionChannel("book", typeof(BitfinexOrderBookEntry), false)]
    public class BookSubscriptionRequest : SubscriptionRequest
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
        [JsonProperty("prec")]
        public string Precision { get; set; }
        [JsonProperty("freq")]
        public string Frequency { get; set; }
        [JsonProperty("len")]
        public int Length { get; set; }

        private Action<BitfinexOrderBookEntry[]> handler;

        public BookSubscriptionRequest(string symbol, string precision, string frequency, int length, Action<BitfinexOrderBookEntry[]> handler)
        {
            Symbol = symbol;
            Precision = precision;
            Frequency = frequency;
            Length = length;
            this.handler = handler;
        }

        protected override string GetSubscriptionSubKey()
        {
            return Symbol+Precision+Frequency+Length;
        }

        protected override void Handle(object obj)
        {
            handler((BitfinexOrderBookEntry[])obj);
        }
    }

    [SubscriptionChannel("book")]
    public class BookSubscriptionResponse : SubscriptionResponse
    {
        public string Symbol { get; set; }
        [JsonProperty("prec")]
        public string Precision { get; set; }
        [JsonProperty("freq")]
        public string Frequency { get; set; }
        [JsonProperty("len")]
        public int Length { get; set; }

        protected override string GetSubsciptionSubKey()
        {
            return Symbol + Precision + Frequency + Length;
        }
    }
}
