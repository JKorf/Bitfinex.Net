using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class OrderStatusConverter: JsonConverter
    {
        private readonly Dictionary<string, OrderStatus> mapping = new Dictionary<string, OrderStatus>()
        {
            {  "ACTIVE", OrderStatus.Active },
            {  "EXECUTED", OrderStatus.Executed},
            {  "PARTIALLY FILLED",OrderStatus.PartiallyFilled},
            {  "CANCELED", OrderStatus.Canceled},
            {  "INSUFFICIENT BALANCE (G1) was: PARTIALLY FILLED", OrderStatus.PartiallyFilled }
        };
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var statusString = reader.Value.ToString();
            var split = statusString.Split(new[] {" @ "}, StringSplitOptions.RemoveEmptyEntries);
            return mapping.Single(m => m.Key == split[0]).Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(OrderStatus);
        }
    }
}
