using CryptoExchange.Net.Sockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bitfinex.Net.Objects.SocketV2
{
    public class BitfinexSubscriptionRequest: SocketRequest
    {
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

    public class BitfinexBookSubscriptionRequest: BitfinexSubscriptionRequest
    {
        [JsonProperty("prec")]
        public string Precision { get; set; }
        [JsonProperty("len")]
        public int Length { get; set; }

        public BitfinexBookSubscriptionRequest(string symbol, string precision, int length) : base("book", symbol)
        {
            Precision = precision;
            Length = length;
        }
    }
}
