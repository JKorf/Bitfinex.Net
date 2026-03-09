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
        /// ["<c>1</c>"] Lender
        /// </summary>
        [Map("1")]
        Lender,
        /// <summary>
        /// ["<c>-1</c>"] Borrower
        /// </summary>
        [Map("-1")]
        Borrower,
        /// <summary>
        /// ["<c>0</c>"] Both
        /// </summary>
        [Map("0")]
        Both
    }
}
