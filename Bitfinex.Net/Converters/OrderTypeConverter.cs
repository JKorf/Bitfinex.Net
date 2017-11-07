using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class OrderTypeConverter: JsonConverter
    {
        private readonly bool quotes;

        public OrderTypeConverter()
        {
            quotes = true;
        }

        public OrderTypeConverter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<OrderType, string> values = new Dictionary<OrderType, string>()
        {
            { OrderType.Limit, "LIMIT" },
            { OrderType.Market, "MARKET" },
            { OrderType.Stop, "STOP" },
            { OrderType.TrailingStop, "TRAILING STOP" },
            { OrderType.ExchangeMarket, "EXCHANGE MARKET" },
            { OrderType.ExchangeLimit, "EXCHANGE LIMIT" },
            { OrderType.ExchangeStop, "EXCHANGE STOP" },
            { OrderType.ExchangeTrailingStop, "EXCHANGE TRAILING STOP" },
            { OrderType.FOK, "FOK" },
            { OrderType.ExchangeFOK, "EXCHANGE FOK" },
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(OrderType)value]);
            else
                writer.WriteRawValue(values[(OrderType)value]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == reader.Value.ToString()).Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(OrderType);
        }
    }
}
