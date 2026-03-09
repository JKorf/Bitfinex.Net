using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Side of an order
    /// </summary>
    [JsonConverter(typeof(EnumConverter<OrderSide>))]
    [SerializationModel]
    public enum OrderSide
    {
        /// <summary>
        /// ["<c>buy</c>"] Buy
        /// </summary>
        [Map("buy")]
        Buy,
        /// <summary>
        /// ["<c>sell</c>"] Sell
        /// </summary>
        [Map("sell")]
        Sell
    }
}
