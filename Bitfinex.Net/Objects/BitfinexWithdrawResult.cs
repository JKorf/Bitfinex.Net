using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    public class BitfinexWithdrawResult
    {
        public string Status { get; set; }
        public string Message { get; set; }
        [JsonProperty("withdrawal_id")]
        public long WithdrawalId { get; set; }
        public double Fees { get; set; }
    }
}
