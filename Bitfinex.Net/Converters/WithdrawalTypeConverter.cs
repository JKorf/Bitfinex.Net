using Bitfinex.Net.Objects;
using CryptoExchange.Net.Converters;
using System.Collections.Generic;

namespace Bitfinex.Net.Converters
{
    public class WithdrawalTypeConverter: BaseConverter<WithdrawalType>
    {
        public WithdrawalTypeConverter() : this(true) { }
        public WithdrawalTypeConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<WithdrawalType, string> Mapping => new Dictionary<WithdrawalType, string>()
        {
            { WithdrawalType.Bitcoin, "bitcoin" },
            { WithdrawalType.Litecoin, "litecoin" },
            { WithdrawalType.Ethereum, "ethereum" },
            { WithdrawalType.EthereumClassic, "ethereumc" },
            { WithdrawalType.TetherUSO, "tetheruso" },
            { WithdrawalType.ZCash, "zcash" },
            { WithdrawalType.Monero, "monero" },
            { WithdrawalType.IOTA, "iota" },
            { WithdrawalType.Ripple, "ripple" },
            { WithdrawalType.Dash, "dash" },
            { WithdrawalType.Adjustment, "adjustment" },
            { WithdrawalType.Wire, "wire" },
            { WithdrawalType.EOS, "eos" },
            { WithdrawalType.Santiment, "santiment" },
            { WithdrawalType.OmiseGo, "omisego" },
            { WithdrawalType.BitcoinCash, "bcash" },
            { WithdrawalType.Neo, "neo" },
            { WithdrawalType.MetaVerse, "metaverse" },
            { WithdrawalType.QTUM, "qtum" },
            { WithdrawalType.Aventus, "aventus" },
            { WithdrawalType.Eidoo, "eidoo" },
            { WithdrawalType.Datacoin, "datacoin" },
            { WithdrawalType.TetherUSE, "tetheruse" },
            { WithdrawalType.BitcoinGold, "bgold" },
            { WithdrawalType.Qash, "qash" },
            { WithdrawalType.Yoyow, "yoyow" },
            { WithdrawalType.Golem, "golem" },
            { WithdrawalType.Status, "status" },
            { WithdrawalType.TetherEUE, "tethereue" },
            { WithdrawalType.Bat, "bat" },
            { WithdrawalType.MNA, "mna" },
            { WithdrawalType.Fun, "fun" },
            { WithdrawalType.ZRX, "zrx" },
            { WithdrawalType.TNB, "tnb" },
            { WithdrawalType.SPK, "spk" },
            { WithdrawalType.TRX, "trx" },
            { WithdrawalType.RCN, "rcn" },
            { WithdrawalType.RLC, "rlc" },
            { WithdrawalType.AID, "aid" },
            { WithdrawalType.SNG, "sng" },
            { WithdrawalType.REP, "rep" },
            { WithdrawalType.ELF, "elf" },
        };
    }
}
