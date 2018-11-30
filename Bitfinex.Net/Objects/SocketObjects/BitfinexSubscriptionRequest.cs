using CryptoExchange.Net.Sockets;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.SocketObjects
{
    public class BitfinexSubscriptionRequest: SocketRequest
    {
        [JsonIgnore]
        public int ChannelId { get; set; }
        [JsonProperty("event")]
        public string Event { get; set; }
        [JsonProperty("channel")]
        public string Channel { get; set; }
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        public BitfinexSubscriptionRequest(string channel, string symbol)
        {
            Event = "subscribe";
            Channel = channel;
            Symbol = symbol;
        }
    }

    public class BitfinexRawBookSubscriptionRequest: BitfinexSubscriptionRequest
    {
        [JsonProperty("prec")]
        public string Precision { get; set; }
        [JsonProperty("len")]
        public int Length { get; set; }

        public BitfinexRawBookSubscriptionRequest(string symbol, string precision, int length) : base("book", symbol)
        {
            Precision = precision;
            Length = length;
        }
    }

    public class BitfinexBookSubscriptionRequest : BitfinexRawBookSubscriptionRequest
    {
        [JsonProperty("freq")]
        public string Frequency { get; set; }

        public BitfinexBookSubscriptionRequest(string symbol, string precision, string frequency, int length): base(symbol, precision, length)
        {
            Frequency = frequency;
        }
    }

    public class BitfinexKlineSubscriptionRequest : BitfinexSubscriptionRequest
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        public BitfinexKlineSubscriptionRequest(string symbol, string interval): base("candles", symbol)
        {
            Key = "trade:" + interval + ":" + symbol;
        }
    }
}
