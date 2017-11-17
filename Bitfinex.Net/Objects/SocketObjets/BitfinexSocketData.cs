using Bitfinex.Net.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Net.Objects.SocketObjets
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexSocketData
    {
        [BitfinexProperty(0)]
        public long ChannelId { get; set; }
        [BitfinexProperty(1)]
        public JToken Data { get; set; }
    }
}
