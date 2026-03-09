using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Funding type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<FundingOfferType>))]
    [SerializationModel]
    public enum FundingOfferType
    {
        /// <summary>
        /// ["<c>LIMIT</c>"] Lend
        /// </summary>
        [Map("LIMIT")]
        Limit,
        /// <summary>
        /// ["<c>FRRDELTAVAR</c>"] FFR delta var
        /// </summary>
        [Map("FRRDELTAVAR")]
        FlashReturnRateDeltaVariable,
        /// <summary>
        /// ["<c>FRRDELTAFIX</c>"] FFR delta fix
        /// </summary>
        [Map("FRRDELTAFIX")]
        FlashReturnRateDeltaFixed
    }
}
