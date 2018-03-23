using System.Collections.Generic;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    public class OrderSideConverter: BaseConverter<OrderSide>
    {
        public OrderSideConverter(): this(true) { }
        public OrderSideConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<OrderSide, string> Mapping => new Dictionary<OrderSide, string>()
        {
            { OrderSide.Buy, "buy" },
            { OrderSide.Sell, "sell" },
        };
    }
}
