using System.Collections.Generic;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    internal class StatKeyConverter: BaseConverter<StatKey>
    {
        public StatKeyConverter(): this(true) { }
        public StatKeyConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<StatKey, string>> Mapping => new List<KeyValuePair<StatKey, string>>
        {
            new KeyValuePair<StatKey, string>(StatKey.ActiveFundingInPositions, "credits.size"),
            new KeyValuePair<StatKey, string>(StatKey.ActiveFundingInPositionsPerTradingSymbol, "credits.size.sym"),
            new KeyValuePair<StatKey, string>(StatKey.TotalActiveFunding , "funding.size"),
            new KeyValuePair<StatKey, string>(StatKey.TotalOpenPosition , "pos.size")
        };
    }
}
