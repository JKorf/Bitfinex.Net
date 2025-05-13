using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Asset info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexAssetInfo>))]
    [SerializationModel]
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
        [ArrayProperty(1)]
        public bool DepositStatus { get; set; }
        /// <summary>
        /// Status of withdrawals
        /// </summary>
        [ArrayProperty(2)]
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
