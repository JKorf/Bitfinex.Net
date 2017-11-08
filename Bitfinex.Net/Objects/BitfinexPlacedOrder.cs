using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    public class BitfinexPlacedOrder: BitfinexBaseOrder
    {
        [JsonProperty("order_id")]
        public long OrderId { get; set; }
    }
}
