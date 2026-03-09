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
        /// ["<c>exchange</c>"] Exchange
        /// </summary>
        [Map("exchange")]
        Exchange,
        /// <summary>
        /// ["<c>margin</c>"] Margin
        /// </summary>
        [Map("margin")]
        Margin,
        /// <summary>
        /// ["<c>funding</c>"] Funding
        /// </summary>
        [Map("funding")]
        Funding
    }
}
