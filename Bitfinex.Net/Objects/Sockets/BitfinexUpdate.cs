using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Objects.Sockets
{
    [JsonConverter(typeof(ArrayConverter))]
    internal class BitfinexUpdate<T>
    {
        [ArrayProperty(0)]
        public int ChannelId { get; set; }
        [ArrayProperty(1)]
        [JsonConversion]
        public T Data { get; set; } = default!;
    }

    [JsonConverter(typeof(ArrayConverter))]
    internal class BitfinexTopicUpdate<T>
    {
        [ArrayProperty(0)]
        public int ChannelId { get; set; }
        [ArrayProperty(1)]
        public string Topic { get; set; } = string.Empty;
        [ArrayProperty(2)]
        [JsonConversion]
        public T Data { get; set; } = default!;
    }
}
