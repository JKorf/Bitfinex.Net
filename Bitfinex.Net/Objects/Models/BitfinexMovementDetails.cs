using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Wallet movement info (deposit/withdraw)
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexMovementDetails>))]
    [SerializationModel]
    public record BitfinexMovementDetails
    {
        /// <summary>
        /// The id of the movement
        /// </summary>
        [ArrayProperty(0)]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// The asset of the movement
        /// </summary>
        [ArrayProperty(1)]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// The method
        /// </summary>
        [ArrayProperty(2)]
        public string Method { get; set; } = string.Empty;
        /// <summary>
        /// Remarks
        /// </summary>
        [ArrayProperty(4)]
        public string Remark { get; set; } = string.Empty;
        /// <summary>
        /// The initial creation time
        /// </summary>
        [ArrayProperty(5), JsonConverter(typeof(DateTimeConverter))]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// The last update time
        /// </summary>
        [ArrayProperty(6), JsonConverter(typeof(DateTimeConverter))]
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// The status of the movement
        /// </summary>
        [ArrayProperty(9)]
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// The quantity of the movement
        /// </summary>
        [ArrayProperty(12)]
        public decimal Quantity { get; set; }
        /// <summary>
        /// The fee paid for the movement
        /// </summary>
        [ArrayProperty(13)]
        public decimal Fee { get; set; }
        /// <summary>
        /// The address
        /// </summary>
        [ArrayProperty(16)]
        public string Address { get; set; } = string.Empty;
        /// <summary>
        /// The memo
        /// </summary>
        [ArrayProperty(17)]
        public string? Memo { get; set; } = string.Empty;
        /// <summary>
        /// The transaction id
        /// </summary>
        [ArrayProperty(20)]
        public string TransactionId { get; set; } = string.Empty;
        /// <summary>
        /// Withdraw transaction note
        /// </summary>
        [ArrayProperty(21)]
        public string? WithdrawTransactionNote { get; set; } = string.Empty;

        /// <summary>
        /// Wire bank fees
        /// </summary>
        [ArrayProperty(24)]
        public decimal? BankFees { get; set; }
        /// <summary>
        /// Identifier of bank router
        /// </summary>
        [ArrayProperty(25)]
        public string? BankRouterId { get; set; }
        /// <summary>
        /// External provider movement id
        /// </summary>
        [ArrayProperty(28)]
        public string? ExternalProviderMovementId { get; set; }
        /// <summary>
        /// External provider movement status
        /// </summary>
        [ArrayProperty(29)]
        public string? ExternalProviderMovementStatus { get; set; }
        /// <summary>
        /// External provider movement description
        /// </summary>
        [ArrayProperty(30)]
        public string? ExternalProviderMovementDescription { get; set; }
    }
}
