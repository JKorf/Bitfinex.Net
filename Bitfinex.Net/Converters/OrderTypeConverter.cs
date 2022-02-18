using System.Collections.Generic;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Bitfinex.Net.Converters
{
    internal class OrderTypeConverter: BaseConverter<OrderType>
    {
        public OrderTypeConverter(): this(true) { }
        public OrderTypeConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<OrderType, string>> Mapping => new List<KeyValuePair<OrderType, string>>
        {
            new KeyValuePair<OrderType, string>(OrderType.Limit, "LIMIT"),
            new KeyValuePair<OrderType, string>(OrderType.Market, "MARKET"),
            new KeyValuePair<OrderType, string>(OrderType.Stop, "STOP"),
            new KeyValuePair<OrderType, string>(OrderType.StopLimit, "STOP LIMIT"),
            new KeyValuePair<OrderType, string>(OrderType.TrailingStop, "TRAILING STOP"),
            new KeyValuePair<OrderType, string>(OrderType.ExchangeMarket, "EXCHANGE MARKET"),
            new KeyValuePair<OrderType, string>(OrderType.ExchangeLimit, "EXCHANGE LIMIT"),
            new KeyValuePair<OrderType, string>(OrderType.ExchangeStop, "EXCHANGE STOP"),
            new KeyValuePair<OrderType, string>(OrderType.ExchangeStopLimit, "EXCHANGE STOP LIMIT"),
            new KeyValuePair<OrderType, string>(OrderType.ExchangeTrailingStop, "EXCHANGE TRAILING STOP"),
            new KeyValuePair<OrderType, string>(OrderType.FillOrKill, "FOK"),
            new KeyValuePair<OrderType, string>(OrderType.ExchangeFillOrKill, "EXCHANGE FOK")
        };
    }
}
