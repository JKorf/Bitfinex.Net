using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Interval
    /// </summary>
    [JsonConverter(typeof(EnumConverter<KlineInterval>))]
    [SerializationModel]
    public enum KlineInterval
    {
        /// <summary>
        /// 1m
        /// </summary>
        [Map("1m")]
        OneMinute = 60,
        /// <summary>
        /// 5m
        /// </summary>
        [Map("5m")]
        FiveMinutes = 60 * 5,
        /// <summary>
        /// 15m
        /// </summary>
        [Map("15m")]
        FifteenMinutes = 60 * 15,
        /// <summary>
        /// 30m
        /// </summary>
        [Map("30m")]
        ThirtyMinutes = 60 * 30,
        /// <summary>
        /// 1h
        /// </summary>
        [Map("1h")]
        OneHour = 60 * 60,
        /// <summary>
        /// 3h
        /// </summary>
        [Map("3h")]
        ThreeHours = 60 * 60 * 3,
        /// <summary>
        /// 6h
        /// </summary>
        [Map("6h")]
        SixHours = 60 * 60 * 6,
        /// <summary>
        /// 12h
        /// </summary>
        [Map("12h")]
        TwelveHours = 60 * 60 * 12,
        /// <summary>
        /// 1d
        /// </summary>
        [Map("1D")]
        OneDay = 60 * 60 * 24,
        /// <summary>
        /// 7d
        /// </summary>
        [Map("7D")]
        SevenDays = 60 * 60 * 24 * 7,
        /// <summary>
        /// 14d
        /// </summary>
        [Map("14D")]
        FourteenDays = 60 * 60 * 24 * 14,
        /// <summary>
        /// 1m
        /// </summary>
        [Map("1M")]
        OneMonth = 60 * 60 * 24 * 30
    }
}
