using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    /// <summary>
    /// Result
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexSuccessResult
    {
        /// <summary>
        /// Whether the operation was successful
        /// </summary>
        [ArrayProperty(0)]
        public bool Success { get; set; }
    }
}
