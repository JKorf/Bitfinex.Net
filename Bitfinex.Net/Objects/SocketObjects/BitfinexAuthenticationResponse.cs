using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.SocketObjects
{
    internal class BitfinexAuthenticationResponse
    {
        [JsonProperty("event")]
        public string Event { get; set; } = string.Empty;
        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;
        [JsonProperty("userId")]
        public long UserId { get; set; }
        [JsonProperty("chanId")]
        public long ChannelId { get; set; }
        [JsonProperty("auth_id")]
        public string AuthenticationId { get; set; } = string.Empty;
        [JsonProperty("caps")]
        public BitfinexApiKeyPermissions Permissions { get; set; } = default!;

        // In case of error
        [JsonProperty("code")]
        public int ErrorCode { get; set; }
        [JsonProperty("msg")]
        public string? ErrorMessage { get; set; }
    }

    internal class BitfinexApiKeyPermissions
    {
        public BitfinexReadWritePermission Account { get; set; } = default!;
        public BitfinexReadWritePermission History { get; set; } = default!;
        public BitfinexReadWritePermission Orders { get; set; } = default!;
        public BitfinexReadWritePermission Positions { get; set; } = default!;
        public BitfinexReadWritePermission Funding { get; set; } = default!;
        public BitfinexReadWritePermission Wallets { get; set; } = default!;
        public BitfinexReadWritePermission Withdraw { get; set; } = default!;
    }

    internal class BitfinexReadWritePermission
    {
        public bool Read { get; set; }
        public bool Write { get; set; }
    }
}
