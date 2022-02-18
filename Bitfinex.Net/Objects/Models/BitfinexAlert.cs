using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models
{
    /// <summary>
    /// Alert
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexAlert
    {
        /// <summary>
        /// The key of the alert
        /// </summary>
        [ArrayProperty(0)]
        public string AlertKey { get; set; } = string.Empty;

        /// <summary>
        /// The type of the alert
        /// </summary>
        [ArrayProperty(1)]
        public string AlertType { get; set; } = string.Empty;

        /// <summary>
        /// The symbol the alert is for
        /// </summary>
        [ArrayProperty(2)]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The price of the alert
        /// </summary>
        [ArrayProperty(3)]
        public decimal Price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(4)]
        public decimal T { get; set; }
    }
}
