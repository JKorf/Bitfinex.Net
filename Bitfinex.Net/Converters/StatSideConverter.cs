using System.Collections.Generic;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    public class StatSideConverter: BaseConverter<StatSide>
    {
        public StatSideConverter(): this(true) { }
        public StatSideConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<StatSide, string> Mapping => new Dictionary<StatSide, string>()
        {
            { StatSide.Long, "long" },
            { StatSide.Short, "short" }
        };
    }
}
