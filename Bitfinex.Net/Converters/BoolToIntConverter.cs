using System;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class BoolToIntConverter : JsonConverter
    {
        private readonly bool quotes;

        public BoolToIntConverter()
        {
            quotes = true;
        }

        public BoolToIntConverter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue((bool)value ? "1": "0");
            else
                writer.WriteRawValue((bool)value ? "1" : "0");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value.ToString() == "1";
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }
    }
}
