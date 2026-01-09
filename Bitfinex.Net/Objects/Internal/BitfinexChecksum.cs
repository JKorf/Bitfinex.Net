using CryptoExchange.Net.Converters;
using System;

namespace Bitfinex.Net.Objects.Internal
{
    [JsonConverter(typeof(ArrayConverter<BitfinexChecksum>))]
    [SerializationModel]
    internal class BitfinexChecksum
    {
        [ArrayProperty(0)]
        public int ChannelId { get; set; }
        [ArrayProperty(1)]
        public string Topic { get; set; } = string.Empty;
        [ArrayProperty(2)]
        public int Checksum { get; set; }
        [ArrayProperty(3)]
        public long Sequence { get; set; }
        [ArrayProperty(4), JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }
    }
}
