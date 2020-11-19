using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bitfinex.Net.Objects.SocketObjects
{
    internal class BitfinexSocketConfig
    {
        [JsonProperty("event")] public string Event { get; set; } = "";

        [JsonProperty("flags")]
        public int Flags { get; set; }
    }
}
