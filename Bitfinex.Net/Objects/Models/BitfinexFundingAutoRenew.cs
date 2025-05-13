using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Converters;
using Bitfinex.Net.Converters;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Activate or deactivate auto-renew. Allows you to specify the currency, amount, rate, and period.
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<BitfinexFundingAutoRenew>))]
    [SerializationModel]
    public record BitfinexFundingAutoRenew
    {
        /// <summary>
        /// Currency (USD, …)
        /// </summary>
        [ArrayProperty(0)]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Period in days.
        /// </summary>
        [ArrayProperty(1)]
        public int Period { get; set; }

        /// <summary>
        /// The rate of the order
        /// </summary>
        [ArrayProperty(2)]
        public decimal Rate { get; set; }

        /// <summary>
        /// Max amount to be auto-renewed
        /// </summary>
        [ArrayProperty(3)]
        public decimal MaxQuantity { get; set; }
    }
}
