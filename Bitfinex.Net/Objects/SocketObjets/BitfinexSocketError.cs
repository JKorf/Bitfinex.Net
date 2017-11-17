using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.SocketObjets
{
    public class BitfinexSocketError
    {
        public string Event { get; set; }
        [JsonProperty("msg")]
        public string ErrorMessage { get; set; }
        [JsonProperty("code")]
        public int ErrorCode { get; set; }
    }
}
