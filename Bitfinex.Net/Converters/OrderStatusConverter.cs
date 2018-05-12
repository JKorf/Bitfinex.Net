using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class OrderStatusConverter: JsonConverter
    {
        private readonly Dictionary<string, OrderStatus> mapping = new Dictionary<string, OrderStatus>()
        {
            { "ACTIVE", OrderStatus.Active },
            { "EXECUTED", OrderStatus.Executed },
            { "PARTIALLY FILLED",OrderStatus.PartiallyFilled },
            { "INSUFFICIENT BALANCE (G1) was: PARTIALLY FILLED", OrderStatus.PartiallyFilled },
            { "CANCELED", OrderStatus.Canceled },
            { "POSTONLY CANCELED", OrderStatus.Canceled },
            { "FILLORKILL CANCELED", OrderStatus.Canceled }
        };
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return OrderStatus.Unknown;

            var statusString = reader.Value.ToString();
            var split = statusString.Split(new[] {" @ "}, StringSplitOptions.RemoveEmptyEntries);
            var result = mapping.SingleOrDefault(m => m.Key == split[0]);
            if (result.Equals(default(KeyValuePair<string, OrderStatus>)))
            {
                Debug.WriteLine($"Couldn't deserialize order status: {reader.Value}");
                return OrderStatus.Unknown;
            }

            return result.Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(OrderStatus);
        }
    }
}
