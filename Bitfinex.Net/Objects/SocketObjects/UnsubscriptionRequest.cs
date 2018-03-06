using System.Threading;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class UnsubscriptionRequest
    {
        [JsonProperty("event")]
        public string Event { get; set; }
        [JsonProperty("chanId")]
        public int ChannelId { get; set; }
        [JsonIgnore]
        public ManualResetEvent ConfirmedEvent { get; }

        public UnsubscriptionRequest(int channelId)
        {
            Event = "unsubscribe";
            ChannelId = channelId;
            ConfirmedEvent = new ManualResetEvent(false);
        }
    }
}
