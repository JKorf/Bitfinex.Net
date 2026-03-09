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
        /// ["<c>0</c>"] Daily
        /// </summary>
        [Map("0")]
        Daily,
        /// <summary>
        /// ["<c>1</c>"] Term
        /// </summary>
        [Map("1")]
        Term
    }
}
