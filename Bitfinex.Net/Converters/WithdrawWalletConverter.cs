using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;
using System.Collections.Generic;

namespace Bitfinex.Net.Converters
{
    internal class WithdrawWalletConverter: BaseConverter<WithdrawWallet>
    {
        public WithdrawWalletConverter() : this(true) { }
        public WithdrawWalletConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<WithdrawWallet, string>> Mapping => new List<KeyValuePair<WithdrawWallet, string>>
        {
            new KeyValuePair<WithdrawWallet, string>(WithdrawWallet.Deposit, "deposit"),
            new KeyValuePair<WithdrawWallet, string>(WithdrawWallet.Exchange, "exchange"),
            new KeyValuePair<WithdrawWallet, string>(WithdrawWallet.Trading, "trading")
        };
    }
}
