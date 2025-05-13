using CryptoExchange.Net.Converters.SystemTextJson;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;
using System;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Withdrawal result
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexWithdrawalResultV2>))]
    [SerializationModel]
    public record BitfinexWithdrawalResultV2
    {
        /// <summary>
        /// Timestamp
        /// </summary>
        [ArrayProperty(0), JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Notification type
        /// </summary>
        [ArrayProperty(1)]
        public string NotificationType { get; set; } = string.Empty;
        /// <summary>
        /// Withdrawal info
        /// </summary>
        [ArrayProperty(4)]
        public BitfinexWithdrawalInfo Data { get; set; } = null!;
        /// <summary>
        /// Request status
        /// </summary>
        [ArrayProperty(6)]
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// Message
        /// </summary>
        [ArrayProperty(7)]
        public string Info { get; set; } = string.Empty;
    }

    /// <summary>
    /// Withdrawal info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexWithdrawalInfo>))]
    [SerializationModel]
    public record BitfinexWithdrawalInfo
    {
        /// <summary>
        /// Withdrawal id
        /// </summary>
        [ArrayProperty(0)]
        public long WithdrawalId { get; set; }
        /// <summary>
        /// Withdrawal method
        /// </summary>
        [ArrayProperty(2)]
        public string Method { get; set; } = string.Empty;
        /// <summary>
        /// Payment id
        /// </summary>
        [ArrayProperty(3)]
        public string? PaymentId { get; set; }
        /// <summary>
        /// Wallet type
        /// </summary>
        [ArrayProperty(4)]

        public WithdrawWallet Wallet { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        [ArrayProperty(5)]
        public decimal Quantity { get; set; }
        /// <summary>
        /// Withdrawal fee
        /// </summary>
        [ArrayProperty(8)]
        public decimal Fee { get; set; }
    }
}
