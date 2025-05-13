using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Withdraw wallet type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<WithdrawWallet>))]
    [SerializationModel]
    public enum WithdrawWallet
    {
        /// <summary>
        /// Trading
        /// </summary>
        [Map("trading")]
        Trading,
        /// <summary>
        /// Exchange
        /// </summary>
        [Map("exchange")]
        Exchange,
        /// <summary>
        /// Deposit
        /// </summary>
        [Map("deposit")]
        Deposit
    }
}
