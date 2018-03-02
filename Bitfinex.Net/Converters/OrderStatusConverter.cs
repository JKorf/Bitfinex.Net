using System.Collections.Generic;
using Bitfinex.Net.Objects;
using CryptoExchange.Net;

namespace Bitfinex.Net.Converters
{
    public class OrderStatusConverter: BaseConverter<OrderStatus>
    {
        public OrderStatusConverter(): this(true) { }
        public OrderStatusConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<OrderStatus, string> Mapping => new Dictionary<OrderStatus, string>()
        {
            { OrderStatus.Active, "ACTIVE" },
            { OrderStatus.Executed, "EXECUTED" },
            { OrderStatus.PartiallyFilled, "PARTIALLY FILLED" },
            { OrderStatus.Canceled, "CANCELED" },
        };
    }
}
