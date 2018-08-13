using Bitfinex.Net.Objects;
using CryptoExchange.Net.Converters;
using System.Collections.Generic;

namespace Bitfinex.Net.Converters
{
    public class WithdrawWalletConverter: BaseConverter<WithdrawWallet>
    {
        public WithdrawWalletConverter() : this(true) { }
        public WithdrawWalletConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<WithdrawWallet, string> Mapping => new Dictionary<WithdrawWallet, string>()
        {
            { WithdrawWallet.Deposit, "deposit" },
            { WithdrawWallet.Exchange, "exchange" },
            { WithdrawWallet.Trading, "trading" },
        };
    }
}
