using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;
namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Fund type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<FundType>))]
    [SerializationModel]
    public enum FundType
    {
        /// <summary>
        /// Credit
        /// </summary>
        [JsonPropertyName("credit")]
        Credit,
        /// <summary>
        /// Loan
        /// </summary>
        [JsonPropertyName("loan")]
        Loan
    }
}
