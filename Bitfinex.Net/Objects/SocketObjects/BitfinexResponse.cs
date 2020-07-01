using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.SocketObjects
{
    internal class BitfinexResponse
    {
        public string Event { get; set; } = "";
        public string Channel { get; set; } = "";
    }

    internal class BitfinexSubscribeResponse: BitfinexResponse
    {
        [JsonProperty("chanId")]
        public int ChannelId { get; set; }
    }

    internal class BitfinexErrorResponse: BitfinexResponse
    {
        [JsonProperty("msg")]
        public string Message { get; set; } = "";
        [JsonProperty("code")]
        public int Code { get; set; }
    }
}
