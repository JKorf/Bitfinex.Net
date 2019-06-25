using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexCandle
    {
        /// <summary>
        /// The timestamp of the candle
        /// </summary>
        [ArrayProperty(0), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// The opening price
        /// </summary>
        [ArrayProperty(1)]
        public decimal Open { get; set; }
        /// <summary>
        /// The closing price
        /// </summary>
        [ArrayProperty(2)]
        public decimal Close { get; set; }
        /// <summary>
        /// The highest price
        /// </summary>
        [ArrayProperty(3)]
        public decimal High { get; set; }
        /// <summary>
        /// The lowest price
        /// </summary>
        [ArrayProperty(4)]
        public decimal Low { get; set; }
        /// <summary>
        /// The volume for this candle
        /// </summary>
        [ArrayProperty(5)]
        public decimal Volume { get; set; }
    }
}
