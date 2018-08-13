using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.RestV1Objects
{
    public class BitfinexWithdrawalResult
    {
        public string Status { get; set; }
        public string Message { get; set; }
        [JsonProperty("withdrawal_id")]
        public long WithdrawalId { get; set; }
        public decimal Fees { get; set; }
    }
}
