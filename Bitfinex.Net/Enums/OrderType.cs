using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Order types
    /// </summary>
    [JsonConverter(typeof(EnumConverter<OrderType>))]
    [SerializationModel]
    public enum OrderType
    {
        /// <summary>
        /// ["<c>LIMIT</c>"] Limit order
        /// </summary>
        [Map("LIMIT")]
        Limit,
        /// <summary>
        /// ["<c>MARKET</c>"] Market order
        /// </summary>
        [Map("MARKET")]
        Market,
        /// <summary>
        /// ["<c>STOP</c>"] Stop order
        /// </summary>
        [Map("STOP")]
        Stop,
        /// <summary>
        /// ["<c>STOP LIMIT</c>"] Stop limit order
        /// </summary>
        [Map("STOP LIMIT")]
        StopLimit,
        /// <summary>
        /// ["<c>TRAILING STOP</c>"] Trailing stop order
        /// </summary>
        [Map("TRAILING STOP")]
        TrailingStop,
        /// <summary>
        /// ["<c>EXCHANGE MARKET</c>"] Exchange market order
        /// </summary>
        [Map("EXCHANGE MARKET")]
        ExchangeMarket,
        /// <summary>
        /// ["<c>EXCHANGE LIMIT</c>"] Exchange limit order
        /// </summary>
        [Map("EXCHANGE LIMIT")]
        ExchangeLimit,
        /// <summary>
        /// ["<c>EXCHANGE STOP</c>"] Exchange stop order
        /// </summary>
        [Map("EXCHANGE STOP")]
        ExchangeStop,
        /// <summary>
        /// ["<c>EXCHANGE STOP LIMIT</c>"] Exchange stop limit order
        /// </summary>
        [Map("EXCHANGE STOP LIMIT")]
        ExchangeStopLimit,
        /// <summary>
        /// ["<c>EXCHANGE TRAILING STOP</c>"] Exchange trailing stop order
        /// </summary>
        [Map("EXCHANGE TRAILING STOP")]
        ExchangeTrailingStop,
        /// <summary>
        /// ["<c>FOK</c>"] Fill or kill order
        /// </summary>
        [Map("FOK")]
        FillOrKill,
        /// <summary>
        /// ["<c>EXCHANGE FOK</c>"] Exchange fill or kill order
        /// </summary>
        [Map("EXCHANGE FOK")]
        ExchangeFillOrKill,
        /// <summary>
        /// ["<c>IOC</c>"] Immediate or cancel order
        /// </summary>
        [Map("IOC")]
        ImmediateOrCancel,
        /// <summary>
        /// ["<c>EXCHANGE IOC</c>"] Immediate or cancel order
        /// </summary>
        [Map("EXCHANGE IOC")]
        ExchangeImmediateOrCancel
    }
}
