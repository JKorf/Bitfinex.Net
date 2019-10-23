using Bitfinex.Net.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.SocketObjects
{
    internal class BitfinexMultiCancel
    {
        [JsonProperty("all"), JsonConverter(typeof(BoolToIntConverter))]
        public bool? All { get; set; }
        [JsonProperty("id")]
        public IEnumerable<long>? OrderIds { get; set; }
        [JsonProperty("cid")]
        public object[][]? ClientIds { get; set; }
        [JsonProperty("gid")]
        public long[][]? GroupIds { get; set; }
    }
}
