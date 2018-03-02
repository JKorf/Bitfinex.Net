using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;
using CryptoExchange.Net;
using Newtonsoft.Json;

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
