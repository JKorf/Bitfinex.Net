using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexAlert
    {
        [ArrayProperty(0)]
        public string AlertKey { get; set; }

        [ArrayProperty(1)]
        public string AlertType { get; set; }

        [ArrayProperty(2)]
        public string Symbol { get; set; }

        [ArrayProperty(3)]
        public decimal Price { get; set; }

        [ArrayProperty(4)]
        // TODO what is this value?
        public decimal T { get; set; }
    }
}
