using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Result
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexSuccessResult>))]
    [SerializationModel]
    public record BitfinexSuccessResult
    {
        /// <summary>
        /// Whether the operation was successful
        /// </summary>
        [ArrayProperty(0)]
        public bool Success { get; set; }
    }
}
