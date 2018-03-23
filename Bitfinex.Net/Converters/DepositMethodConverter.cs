using System.Collections.Generic;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    public class DepositMethodConverter: BaseConverter<DepositMethod>
    {
        public DepositMethodConverter(): this(true) { }
        public DepositMethodConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<DepositMethod, string> Mapping => new Dictionary<DepositMethod, string>()
        {
            { DepositMethod.Bitcoin, "bitcoin" },
            { DepositMethod.Litecoin, "litecoin" },
            { DepositMethod.Ethereum, "ethereum" },
            { DepositMethod.Tether, "tetheruso" },
            { DepositMethod.EthereumClassic, "ethereumc" },
            { DepositMethod.ZCash, "zcash" },
            { DepositMethod.Monero, "monero" },
            { DepositMethod.Iota, "iota" },
            { DepositMethod.BCash, "bcash" }
        };
    }
}
