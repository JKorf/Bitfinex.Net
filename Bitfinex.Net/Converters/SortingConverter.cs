using System.Collections.Generic;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    public class SortingConverter : BaseConverter<Sorting>
    {
        public SortingConverter(): this(true) { }
        public SortingConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<Sorting, string> Mapping => new Dictionary<Sorting, string>()
        {
            { Sorting.NewFirst, "-1" },
            { Sorting.OldFirst, "1" }
        };
    }
}
