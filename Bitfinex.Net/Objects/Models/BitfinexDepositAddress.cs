using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Deposit address info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexDepositAddress>))]
    [SerializationModel]
    public record BitfinexDepositAddress
    {
        /// <summary>
        /// The deposit method
        /// </summary>
        [ArrayProperty(1)]
        public string Method { get; set; } = string.Empty;
        /// <summary>
        /// The asset the address is for
        /// </summary>
        [ArrayProperty(2)]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// The deposit address
        /// </summary>
        [ArrayProperty(4)]
        public string Address { get; set; } = string.Empty;
        /// <summary>
        /// Pool address (for currencies that require a Tag/Memo/Payment_ID)
        /// </summary>
        [ArrayProperty(5)]
        public string PoolAddress { get; set; } = string.Empty;
    }
}
