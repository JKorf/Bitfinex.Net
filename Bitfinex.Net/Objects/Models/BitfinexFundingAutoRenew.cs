using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Activate or deactivate auto-renew. Allows you to specify the currency, amount, rate, and period.
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexFundingAutoRenew
    {
        /// <summary>
        /// Millisecond Time Stamp of the update.
        /// </summary>
        [ArrayProperty(0), JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Purpose of notification ('on-req', 'oc-req', 'uca', 'fon-req', 'foc-req', 'fa-req').
        /// </summary>
        [ArrayProperty(1)]
        public string? Type { get; set; }

        /// <summary>
        /// Unique ID of the message.
        /// </summary>
        [ArrayProperty(2)]
        public long Id { get; set; }

        /// <summary>
        /// Currency (USD, …)
        /// </summary>
        [ArrayProperty(3)]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Period in days.
        /// </summary>
        [ArrayProperty(4)]
        public int Period { get; set; }

        /// <summary>
        /// The rate of the order
        /// </summary>
        [ArrayProperty(5)]
        public decimal Rate { get; set; }

        /// <summary>
        /// Max amount to be auto-renewed
        /// </summary>
        [ArrayProperty(6)]
        public decimal MaxQuantity { get; set; }

        /// <summary>
        /// Work in progress.
        /// </summary>
        [ArrayProperty(7)]
        public int? Code { get; set; }

        /// <summary>
        /// Status of the notification; it may vary over time (SUCCESS, ERROR, FAILURE, ...).
        /// </summary>
        [ArrayProperty(8)]
        public string? Status { get; set; }

        /// <summary>
        /// Text of the notification.
        /// </summary>
        [ArrayProperty(9)]
        public string? Text { get; set; }
    }
}
