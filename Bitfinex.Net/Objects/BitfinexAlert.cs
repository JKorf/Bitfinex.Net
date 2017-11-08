using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexAlert
    {
        [BitfinexProperty(0)]
        public string AlertKey { get; set; }

        [BitfinexProperty(1)]
        public string AlertType { get; set; }

        [BitfinexProperty(2)]
        public string Symbol { get; set; }

        [BitfinexProperty(3)]
        public double Price { get; set; }

        [BitfinexProperty(4)]
        // TODO what is this value?
        public double T { get; set; }
    }
}
