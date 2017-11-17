using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.SocketObjets
{
    public class BitfinexSubscribeRequest
    {
        [JsonProperty("event")]
        public string Event { get; set; }
        [JsonProperty("channel")]
        public string Channel { get; set; }

        public BitfinexSubscribeRequest(string channel)
        {
            Event = "subscribe";
            Channel = channel;
        }
    }

    public class BitfinexTickerSubscribeRequest: BitfinexSubscribeRequest
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        public BitfinexTickerSubscribeRequest(string symbol): base("ticker")
        {
            Symbol = symbol;
        }
    }

    public class BitfinexTradeSubscribeRequest : BitfinexSubscribeRequest
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        public BitfinexTradeSubscribeRequest(string symbol) : base("trades")
        {
            Symbol = symbol;
        }
    }
}
