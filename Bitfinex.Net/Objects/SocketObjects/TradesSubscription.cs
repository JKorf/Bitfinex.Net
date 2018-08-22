using System;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.SocketObjects
{
    [SubscriptionChannel("trades", typeof(BitfinexTradeSimple), true)]
    public class TradesSubscriptionRequest : SubscriptionRequest
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        private readonly Action<BitfinexSocketEvent<BitfinexTradeSimple[]>> handler;

        public TradesSubscriptionRequest(string symbol, Action<BitfinexSocketEvent<BitfinexTradeSimple[]>> handler)
        {
            Symbol = symbol;
            this.handler = handler;
        }

        protected override string GetSubscriptionSubKey()
        {
            return Symbol;
        }

        protected override void Handle(params object[] obj)
        {
            var evntType = (BitfinexEventType?) obj[0];
            handler(new BitfinexSocketEvent<BitfinexTradeSimple[]>(evntType ?? BitfinexEventType.TradesSnapshot, (BitfinexTradeSimple[])obj[1]));
        }
    }

    [SubscriptionChannel("trades")]
    public class TradesSubscriptionResponse : SubscriptionResponse
    {
        public string Symbol { get; set; }
        public string Pair { get; set; }

        protected override string[] GetSubscriptionSubKeys()
        {
            return new[] { Symbol, Pair };
        }
    }
}
