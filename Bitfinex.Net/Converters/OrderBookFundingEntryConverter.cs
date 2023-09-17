using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using Bitfinex.Net.Objects.Models;

namespace Bitfinex.Net.Converters
{
    internal class OrderBookFundingEntryConverter : JsonConverter
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
                var result = new List<BitfinexOrderBookFundingEntry>();
                foreach(var entry in data)                
                    result.Add(ParseEntry((JArray)entry));                
                return result;
            }
            else
            {
                return ParseEntry(data);
            }           
        }

        private static BitfinexOrderBookFundingEntry ParseEntry(JArray data)
        {
            JToken price = data[0];
            JToken quantity = data[3];
            var result = new BitfinexOrderBookFundingEntry()
            {
                Price = (decimal)price,
                Period = (int)data[1],
                Count = (int)data[2],
                Quantity = (decimal)quantity,
            };

            var priceWithExp = ((JValue)price).ToString(CultureInfo.InvariantCulture);
            if (priceWithExp.Contains("E-07") || priceWithExp.Contains("E-08"))
                result.RawPrice = priceWithExp.Replace('E', 'e').Replace("-07", "-7").Replace("-08", "-8");
            else
                result.RawPrice = ((JValue)price).ToString("F8", CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');


            var quantityWithExp = ((JValue)quantity).ToString(CultureInfo.InvariantCulture);
            if (quantityWithExp.Contains("E-07") || quantityWithExp.Contains("E-08"))
                result.RawQuantity = quantityWithExp.Replace('E', 'e').Replace("-07", "-7").Replace("-08", "-8");
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
