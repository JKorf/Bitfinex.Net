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
        public WaitAction<bool> ConfirmedEvent { get; }

        public UnsubscriptionRequest(int channelId)
        {
            Event = "unsubscribe";
            ChannelId = channelId;
            ConfirmedEvent = new WaitAction<bool>();
        }
    }
}
