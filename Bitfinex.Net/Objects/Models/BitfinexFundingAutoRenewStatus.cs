using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Funding auto renew status
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexFundingAutoRenewStatus
    {
        /// <summary>
        /// The symbol
        /// </summary>
        [ArrayProperty(0)]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The period of the offer in days
        /// </summary>
        [ArrayProperty(1)]
        public int Period { get; set; }

        /// <summary>
        /// The rate of the order
        /// </summary>
        [ArrayProperty(2)]
        public decimal Rate { get; set; }

        /// <summary>
        /// The quantity of the offer
        /// </summary>
        [ArrayProperty(3)]
        public decimal Quantity { get; set; }

    }
}
