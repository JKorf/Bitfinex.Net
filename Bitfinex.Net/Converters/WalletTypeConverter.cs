using System.Collections.Generic;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    internal class WalletTypeConverter: BaseConverter<WalletType>
    {
        public WalletTypeConverter(): this(true) { }
        public WalletTypeConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<WalletType, string>> Mapping => new List<KeyValuePair<WalletType, string>>
        {
            new KeyValuePair<WalletType, string>(WalletType.Exchange, "exchange"),
            new KeyValuePair<WalletType, string>(WalletType.Funding, "funding"),
            new KeyValuePair<WalletType, string>(WalletType.Margin, "margin")
        };
    }
}
