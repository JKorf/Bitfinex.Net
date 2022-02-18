using System.Collections.Generic;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    internal class MarginFundingTypeConverter: BaseConverter<MarginFundingType>
    {
        public MarginFundingTypeConverter(): this(true) { }
        public MarginFundingTypeConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<MarginFundingType, string>> Mapping => new List<KeyValuePair<MarginFundingType, string>>
        {
            new KeyValuePair<MarginFundingType, string>(MarginFundingType.Daily, "0"),
            new KeyValuePair<MarginFundingType, string>(MarginFundingType.Term, "1")
        };
    }
}
