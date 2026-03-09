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
        /// ["<c>lend</c>"] Lend
        /// </summary>
        [Map("lend")] 
        Lend,
        /// <summary>
        /// ["<c>loan</c>"] Loan
        /// </summary>
        [Map("loan")]
        Loan
    }
}
