using System;
using Bitfinex.Net.Converters;
using Bitfinex.Net.Objects.SocketObjects;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.ExchangeInterfaces;
using Newtonsoft.Json;

namespace Bitfinex.Net.Objects
{
    /// <summary>
    /// Order info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BitfinexOrder: ICommonOrder, ICommonOrderId
    {
        /// <summary>
        /// The id of the order
        /// </summary>
        [ArrayProperty(0)]
        public long Id { get; set; }

        /// <summary>
        /// The group id of the order
        /// </summary>
        [ArrayProperty(1)]
        public long? GroupId { get; set; }

        /// <summary>
        /// The client order id
        /// </summary>
        [ArrayProperty(2)]
        public long? ClientOrderId { get; set; }

        /// <summary>
        /// The symbol of the order
        /// </summary>
        [ArrayProperty(3)]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The creation time of the order
        /// </summary>
        [ArrayProperty(4), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampCreated { get; set; }

        /// <summary>
        /// The last update time
        /// </summary>
        [ArrayProperty(5), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampUpdated { get; set; }

        /// <summary>
        /// The amount left
        /// </summary>
        [ArrayProperty(6)]
        public decimal Amount { get; set; }

        /// <summary>
        /// The original amount
        /// </summary>
        [ArrayProperty(7)]
        public decimal AmountOriginal { get; set; }

        /// <summary>
        /// The order type
        /// </summary>
        [ArrayProperty(8), JsonConverter(typeof(OrderTypeConverter))]
        public OrderType Type { get; set; }

        /// <summary>
        /// The previous order type
        /// </summary>
        [ArrayProperty(9), JsonConverter(typeof(OrderTypeConverter))]
        public OrderType? TypePrevious { get; set; }
        
        /// <summary>
        /// Tif timestamp
        /// </summary>
        [ArrayProperty(10), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampTif { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(11)]
        public string PlaceHolder1 { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(12)]
        public int? Flags { get; set; }

        /// <summary>
        /// The status of the order
        /// </summary>
        [JsonIgnore]
        public OrderStatus Status => new OrderStatusConverter().FromString(StatusString);

        /// <summary>
        /// The raw status string
        /// </summary>
        [ArrayProperty(13)]
        public string StatusString { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(14)]
        public string PlaceHolder3 { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(15)]
        public string PlaceHolder4 { get; set; } = string.Empty;

        /// <summary>
        /// The price of the order
        /// </summary>
        [ArrayProperty(16)]
        public decimal Price { get; set; }

        /// <summary>
        /// The average price of the order (market orders)
        /// </summary>
        [ArrayProperty(17)]
        public decimal? PriceAverage { get; set; }
        
        /// <summary>
        /// The trailing price of the order
        /// </summary>
        [ArrayProperty(18)]
        public decimal PriceTrailing { get; set; }

        /// <summary>
        /// The aux limit price
        /// </summary>
        [ArrayProperty(19)]
        public decimal PriceAuxilliaryLimit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(20)]
        public string PlaceHolder5 { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(21)]
        public string PlaceHolder6 { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(22)]
        public string PlaceHolder7 { get; set; } = string.Empty;

        /// <summary>
        /// Whether the order is hidden
        /// </summary>
        [ArrayProperty(23)]
        public bool Hidden { get; set; }
        /// <summary>
        /// If another order caused this order to be placed (OCO) this will be that other order's ID
        /// </summary>
        [ArrayProperty(24)]
        public long? PlacedId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(25)]
        public string? PlaceHolder8 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(26)]
        public string? PlaceHolder9 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(27)]
        public string? PlaceHolder10 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(28)]
        public string? Routing { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(29)]
        public string? PlaceHolder11 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(30)]
        public string? PlaceHolder12 { get; set; }
        /// <summary>
        /// Meta data for the order
        /// </summary>
        [ArrayProperty(31)]
        [JsonConverter(typeof(BitfinexMetaConverter))]
        public BitfinexMeta? Meta { get; set; }

        string ICommonOrderId.CommonId => Id.ToString();
        string ICommonOrder.CommonSymbol => Symbol;
        decimal ICommonOrder.CommonPrice => Price;
        decimal ICommonOrder.CommonQuantity => AmountOriginal;
        IExchangeClient.OrderStatus ICommonOrder.CommonStatus => Status == OrderStatus.Canceled ? IExchangeClient.OrderStatus.Canceled:
            Status == OrderStatus.Executed ? IExchangeClient.OrderStatus.Filled:
            IExchangeClient.OrderStatus.Active;
        bool ICommonOrder.IsActive => Status == OrderStatus.Active;
        DateTime ICommonOrder.CommonOrderTime => TimestampCreated;

        IExchangeClient.OrderSide ICommonOrder.CommonSide =>
            AmountOriginal < 0 ? IExchangeClient.OrderSide.Sell : IExchangeClient.OrderSide.Buy;
        IExchangeClient.OrderType ICommonOrder.CommonType
        {
            get
            {
                if (Type == OrderType.ExchangeMarket) return IExchangeClient.OrderType.Market;
                if (Type == OrderType.ExchangeLimit) return IExchangeClient.OrderType.Limit;
                else return IExchangeClient.OrderType.Other;
            }
        }
    }
}
