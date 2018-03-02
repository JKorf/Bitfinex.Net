using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.SocketObjects2
{
    [SubscriptionChannel("candles", typeof(BitfinexCandle), false)]
    public class CandleSubscriptionRequest : SubscriptionRequest
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        private Action<BitfinexCandle[]> handler;

        public CandleSubscriptionRequest(string symbol, string interval, Action<BitfinexCandle[]> handler)
        {
            Key = "trade:" + interval +":"+ symbol;

            this.handler = handler;
        }

        protected override string GetSubscriptionSubKey()
        {
            return Key;
        }

        protected override void Handle(object obj)
        {
            handler((BitfinexCandle[])obj);
        }
    }

    [SubscriptionChannel("candles")]
    public class CandleSubscriptionResponse : SubscriptionResponse
    {
        public string Symbol { get; set; }
        public string Key { get; set; }

        protected override string GetSubsciptionSubKey()
        {
            return Key;
        }
    }
}
