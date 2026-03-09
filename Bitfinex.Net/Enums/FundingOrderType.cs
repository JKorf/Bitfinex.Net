using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Funding - Order Types.
    /// </summary>
    [JsonConverter(typeof(EnumConverter<FundingOrderType>))]
    [SerializationModel]
    public enum FundingOrderType
    {
        /// <summary>
        /// ["<c>LIMIT</c>"] Place an order at an explicit, static rate.
        /// </summary>
        [Map("LIMIT")]
        Limit,
        /// <summary>
        /// ["<c>FRRDELTAVAR</c>"] Place an order at an implicit, static rate, relative to the FRR.
        /// </summary>
        [Map("FRRDELTAVAR")]
        FRRDeltaVar,
        /// <summary>
        /// ["<c>FRRDELTAFIX</c>"] Place an order at an implicit, dynamic rate, relative to the FRR.
        /// </summary>
        [Map("FRRDELTAFIX")]
        FRRDeltaFix,
    }
}
