using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Internal
{
    [SerializationModel]
    internal class BitfinexMultiCancel
    {
        [JsonPropertyName("all"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool? All { get; set; }
        [JsonPropertyName("id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IEnumerable<long>? OrderIds { get; set; }
        [JsonPropertyName("cid"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public object[][]? ClientIds { get; set; }
        [JsonPropertyName("gid"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public long[][]? GroupIds { get; set; }
    }
}
