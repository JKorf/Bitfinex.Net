using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class DecimalAsStringConverter: JsonConverter
    {
        private readonly bool quotes;

        public DecimalAsStringConverter()
        {
            quotes = true;
        }

        public DecimalAsStringConverter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                return;

            if (quotes)
                writer.WriteValue(((decimal)value).ToString(CultureInfo.InvariantCulture));
            else
                writer.WriteRawValue(((decimal)value).ToString(CultureInfo.InvariantCulture));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal);
        }
    }
}
