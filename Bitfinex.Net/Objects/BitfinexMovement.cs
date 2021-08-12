using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
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
        /// The currency of the movement
        /// </summary>
        [ArrayProperty(1)]
        public string Currency { get; set; } = string.Empty;
        /// <summary>
        /// The full name of the currency
        /// </summary>
        [ArrayProperty(2)]
        public string CurrencyName { get; set; } = string.Empty;
        /// <summary>
        /// The initial creation time
        /// </summary>
        [ArrayProperty(5), JsonConverter(typeof(TimestampConverter))]
        public DateTime Started { get; set; }
        /// <summary>
        /// The last update time
        /// </summary>
        [ArrayProperty(6), JsonConverter(typeof(TimestampConverter))]
        public DateTime Updated { get; set; }
        /// <summary>
        /// The status of the movement
        /// </summary>
        [ArrayProperty(9)]
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// The amount of the movement
        /// </summary>
        [ArrayProperty(12)]
        public decimal Amount { get; set; }
        /// <summary>
        /// The fees of the movement
        /// </summary>
        [ArrayProperty(13)]
        public decimal Fees { get; set; }
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
