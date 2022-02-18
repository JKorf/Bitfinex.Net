using System.Collections.Generic;
using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Internal
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
