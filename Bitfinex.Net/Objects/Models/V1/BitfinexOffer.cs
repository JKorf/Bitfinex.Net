using System;
using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models.V1
{
    /// <summary>
    /// Offer info
    /// </summary>
    public class BitfinexOffer
    {
        /// <summary>
        /// The id of the offer
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// The asset of the offer
        /// </summary>
        [JsonProperty("currency")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// The rate of the offer
        /// </summary>
        [JsonProperty("rate")]
        public decimal Price { get; set; }
        /// <summary>
        /// The period in days
        /// </summary>
        public int Period { get; set; }
        /// <summary>
        /// The direction of the offer
        /// </summary>
        [JsonConverter(typeof(FundingTypeConverter))]
        public FundingType Direction { get; set; }
        /// <summary>
        /// The timestamp
        /// </summary>
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// If the offer is live
        /// </summary>
        [JsonProperty("is_live")]
        public bool IsLive { get; set; }
        /// <summary>
        /// If the offer is canceled
        /// </summary>
        [JsonProperty("is_cancelled")]
        public bool IsCanceled { get; set; }
        /// <summary>
        /// The original quantity of the offer
        /// </summary>
        [JsonProperty("original_amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// The remaining quantity on the offer
        /// </summary>
        [JsonProperty("remaining_amount")]
        public decimal QuantityRemaining { get; set; }
        /// <summary>
        /// The executed quantity of the offer
        /// </summary>
        [JsonProperty("executed_amount")]
        public decimal QuantityFilled { get; set; }
        /// <summary>
        /// The offer id
        /// </summary>
        [JsonProperty("offer_id")]
        public long OfferId { get; set; }
    }
}
