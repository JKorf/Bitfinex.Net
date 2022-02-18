using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Wallet movement info (deposit/withdraw)
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexMovement
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
        /// The full name of the asset
        /// </summary>
        [ArrayProperty(2)]
        public string AssetName { get; set; } = string.Empty;
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
        /// The transaction id
        /// </summary>
        [ArrayProperty(20)]
        public string TransactionId { get; set; } = string.Empty;
    }
}
