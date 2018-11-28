using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.SocketV2
{
    [JsonConverter(typeof(ArrayConverter))]

    public class BitfinexChannelUpdate<T>
    {
        [ArrayProperty(0)]
        public int ChannelId { get; set; }
        [ArrayProperty(1)]
        public T Data { get; set; }
    }
}
