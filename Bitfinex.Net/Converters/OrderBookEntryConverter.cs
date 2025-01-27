using Bitfinex.Net.Objects.Models;
using System;
using System.Globalization;
using System.Text.Json;

namespace Bitfinex.Net.Converters
{
    internal class OrderBookEntryConverter : JsonConverter<BitfinexOrderBookEntry>
    {
        public override BitfinexOrderBookEntry? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var doc = JsonDocument.ParseValue(ref reader);
            var arrLength = doc.RootElement.GetArrayLength();

            string price, quantity;
            int count;
            if (arrLength == 3)
            {
                price = doc.RootElement[0].GetRawText();
                count = doc.RootElement[1].GetInt32();
                quantity = doc.RootElement[2].GetRawText();
            }
            else
            {
                price = doc.RootElement[0].GetRawText();
                count = doc.RootElement[2].GetInt32();
                quantity = doc.RootElement[3].GetRawText();
            }

            var result = new BitfinexOrderBookEntry()
            {
                Count = (int)count,
                Price = decimal.Parse(price, NumberStyles.Float, CultureInfo.InvariantCulture),
                Quantity = decimal.Parse(quantity, NumberStyles.Float, CultureInfo.InvariantCulture),
            };

            if (price.Contains("E-07") || price.Contains("E-08"))
                result.RawPrice = price.Replace('E', 'e').Replace("-07", "-7").Replace("-08", "-8");
            else
                result.RawPrice = price;//.TrimEnd('0').TrimEnd('.');


            if (quantity.Contains("E-07") || quantity.Contains("E-08"))
                result.RawQuantity = quantity.Replace('E', 'e').Replace("-07", "-7").Replace("-08", "-8");
            else
                result.RawQuantity = quantity;//.TrimEnd('0').TrimEnd('.');

            return result;
        }

        public override void Write(Utf8JsonWriter writer, BitfinexOrderBookEntry value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            writer.WriteRawValue(value.RawPrice);
            writer.WriteNumberValue(value.Count);
            writer.WriteRawValue(value.RawQuantity);

            writer.WriteEndArray();
        }
    }
}
