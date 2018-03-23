using System.Collections.Generic;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    public class StatKeyConverter: BaseConverter<StatKey>
    {
        public StatKeyConverter(): this(true) { }
        public StatKeyConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<StatKey, string> Mapping => new Dictionary<StatKey, string>()
        {
            { StatKey.ActiveFundingInPositions, "credits.size" },
            { StatKey.ActiveFundingInPositionsPerTradingSymbol, "credits.size.sym" },
            { StatKey.TotalActiveFunding , "funding.size" },
            { StatKey.TotalOpenPosition , "pos.size" },
        };
    }
}
