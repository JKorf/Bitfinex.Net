using System.Collections.Generic;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    public class MarginFundingTypeConverter: BaseConverter<MarginFundingType>
    {
        public MarginFundingTypeConverter(): this(true) { }
        public MarginFundingTypeConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<MarginFundingType, string> Mapping => new Dictionary<MarginFundingType, string>()
        {
            { MarginFundingType.Daily, "0" },
            { MarginFundingType.Term, "1" }
        };
    }
}
