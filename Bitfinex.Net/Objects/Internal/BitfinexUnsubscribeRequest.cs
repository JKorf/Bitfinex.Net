namespace Bitfinex.Net.Objects.Internal
{
    [SerializationModel]
    internal class BitfinexUnsubscribeRequest
    {
        [JsonPropertyName("event")]
        public string Event { get; set; }
        [JsonPropertyName("chanId")]
        public int ChannelId { get; set; }

        public BitfinexUnsubscribeRequest(int channelId)
        {
            Event = "unsubscribe";
            ChannelId = channelId;
        }
    }
}
