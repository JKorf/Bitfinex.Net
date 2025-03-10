using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Funding type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<FundingType>))]
    [SerializationModel]
    public enum FundingType
    {
        /// <summary>
        /// Lend
        /// </summary>
        [Map("lend")] 
        Lend,
        /// <summary>
        /// Loan
        /// </summary>
        [Map("loan")]
        Loan
    }
}
