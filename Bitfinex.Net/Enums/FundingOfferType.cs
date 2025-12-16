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
        /// Lend
        /// </summary>
        [Map("LIMIT")]
        Limit,
        /// <summary>
        /// FFR delta var
        /// </summary>
        [Map("FRRDELTAVAR")]
        FlashReturnRateDeltaVariable,
        /// <summary>
        /// FFR delta fix
        /// </summary>
        [Map("FRRDELTAFIX")]
        FlashReturnRateDeltaFixed
    }
}
