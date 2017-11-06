using Bitfinex.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexWallet
    {
        [BitfinexProperty(0), JsonConverter(typeof(WalletTypeConverter))]
        public WalletType Type { get; set; }

        [BitfinexProperty(1)]
        public string Currency { get; set; }

        [BitfinexProperty(2)]
        public double Balance { get; set; }

        [BitfinexProperty(3)]
        public double UnsettledInterest { get; set; }

        [BitfinexProperty(4)]
        public double? BalanceAvailable { get; set; }
    }
}
