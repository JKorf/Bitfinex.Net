//using Bitfinex.Net.Objects.Models;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Text;
//using System.Text.Json;
//using System.Text.Json.Serialization;

//namespace Bitfinex.Net.Converters
//{
//    internal class OrderBookEntryConverter2 : JsonConverter<BitfinexOrderBookFundingEntry>
//    {
//        public override BitfinexOrderBookFundingEntry? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//        {
//            var doc = JsonDocument.ParseValue(ref reader);
//            var arrLength = doc.RootElement.GetArrayLength();

//#warning this now only deserializes single entries, not array of entries. Check if needed

//            var price = doc.RootElement[0].GetRawText();
//            var period = doc.RootElement[1].GetInt32();
//            var count = doc.RootElement[2].GetInt32();
//            var quantity = doc.RootElement[3].GetRawText();
            
//            var result = new BitfinexOrderBookFundingEntry()
//            {
//                Count = count,
//                Period = period,
//                Price = decimal.Parse(price, NumberStyles.Float, CultureInfo.InvariantCulture),
//                Quantity = decimal.Parse(quantity, NumberStyles.Float, CultureInfo.InvariantCulture),
//            };

//            if (price.Contains("E-07") || price.Contains("E-08"))
//                result.RawPrice = price.Replace('E', 'e').Replace("-07", "-7").Replace("-08", "-8");
//            else
//                result.RawPrice = price.TrimEnd('0').TrimEnd('.');


//            if (quantity.Contains("E-07") || quantity.Contains("E-08"))
//                result.RawQuantity = quantity.Replace('E', 'e').Replace("-07", "-7").Replace("-08", "-8");
//            else
//                result.RawQuantity = quantity.TrimEnd('0').TrimEnd('.');

//            return result;
//        }

//        public override void Write(Utf8JsonWriter writer, BitfinexOrderBookFundingEntry value, JsonSerializerOptions options)
//        {
//            writer.WriteStartArray();

//            writer.WriteRawValue(value.RawPrice);
//            writer.WriteNumberValue(value.Period);
//            writer.WriteNumberValue(value.Count);
//            writer.WriteRawValue(value.RawQuantity);

//            writer.WriteEndArray();
//        }
//    }
//}
