using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Deposit address info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexDepositAddress
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
