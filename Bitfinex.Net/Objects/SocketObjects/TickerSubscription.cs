using System;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.SocketObjects
{
    [SubscriptionChannel("ticker", typeof(BitfinexMarketOverview), false)]
    public class TickerSubscriptionRequest : SubscriptionRequest
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        private readonly Action<BitfinexMarketOverview[]> handler;

        public TickerSubscriptionRequest(string symbol, Action<BitfinexMarketOverview[]> handler)
        {
            Symbol = symbol;
            this.handler = handler;
        }

        protected override string GetSubscriptionSubKey()
        {
            return Symbol;
        }

        protected override void Handle(object obj)
        {
            handler((BitfinexMarketOverview[]) obj);
        }
    }

    [SubscriptionChannel("ticker")]
    public class TickerSubscriptionResponse : SubscriptionResponse
    {
        public string Pair { get; set; }
        public string Symbol { get; set; }

        protected override string[] GetSubscriptionSubKeys()
        {
            return new[] { Pair, Symbol };
        }
    }
}
