using Bitfinex.Net.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Bitfinex.Net.Converters
{
    internal class OrderBookEntryConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
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

        private static BitfinexOrderBookEntry ParseEntry(JArray data)
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
            var result = new BitfinexOrderBookEntry()
            {
                Count = (int)count,
                Price = (decimal)price,
                Quantity = (decimal)quantity,
            };

            result.RawPrice = ((JValue)price).ToString("F8", CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
            var withExp = ((JValue)price).ToString(CultureInfo.InvariantCulture);
            if (withExp.Contains("e-7") || withExp.Contains("e-8"))
                result.RawQuantity = withExp;
            else
                result.RawQuantity = ((JValue)quantity).ToString("F8", CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');

            return result;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
