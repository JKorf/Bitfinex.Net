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
        /// ["<c>1m</c>"] 1m
        /// </summary>
        [Map("1m")]
        OneMinute = 60,
        /// <summary>
        /// ["<c>5m</c>"] 5m
        /// </summary>
        [Map("5m")]
        FiveMinutes = 60 * 5,
        /// <summary>
        /// ["<c>15m</c>"] 15m
        /// </summary>
        [Map("15m")]
        FifteenMinutes = 60 * 15,
        /// <summary>
        /// ["<c>30m</c>"] 30m
        /// </summary>
        [Map("30m")]
        ThirtyMinutes = 60 * 30,
        /// <summary>
        /// ["<c>1h</c>"] 1h
        /// </summary>
        [Map("1h")]
        OneHour = 60 * 60,
        /// <summary>
        /// ["<c>3h</c>"] 3h
        /// </summary>
        [Map("3h")]
        ThreeHours = 60 * 60 * 3,
        /// <summary>
        /// ["<c>6h</c>"] 6h
        /// </summary>
        [Map("6h")]
        SixHours = 60 * 60 * 6,
        /// <summary>
        /// ["<c>12h</c>"] 12h
        /// </summary>
        [Map("12h")]
        TwelveHours = 60 * 60 * 12,
        /// <summary>
        /// ["<c>1D</c>"] 1d
        /// </summary>
        [Map("1D")]
        OneDay = 60 * 60 * 24,
        /// <summary>
        /// ["<c>7D</c>"] 7d
        /// </summary>
        [Map("7D")]
        SevenDays = 60 * 60 * 24 * 7,
        /// <summary>
        /// ["<c>14D</c>"] 14d
        /// </summary>
        [Map("14D")]
        FourteenDays = 60 * 60 * 24 * 14,
        /// <summary>
        /// ["<c>1M</c>"] 1m
        /// </summary>
        [Map("1M")]
        OneMonth = 60 * 60 * 24 * 30
    }
}
