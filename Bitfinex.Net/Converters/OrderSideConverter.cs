using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class OrderSideConverter: JsonConverter
    {
        private readonly bool quotes;

        public OrderSideConverter()
        {
            quotes = true;
        }

        public OrderSideConverter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<OrderSide, string> values = new Dictionary<OrderSide, string>()
        {
            { OrderSide.Buy, "buy" },
            { OrderSide.Sell, "sell" },
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(OrderSide)value]);
            else
                writer.WriteRawValue(values[(OrderSide)value]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == reader.Value.ToString()).Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(OrderSide);
        }
    }
}
