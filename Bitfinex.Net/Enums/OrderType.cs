using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
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
        /// Limit order
        /// </summary>
        [Map("LIMIT")]
        Limit,
        /// <summary>
        /// Market order
        /// </summary>
        [Map("MARKET")]
        Market,
        /// <summary>
        /// Stop order
        /// </summary>
        [Map("STOP")]
        Stop,
        /// <summary>
        /// Stop limit order
        /// </summary>
        [Map("STOP LIMIT")]
        StopLimit,
        /// <summary>
        /// Trailing stop order
        /// </summary>
        [Map("TRAILING STOP")]
        TrailingStop,
        /// <summary>
        /// Exchange market order
        /// </summary>
        [Map("EXCHANGE MARKET")]
        ExchangeMarket,
        /// <summary>
        /// Exchange limit order
        /// </summary>
        [Map("EXCHANGE LIMIT")]
        ExchangeLimit,
        /// <summary>
        /// Exchange stop order
        /// </summary>
        [Map("EXCHANGE STOP")]
        ExchangeStop,
        /// <summary>
        /// Exchange stop limit order
        /// </summary>
        [Map("EXCHANGE STOP LIMIT")]
        ExchangeStopLimit,
        /// <summary>
        /// Exchange trailing stop order
        /// </summary>
        [Map("EXCHANGE TRAILING STOP")]
        ExchangeTrailingStop,
        /// <summary>
        /// Fill or kill order
        /// </summary>
        [Map("FOK")]
        FillOrKill,
        /// <summary>
        /// Exchange fill or kill order
        /// </summary>
        [Map("EXCHANGE FOK")]
        ExchangeFillOrKill,
        /// <summary>
        /// Immediate or cancel order
        /// </summary>
        [Map("IOC")]
        ImmediateOrCancel,
        /// <summary>
        /// Immediate or cancel order
        /// </summary>
        [Map("EXCHANGE IOC")]
        ExchangeImmediateOrCancel
    }
}
