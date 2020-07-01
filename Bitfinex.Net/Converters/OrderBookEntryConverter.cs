using Bitfinex.Net.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Bitfinex.Net.Converters
{
    internal class OrderBookEntryConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var data = JArray.Load(reader);
            if(data[0].Type == JTokenType.Array)
            {
                var result = new List<BitfinexOrderBookEntry>();
                foreach(var entry in data)                
                    result.Add(ParseEntry((JArray)entry));                
                return result;
            }
            else
            {
                return ParseEntry(data);
            }           
        }

        private BitfinexOrderBookEntry ParseEntry(JArray data)
        {
            JToken price;
            JToken count;
            JToken quantity;
            if (data.Count == 3)
            {
                price = data[0];
                count = data[1];
                quantity = data[2];
            }
            else
            {
                price = data[0];
                count = data[2];
                quantity = data[3];
            }
            return new BitfinexOrderBookEntry()
            {
                Count = (int)count,
                Price = (decimal)price,
                Quantity = (decimal)quantity,
                RawPrice = price.Value<string>(),
                RawQuantity = quantity.Value<string>()
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
