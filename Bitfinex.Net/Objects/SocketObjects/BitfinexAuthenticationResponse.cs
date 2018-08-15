using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class BitfinexAuthenticationResponse
    {
        [JsonProperty("event")]
        public string Event { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("userId")]
        public long UserId { get; set; }
        [JsonProperty("chanId")]
        public long ChannelId { get; set; }
        [JsonProperty("auth_id")]
        public string AuthenticationId { get; set; }
        [JsonProperty("caps")]
        public BitfinexApiKeyPermissions Permissions { get; set; }

        // In case of error
        [JsonProperty("code")]
        public int ErrorCode { get; set; }
        [JsonProperty("msg")]
        public string ErrorMessage { get; set; }
    }
}
