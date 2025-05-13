using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Margin funding type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<MarginFundingType>))]
    [SerializationModel]
    public enum MarginFundingType
    {
        /// <summary>
        /// Daily
        /// </summary>
        [Map("0")]
        Daily,
        /// <summary>
        /// Term
        /// </summary>
        [Map("1")]
        Term
    }
}
