using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models.V1
{
    /// <summary>
    /// Deposit address info
    /// </summary>
    public class BitfinexDepositAddress
    {
        /// <summary>
        /// Whether it was successful
        /// </summary>
        [JsonConverter(typeof(StringToBoolConverter))]
        public bool Result { get; set; }
        /// <summary>
        /// The deposit method
        /// </summary>
        public string Method { get; set; } = string.Empty;
        /// <summary>
        /// The asset the address is for
        /// </summary>
        [JsonProperty("currency")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// The deposit address
        /// </summary>
        public string Address { get; set; } = string.Empty;
    }
}
