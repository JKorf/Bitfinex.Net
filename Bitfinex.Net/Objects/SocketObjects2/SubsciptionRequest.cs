using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.SocketObjects2
{
    public abstract class SubsciptionRequest
    {
        [JsonProperty("event")]
        public string Event { get; set; }
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonIgnore]
        public ManualResetEvent ConfirmedEvent { get; }

        protected SubsciptionRequest(string channel)
        {
            Event = "subscribe";
            Channel = channel;
            ConfirmedEvent = new ManualResetEvent(false);
        }

        public string GetSubscriptionKey()
        {
            return Channel + GetSubsciptionSubKey();
        }

        protected abstract string GetSubsciptionSubKey();
    }

    public abstract class SubscriptionResponse
    {
        public string Event { get; set; }
        public string Channel { get; set; }
        [JsonProperty("chanId")]
        public int ChannelId { get; set; }

        public string GetSubscriptionKey()
        {
            return Channel + GetSubsciptionSubKey();
        }

        protected abstract string GetSubsciptionSubKey();
    }
}
