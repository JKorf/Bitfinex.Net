using CryptoExchange.Net.Attributes;

namespace Bitfinex.Net.Enums
{
    /// <summary>
    /// Type of wallet
    /// </summary>
    [JsonConverter(typeof(EnumConverter<WalletType>))]
    [SerializationModel]
    public enum WalletType
    {
        /// <summary>
        /// Exchange
        /// </summary>
        [Map("exchange")]
        Exchange,
        /// <summary>
        /// Margin
        /// </summary>
        [Map("margin")]
        Margin,
        /// <summary>
        /// Funding
        /// </summary>
        [Map("funding")]
        Funding
    }
}
