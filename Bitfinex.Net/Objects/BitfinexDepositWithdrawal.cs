using System;
using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    public class BitfinexDepositWithdrawal
    {
        public long Id { get; set; }
        public string Currency { get; set; }
        [JsonConverter(typeof(WithdrawTypeConverter))]
        public WithdrawType Method { get; set; }
        [JsonConverter(typeof(WithdrawalDespositTypeConverter))]
        public WithdrawalDespositType Type { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Status { get; set; } // TODO possible status?
        [JsonConverter(typeof(TimestampSecondsConverter))]
        public DateTime Timestamp { get; set; }
        [JsonProperty("timestamp_created"), JsonConverter(typeof(TimestampSecondsConverter))]
        public DateTime TimestampCreated { get; set; }
        [JsonProperty("txid")]
        public string TransactionId { get; set; }
        public double Fee { get; set; }
    }
}
