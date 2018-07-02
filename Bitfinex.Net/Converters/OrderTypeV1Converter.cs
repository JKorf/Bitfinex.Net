using System.Collections.Generic;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    public class OrderTypeConverter: BaseConverter<OrderType>
    {
        public OrderTypeConverter(): this(true) { }
        public OrderTypeConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<OrderType, string> Mapping => new Dictionary<OrderType, string>()
        {
            { OrderType.Limit, "LIMIT" },
            { OrderType.Market, "MARKET" },
            { OrderType.Stop, "STOP" },
            { OrderType.StopLimit, "STOP LIMIT" },
            { OrderType.TrailingStop, "TRAILING STOP" },
            { OrderType.ExchangeMarket, "EXCHANGE MARKET" },
            { OrderType.ExchangeLimit, "EXCHANGE LIMIT" },
            { OrderType.ExchangeStop, "EXCHANGE STOP" },
            { OrderType.ExchangeStopLimit, "EXCHANGE STOP LIMIT" },
            { OrderType.ExchangeTrailingStop, "EXCHANGE TRAILING STOP" },
            { OrderType.FillOrKill, "FOK" },
            { OrderType.ExchangeFillOrKill, "EXCHANGE FOK" },
        };
    }
}
