using System;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.SocketObjects
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

        private readonly Action<BitfinexOrderBookBase[]> handler;

        public BookSubscriptionRequest(string symbol, string precision, string frequency, int length, Action<BitfinexOrderBookBase[]> handler)
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

    [SubscriptionChannel("book", typeof(BitfinexRawOrderBookEntry), false)]
    public class RawBookSubscriptionRequest : SubscriptionRequest
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
        [JsonProperty("prec")]
        public string Precision { get; set; }
        [JsonProperty("len")]
        public int Length { get; set; }

        private readonly Action<BitfinexOrderBookBase[]> handler;

        public RawBookSubscriptionRequest(string symbol, string precision, int length, Action<BitfinexOrderBookBase[]> handler)
        {
            Symbol = symbol;
            Precision = precision;
            Length = length;
            this.handler = handler;
        }

        protected override string GetSubscriptionSubKey()
        {
            return Symbol + Precision + "F0" + Length;
        }

        protected override void Handle(object obj)
        {
            handler((BitfinexRawOrderBookEntry[])obj);
        }
    }


    [SubscriptionChannel("book")]
    public class BookSubscriptionResponse : SubscriptionResponse
    {
        public string Symbol { get; set; }
        public string Pair { get; set; }
        [JsonProperty("prec")]
        public string Precision { get; set; }
        [JsonProperty("freq")]
        public string Frequency { get; set; }
        [JsonProperty("len")]
        public int Length { get; set; }

        protected override string[] GetSubscriptionSubKeys()
        {
            return new[] { Symbol + Precision + Frequency + Length, Pair + Precision + Frequency + Length };
        }
    }
}
