using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.SocketObjects2
{
    public class TickerSubscriptionRequest : SubsciptionRequest
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        public TickerSubscriptionRequest(string symbol) : base("ticker")
        {
            Symbol = symbol;
        }

        protected override string GetSubsciptionSubKey()
        {
            return Symbol;
        }
    }

    public class TickerSubscriptionResponse : SubscriptionResponse
    {
        public string Symbol { get; set; }

        protected override string GetSubsciptionSubKey()
        {
            return Symbol;
        }
    }
}
