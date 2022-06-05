using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using Bitfinex.Net.Objects.Models;

namespace Bitfinex.Net.Converters
{
    internal class RawOrderBookEntryConverter : JsonConverter
    {
        public override bool CanWrite { get { return false; } }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var data = JArray.Load(reader);
            if(data[0].Type == JTokenType.Array)
            {
                var result = new List<BitfinexRawOrderBookEntry>();
                foreach(var entry in data)                
                    result.Add(ParseEntry((JArray)entry));                
                return result;
            }
            else
            {
                return ParseEntry(data);
            }           
        }

        private static BitfinexRawOrderBookEntry ParseEntry(JArray data)
        {
            JToken price;
            JToken? period;
            JToken orderId;
            JToken quantity;
            if (data.Count == 3)
            {
                orderId = data[0];
                price = data[1];
                quantity = data[2];
                period = null;
            }
            else
            {
                orderId = data[0];
                period = data[1];
                price = data[2];
                quantity = data[3];
            }
            var result = new BitfinexRawOrderBookEntry()
            {
                OrderId = (int)orderId,
                Price = (decimal)price,
                Period = (decimal?)period,
                Quantity = (decimal)quantity,
            };

            return result;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
