using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Asset info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public record BitfinexAssetInfo
    {
        /// <summary>
        /// Method name
        /// </summary>
        [ArrayProperty(0)]
        public string Method { get; set; } = string.Empty;
        /// <summary>
        /// Status of deposits
        /// </summary>
        [ArrayProperty(1), JsonConverter(typeof(BoolToIntConverter))]
        public bool DepositStatus { get; set; }
        /// <summary>
        /// Status of withdrawals
        /// </summary>
        [ArrayProperty(2), JsonConverter(typeof(BoolToIntConverter))]
        public bool WithdrawalStatus { get; set; }
        /// <summary>
        /// Payment id for deposits
        /// </summary>
        [ArrayProperty(7)]
        public string PaymentIdDeposits { get; set; } = string.Empty;
        /// <summary>
        /// Payment id for deposits
        /// </summary>
        [ArrayProperty(8)]
        public string PaymentIdWithdrawals { get; set; } = string.Empty;
        /// <summary>
        /// Network confirmations required for deposits
        /// </summary>
        [ArrayProperty(11)]
        public int DepositConfirmations { get; set; }
    }
}
