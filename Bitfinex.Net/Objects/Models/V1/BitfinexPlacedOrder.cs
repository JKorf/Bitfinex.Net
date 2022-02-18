using System;
using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects.Models.V1
{
    /// <summary>
    /// Placed order info
    /// </summary>
    public class BitfinexPlacedOrder
    {
        /// <summary>
        /// The id of the order
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// The symbol the order is for
        /// </summary>
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// On what exchange
        /// </summary>
        public string Exchange { get; set; } = string.Empty;
        /// <summary>
        /// The price of the order
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// The average execution price (for market orders)
        /// </summary>
        [JsonProperty("avg_execution_price")]
        public decimal AverageExecutionPrice { get; set; }
        /// <summary>
        /// The side of the order
        /// </summary>
        [JsonConverter(typeof(OrderSideConverter))]
        public OrderSide Side { get; set; }
        /// <summary>
        /// The type of the order
        /// </summary>
        [JsonConverter(typeof(OrderTypeV1Converter))]
        public OrderTypeV1 Type { get; set; }
        /// <summary>
        /// The timestamp of the order
        /// </summary>
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// If the order is live
        /// </summary>
        [JsonProperty("is_live")]
        public bool Live { get; set; }
        /// <summary>
        /// If the order is canceled
        /// </summary>
        [JsonProperty("is_cancelled")]
        public bool Canceled { get; set; }
        /// <summary>
        /// If the order is hidden
        /// </summary>
        [JsonProperty("is_hidden")]
        public bool Hidden { get; set; }
        /// <summary>
        /// If order was forced (margin only)
        /// </summary>
        [JsonProperty("was_forced")]
        public bool Forced { get; set; }
        /// <summary>
        /// The original quantity of the order
        /// </summary>
        [JsonProperty("original_amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// The remaining quantity of the order
        /// </summary>
        [JsonProperty("remaining_amount")]
        public decimal QuantityRemaining { get; set; }
        /// <summary>
        /// The executed quantity of the order
        /// </summary>
        [JsonProperty("executed_amount")]
        public decimal QuantityFilled { get; set; }
        /// <summary>
        /// The group id
        /// </summary>
        [JsonProperty("gid")]
        public long? GroupId { get; set; }
        /// <summary>
        /// The client order id
        /// </summary>
        [JsonProperty("cid")]
        public long ClientOrderId { get; set; }
        /// <summary>
        /// The client order date
        /// </summary>
        [JsonProperty("cid_date")]
        public string ClientOrderDate { get; set; } = string.Empty;
        /// <summary>
        /// The source of the order
        /// </summary>
        [JsonProperty("src")]
        public string Source { get; set; } = string.Empty;
        /// <summary>
        /// If this was an OneCancelsOther order this is the id of the other order
        /// </summary>
        [JsonProperty("oco_order")]
        public long? OcoOrder { get; set; }

        /// <summary>
        /// Affiliate code for the order
        /// </summary>
        [JsonProperty("aff_code")]
        public string? AffiliateCode { get; set; }        
    }
}
