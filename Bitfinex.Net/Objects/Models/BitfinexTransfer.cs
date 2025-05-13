using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Converters;
using System;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Transfer info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexTransfer>))]
    [SerializationModel]
    public record BitfinexTransfer
    {
        /// <summary>
        /// Time of last update
        /// </summary>
        [ArrayProperty(0), JsonConverter(typeof(DateTimeConverter))]
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// From wallet type
        /// </summary>
        [ArrayProperty(1)]
        public string FromWallet { get; set; } = string.Empty;
        /// <summary>
        /// To wallet type
        /// </summary>
        [ArrayProperty(2)]
        public string ToWallet { get; set; } = string.Empty;
        /// <summary>
        /// Asset transfered
        /// </summary>
        [ArrayProperty(4)]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// Target asset
        /// </summary>
        [ArrayProperty(5)]
        public string? AssetTo { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        [ArrayProperty(7)]
        public decimal Quantity { get; set; }
    }
}
