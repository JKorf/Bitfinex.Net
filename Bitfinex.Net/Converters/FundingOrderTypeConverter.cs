using System.Collections.Generic;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    internal class FundingOrderTypeConverter : BaseConverter<FundingOrderType>
    {
        public FundingOrderTypeConverter(): this(true) { }
        public FundingOrderTypeConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<FundingOrderType, string>> Mapping => new List<KeyValuePair<FundingOrderType, string>>
        {
            new KeyValuePair<FundingOrderType, string>(FundingOrderType.Limit, "LIMIT"),
            new KeyValuePair<FundingOrderType, string>(FundingOrderType.FRRDeltaVar, "FRRDELTAVAR"),
            new KeyValuePair<FundingOrderType, string>(FundingOrderType.FRRDeltaFix, "FRRDELTAFIX")
        };
    }
}
