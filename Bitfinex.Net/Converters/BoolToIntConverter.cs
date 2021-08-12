using System;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    internal class BoolToIntConverter : JsonConverter
    {
        private readonly bool quotes;
        private readonly bool asInt;

        public BoolToIntConverter()
        {
            quotes = true;
            asInt = false;
        }

        public BoolToIntConverter(bool useQuotes = true, bool writeAsInt = false)
        {
            quotes = useQuotes;
            asInt = writeAsInt;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (asInt)
            {
                writer.WriteValue((bool?)value == true ? 1 : 0);
            }
            else
            {
                if (quotes)
                    writer.WriteValue((bool?)value == true ? "1" : "0");
                else
                    writer.WriteRawValue((bool?)value == true ? "1" : "0");
            }
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;

            return reader.Value.ToString() == "1";
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool) || objectType == typeof(bool?);
        }
    }
}
