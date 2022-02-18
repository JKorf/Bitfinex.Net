using System.Collections.Generic;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    internal class SortingConverter : BaseConverter<Sorting>
    {
        public SortingConverter(): this(true) { }
        public SortingConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<Sorting, string>> Mapping => new List<KeyValuePair<Sorting, string>>
        {
            new KeyValuePair<Sorting, string>(Sorting.NewFirst, "-1"),
            new KeyValuePair<Sorting, string>(Sorting.OldFirst, "1")
        };
    }
}
