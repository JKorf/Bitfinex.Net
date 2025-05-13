using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;

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
    }
}
