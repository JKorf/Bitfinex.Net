using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.SocketObjects
{
    internal class BitfinexAuthenticationResponse
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

    internal class BitfinexApiKeyPermissions
    {
        public BitfinexReadWritePermission Account { get; set; }
        public BitfinexReadWritePermission History { get; set; }
        public BitfinexReadWritePermission Orders { get; set; }
        public BitfinexReadWritePermission Positions { get; set; }
        public BitfinexReadWritePermission Funding { get; set; }
        public BitfinexReadWritePermission Wallets { get; set; }
        public BitfinexReadWritePermission Withdraw { get; set; }
    }

    internal class BitfinexReadWritePermission
    {
        public bool Read { get; set; }
        public bool Write { get; set; }
    }
}
