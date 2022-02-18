using System.Collections.Generic;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    internal class FundingTypeConverter: BaseConverter<FundingType>
    {
        public FundingTypeConverter(): this(true) { }
        public FundingTypeConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<FundingType, string>> Mapping => new List<KeyValuePair<FundingType, string>>
        {
            new KeyValuePair<FundingType, string>(FundingType.Lend, "lend"),
            new KeyValuePair<FundingType, string>(FundingType.Loan, "loan")
        };
    }
}
