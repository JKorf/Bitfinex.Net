using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Wallet info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexWallet
    {
        /// <summary>
        /// The type of the wallet
        /// </summary>
        [ArrayProperty(0), JsonConverter(typeof(WalletTypeConverter))]
        public WalletType Type { get; set; }

        /// <summary>
        /// The asset
        /// </summary>
        [ArrayProperty(1)]
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// The current balance
        /// </summary>
        [ArrayProperty(2)]
        public decimal Total { get; set; }

        /// <summary>
        /// The unsettled interest
        /// </summary>
        [ArrayProperty(3)]
        public decimal UnsettledInterest { get; set; }

        /// <summary>
        /// The available balance
        /// </summary>
        [ArrayProperty(4)]
        public decimal? Available { get; set; }
    }
}
