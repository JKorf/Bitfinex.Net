using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.RestV1Objects
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
        public string Method { get; set; } = "";
        /// <summary>
        /// The currency the address is for
        /// </summary>
        public string Currency { get; set; } = "";
        /// <summary>
        /// The deposit address
        /// </summary>
        public string Address { get; set; } = "";
    }
}
