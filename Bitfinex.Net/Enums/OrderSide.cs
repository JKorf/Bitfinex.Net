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
        /// Buy
        /// </summary>
        [Map("buy")]
        Buy,
        /// <summary>
        /// Sell
        /// </summary>
        [Map("sell")]
        Sell
    }
}
