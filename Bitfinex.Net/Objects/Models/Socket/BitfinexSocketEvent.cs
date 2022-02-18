using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models.Socket
{
    /// <summary>
    /// Socket event wrapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexSocketEvent<T>
    {
        /// <summary>
        /// The channel id of the event
        /// </summary>
        [ArrayProperty(0)]
        private int ChannelId { get; set; }
        /// <summary>
        /// The type of the event
        /// </summary>
        public BitfinexEventType EventType { get; set; }

        /// <summary>
        /// The data
        /// </summary>
        [ArrayProperty(2), JsonConverter(typeof(ArrayConverter))]
        public T Data { get; set; } = default!;

        /// <summary>
        /// ctor
        /// </summary>
        public BitfinexSocketEvent() { }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public BitfinexSocketEvent(BitfinexEventType type, T data)
        {
            EventType = type;
            Data = data;
        }
    }
}
