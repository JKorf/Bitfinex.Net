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
        /// ["<c>trading</c>"] Trading
        /// </summary>
        [Map("trading")]
        Trading,
        /// <summary>
        /// ["<c>exchange</c>"] Exchange
        /// </summary>
        [Map("exchange")]
        Exchange,
        /// <summary>
        /// ["<c>deposit</c>"] Deposit
        /// </summary>
        [Map("deposit")]
        Deposit
    }
}
