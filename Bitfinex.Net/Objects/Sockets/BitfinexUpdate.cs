using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Sockets
{
    [JsonConverter(typeof(ArrayConverter))]
    internal class BitfinexUpdate<T>
    {
        [ArrayProperty(0)]
        public int ChannelId { get; set; }
        [ArrayProperty(1)]
        public T Data { get; set; }
    }
}
