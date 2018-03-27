using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexWallet
    {
        [ArrayProperty(0), JsonConverter(typeof(WalletTypeConverter))]
        public WalletType Type { get; set; }

        [ArrayProperty(1)]
        public string Currency { get; set; }

        [ArrayProperty(2)]
        public decimal Balance { get; set; }

        [ArrayProperty(3)]
        public decimal UnsettledInterest { get; set; }

        [ArrayProperty(4)]
        public decimal? BalanceAvailable { get; set; }
    }
}
