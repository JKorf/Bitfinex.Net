using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Withdraw wallet type
    /// </summary>
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
