using System.Collections.Generic;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    internal class FundingSideConverter : BaseConverter<FundingSide>
    {
        public FundingSideConverter(): this(true) { }
        public FundingSideConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<FundingSide, string>> Mapping => new List<KeyValuePair<FundingSide, string>>
        {
            new KeyValuePair<FundingSide, string>(FundingSide.Lender, "1"),
            new KeyValuePair<FundingSide, string>(FundingSide.Borrower, "-1"),
            new KeyValuePair<FundingSide, string>(FundingSide.Both, "0")
        };
    }
}
