using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;

namespace Bitfinex.Net.Objects
{
    /// <summary>
    /// Result V2.
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexWriteResult<T>
        where T: class
    {
        /// <summary>
        /// Millisecond Time Stamp of the update.
        /// </summary>
        [ArrayProperty(0), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Purpose of notification ('on-req', 'oc-req', 'uca', 'fon-req', 'foc-req').
        /// </summary>
        [ArrayProperty(1)]
        public string? Type { get; set; }

        /// <summary>
        /// Unique ID of the message.
        /// </summary>
        [ArrayProperty(2)]
        public long Id { get; set; }

        /// <summary>
        /// </summary>
        [ArrayProperty(3)]
        public string? Placeholder1 { get; set; }

        /// <summary>
        /// Data object.
        /// </summary>
        [ArrayProperty(4)]
        public T? Data { get; set; }

        /// <summary>
        /// Work in progress.
        /// </summary>
        [ArrayProperty(5)]
        public int? Code { get; set; }

        /// <summary>
        /// Status of the notification; it may vary over time (SUCCESS, ERROR, FAILURE, ...).
        /// </summary>
        [ArrayProperty(6)]
        public string? Status { get; set; }

        /// <summary>
        /// Text of the notification.
        /// </summary>
        [ArrayProperty(7)]
        public string? Text { get; set; }
    }
}
