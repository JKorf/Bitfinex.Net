using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class OrderStatusConverter: JsonConverter
    {
        private readonly Dictionary<OrderStatus, string> mapping = new Dictionary<OrderStatus, string>()
        {
            { OrderStatus.Active, "ACTIVE" },
            { OrderStatus.Executed, "EXECUTED" },
            { OrderStatus.PartiallyFilled, "PARTIALLY FILLED" },
            { OrderStatus.Canceled, "CANCELED" },
        };
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var statusString = reader.Value.ToString();
            var split = statusString.Split(new[] {" @ "}, StringSplitOptions.RemoveEmptyEntries);
            return mapping.Single(m => m.Value == split[0]).Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(OrderStatus);
        }
    }
}
