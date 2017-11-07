using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class OrderStatusConverter: JsonConverter
    {
        private readonly bool quotes;

        public OrderStatusConverter()
        {
            quotes = true;
        }

        public OrderStatusConverter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<OrderStatus, string> values = new Dictionary<OrderStatus, string>()
        {
            { OrderStatus.Active, "ACTIVE" },
            { OrderStatus.Executed, "EXECUTED" },
            { OrderStatus.PartiallyFilled, "PARTIALLY FILLED" },
            { OrderStatus.Canceled, "CANCELED" },
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(OrderStatus)value]);
            else
                writer.WriteRawValue(values[(OrderStatus)value]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == reader.Value.ToString()).Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(OrderStatus);
        }
    }
}
