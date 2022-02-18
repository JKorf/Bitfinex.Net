using System.Collections.Generic;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    internal class FundingOfferTypeConverter : BaseConverter<FundingOfferType>
    {
        public FundingOfferTypeConverter(): this(true) { }
        public FundingOfferTypeConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<FundingOfferType, string>> Mapping => new List<KeyValuePair<FundingOfferType, string>>
        {
            new KeyValuePair<FundingOfferType, string>(FundingOfferType.Limit, "LIMIT"),
            new KeyValuePair<FundingOfferType, string>(FundingOfferType.FlashReturnRateDeltaFixed, "FRRDELTAVAR"),
            new KeyValuePair<FundingOfferType, string>(FundingOfferType.FlashReturnRateDeltaVariable, "FRRDELTAFIX")
        };
    }
}
