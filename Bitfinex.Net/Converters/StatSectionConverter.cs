using System.Collections.Generic;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    public class StatSectionConverter: BaseConverter<StatSection>
    {
        public StatSectionConverter(): this(true) { }
        public StatSectionConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<StatSection, string> Mapping => new Dictionary<StatSection, string>()
        {
            { StatSection.History, "hist" },
            { StatSection.Last, "last" }
        };
    }
}
