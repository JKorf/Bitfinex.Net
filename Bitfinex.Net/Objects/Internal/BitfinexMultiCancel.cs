using System.Collections.Generic;
using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Internal
{
    internal class BitfinexMultiCancel
    {
        [JsonProperty("all", DefaultValueHandling = DefaultValueHandling.Ignore), JsonConverter(typeof(BoolToIntConverter))]
        public bool? All { get; set; }
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IEnumerable<long>? OrderIds { get; set; }
        [JsonProperty("cid", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object[][]? ClientIds { get; set; }
        [JsonProperty("gid", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long[][]? GroupIds { get; set; }
    }
}
