using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Margin base
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexMarginBase
    {
        /// <summary>
        /// Type
        /// </summary>
        [ArrayProperty(0)]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Data
        /// </summary>
        [ArrayProperty(1), JsonConverter(typeof(ArrayConverter))]
        public BitfinexMarginBaseInfo Data { get; set; } = default!;
    }
    
    /// <summary>
    /// Margin base info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexMarginBaseInfo
    {
        /// <summary>
        /// User profit and loss
        /// </summary>
        [ArrayProperty(0)]
        public decimal UserProfitLoss { get; set; }

        /// <summary>
        /// Amount of swaps
        /// </summary>
        [ArrayProperty(1)]
        public decimal UserSwapsAmount { get; set; }

        /// <summary>
        /// Balance in margin funding account
        /// </summary>
        [ArrayProperty(2)]
        public decimal MarginBalance { get; set; }

        /// <summary>
        /// Balance after profit/loss
        /// </summary>
        [ArrayProperty(3)]
        public decimal MarginNet { get; set; }
    }
}
