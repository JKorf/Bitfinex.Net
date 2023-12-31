﻿using Bitfinex.Net.Converters;
using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Sockets
{
    [JsonConverter(typeof(ArrayConverter))]
    internal class BitfinexUpdate<T>
    {
        [ArrayProperty(0)]
        public int ChannelId { get; set; }
        [ArrayProperty(1)]
        [JsonConversion]
        public T Data { get; set; }
    }

    [JsonConverter(typeof(ArrayConverter))]
    internal class BitfinexUpdate3<T>
    {
        [ArrayProperty(0)]
        public int ChannelId { get; set; }
        [ArrayProperty(1)]
        public string Topic { get; set; }
        [ArrayProperty(2)]
        [JsonConversion]
        public T Data { get; set; }
    }
}
