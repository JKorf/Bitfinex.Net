using System;
using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.RestV1Objects
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
        /// The currency of the offer
        /// </summary>
        public string Currency { get; set; } = "";
        /// <summary>
        /// The rate of the offer
        /// </summary>
        public decimal Rate { get; set; }
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
        [JsonConverter(typeof(TimestampSecondsConverter))]
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
        /// The original amount of the offer
        /// </summary>
        [JsonProperty("original_amount")]
        public decimal OriginalAmount { get; set; }
        /// <summary>
        /// The remaining amount on the offer
        /// </summary>
        [JsonProperty("remaining_amount")]
        public decimal RemainingAmount { get; set; }
        /// <summary>
        /// The executed amount of the offer
        /// </summary>
        [JsonProperty("executed_amount")]
        public decimal ExecutedAmount { get; set; }
        /// <summary>
        /// The offer id
        /// </summary>
        [JsonProperty("offer_id")]
        public long OfferId { get; set; }
    }
}
