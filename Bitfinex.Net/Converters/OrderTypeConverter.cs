using System.Collections.Generic;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    public class OrderTypeV1Converter: BaseConverter<OrderTypeV1>
    {
        public OrderTypeV1Converter(): this(true) { }
        public OrderTypeV1Converter(bool quotes) : base(quotes) { }

        protected override Dictionary<OrderTypeV1, string> Mapping => new Dictionary<OrderTypeV1, string>()
        {
            { OrderTypeV1.Market, "market" },
            { OrderTypeV1.Limit, "limit" },
            { OrderTypeV1.Stop, "stop" },
            { OrderTypeV1.StopLimit, "stop limit" },
            { OrderTypeV1.TrailingStop, "trailing-stop" },
            { OrderTypeV1.FillOrKill, "fill-or-kill" },
            { OrderTypeV1.ExchangeMarket, "exchange market" },
            { OrderTypeV1.ExchangeLimit, "exchange limit" },
            { OrderTypeV1.ExchangeStop, "exchange stop" },
            { OrderTypeV1.ExchangeStopLimit, "exchange stop limit" },
            { OrderTypeV1.ExchangeTrailingStop, "exchange trailing-stop" },
            { OrderTypeV1.ExchangeFillOrKill, "exchange fill-or-kill" },
        };
    }
}
