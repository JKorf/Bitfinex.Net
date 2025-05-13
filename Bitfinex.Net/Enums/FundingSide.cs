using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Funding side
    /// </summary>
    [JsonConverter(typeof(EnumConverter<FundingSide>))]
    [SerializationModel]
    public enum FundingSide
    {
        /// <summary>
        /// Lender
        /// </summary>
        [Map("1")]
        Lender,
        /// <summary>
        /// Borrower
        /// </summary>
        [Map("-1")]
        Borrower,
        /// <summary>
        /// Both
        /// </summary>
        [Map("0")]
        Both
    }
}
