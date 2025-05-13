using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Result V2.
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexWriteResult>))]
    [SerializationModel]
    public record BitfinexWriteResult
    {
        /// <summary>
        /// Millisecond Time Stamp of the update.
        /// </summary>
        [ArrayProperty(0), JsonConverter(typeof(DateTimeConverter))]
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

    /// <inheritdoc />
    [JsonConverter(typeof(ArrayConverter<BitfinexWriteResultFundingOffer>))]
    [SerializationModel]
    public record BitfinexWriteResultFundingOffer : BitfinexWriteResult
    {
        /// <summary>
        /// Funding offer
        /// </summary>
        [ArrayProperty(4)]
        [JsonConversion]
        public BitfinexFundingOffer Data { get; set; } = default!;
    }

    /// <inheritdoc />
    [JsonConverter(typeof(ArrayConverter<BitfinexFundingAutoRenew>))]
    [SerializationModel]
    public record BitfinexWriteResultFundingAutoRenew : BitfinexWriteResult
    {
        /// <summary>
        /// Auto renew
        /// </summary>
        [ArrayProperty(4)]
        [JsonConversion]
        public BitfinexFundingAutoRenew Data { get; set; } = default!;
    }

    /// <inheritdoc />
    [JsonConverter(typeof(ArrayConverter<BitfinexWriteResultDepositAddress>))]
    [SerializationModel]
    public record BitfinexWriteResultDepositAddress : BitfinexWriteResult
    {
        /// <summary>
        /// Deposit address
        /// </summary>
        [ArrayProperty(4)]
        [JsonConversion]
        public BitfinexDepositAddress Data { get; set; } = default!;
    }

    /// <inheritdoc />
    [JsonConverter(typeof(ArrayConverter<BitfinexWriteResultTransfer>))]
    [SerializationModel]
    public record BitfinexWriteResultTransfer : BitfinexWriteResult
    {
        /// <summary>
        /// Transfer info
        /// </summary>
        [ArrayProperty(4)]
        [JsonConversion]
        public BitfinexTransfer Data { get; set; } = default!;
    }

    /// <inheritdoc />
    [JsonConverter(typeof(ArrayConverter<BitfinexWriteResultOrder>))]
    [SerializationModel]
    public record BitfinexWriteResultOrder : BitfinexWriteResult
    {
        /// <summary>
        /// Order info
        /// </summary>
        [ArrayProperty(4)]
        [JsonConversion]
        public BitfinexOrder Data { get; set; } = default!;
    }

    /// <inheritdoc />
    [JsonConverter(typeof(ArrayConverter<BitfinexWriteResultPosition>))]
    [SerializationModel]
    public record BitfinexWriteResultPosition : BitfinexWriteResult
    {
        /// <summary>
        /// Position info
        /// </summary>
        [ArrayProperty(4)]
        [JsonConversion]
        public BitfinexPosition Data { get; set; } = default!;
    }

    /// <inheritdoc />
    [JsonConverter(typeof(ArrayConverter<BitfinexWriteResultPositionBasic>))]
    [SerializationModel]
    public record BitfinexWriteResultPositionBasic : BitfinexWriteResult
    {
        /// <summary>
        /// Position info
        /// </summary>
        [ArrayProperty(4)]
        [JsonConversion]
        public BitfinexPositionBasic Data { get; set; } = default!;
    }

    /// <inheritdoc />
    [JsonConverter(typeof(ArrayConverter<BitfinexWriteResultOrders>))]
    [SerializationModel]
    public record BitfinexWriteResultOrders : BitfinexWriteResult
    {
        /// <summary>
        /// Orders
        /// </summary>
        [ArrayProperty(4)]
        [JsonConversion]
        public BitfinexOrder[] Data { get; set; } = default!;
    }
}
