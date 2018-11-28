using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.SocketV2
{
    public class BitfinexResponse
    {
        public string Event { get; set; }
        public string Channel { get; set; }
    }

    public class BitfinexSubscribeResponse: BitfinexResponse
    {
        [JsonProperty("chanId")]
        public int ChannelId { get; set; }
    }

    public class BitfinexErrorResponse: BitfinexResponse
    {
        [JsonProperty("msg")]
        public string Message { get; set; }
        [JsonProperty("code")]
        public int Code { get; set; }
    }
}
