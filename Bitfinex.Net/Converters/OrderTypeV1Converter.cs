using System.Collections.Generic;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    internal class OrderTypeV1Converter: BaseConverter<OrderTypeV1>
    {
        public OrderTypeV1Converter(): this(true) { }
        public OrderTypeV1Converter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<OrderTypeV1, string>> Mapping => new List<KeyValuePair<OrderTypeV1, string>>
        {
            new KeyValuePair<OrderTypeV1, string>( OrderTypeV1.Market, "market"),
            new KeyValuePair<OrderTypeV1, string>(OrderTypeV1.Limit, "limit"),
            new KeyValuePair<OrderTypeV1, string>(OrderTypeV1.Stop, "stop"),
            new KeyValuePair<OrderTypeV1, string>(OrderTypeV1.StopLimit, "stop-limit"),
            new KeyValuePair<OrderTypeV1, string>(OrderTypeV1.TrailingStop, "trailing-stop"),
            new KeyValuePair<OrderTypeV1, string>(OrderTypeV1.FillOrKill, "fill-or-kill"),
            new KeyValuePair<OrderTypeV1, string>(OrderTypeV1.FillOrKill, "fok"),
            new KeyValuePair<OrderTypeV1, string>(OrderTypeV1.ExchangeMarket, "exchange market"),
            new KeyValuePair<OrderTypeV1, string>(OrderTypeV1.ExchangeLimit, "exchange limit"),
            new KeyValuePair<OrderTypeV1, string>(OrderTypeV1.ExchangeStop, "exchange stop"),
            new KeyValuePair<OrderTypeV1, string>(OrderTypeV1.ExchangeStopLimit, "exchange stop-limit"),
            new KeyValuePair<OrderTypeV1, string>(OrderTypeV1.ExchangeTrailingStop, "exchange trailing-stop" ),
            new KeyValuePair<OrderTypeV1, string>(OrderTypeV1.ExchangeFillOrKill, "exchange fill-or-kill"),
            new KeyValuePair<OrderTypeV1, string>(OrderTypeV1.ExchangeFillOrKill, "exchange fok")
        };
    }
}
