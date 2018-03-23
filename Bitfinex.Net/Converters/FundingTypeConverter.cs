using System.Collections.Generic;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    public class FundingTypeConverter: BaseConverter<FundingType>
    {
        public FundingTypeConverter(): this(true) { }
        public FundingTypeConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<FundingType, string> Mapping => new Dictionary<FundingType, string>()
        {
            { FundingType.Lend, "lend" },
            { FundingType.Loan, "loan" },
        };
    }
}
