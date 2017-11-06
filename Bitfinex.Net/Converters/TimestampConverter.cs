using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class TimestampConverter : JsonConverter
    {
        private readonly bool quotes;

        public TimestampConverter()
        {
            quotes = true;
        }

        public TimestampConverter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var t = long.Parse(reader.Value.ToString());
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(t);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(Math.Round(((DateTime)value - new DateTime(1970, 1, 1)).TotalMilliseconds));
            else
                writer.WriteRawValue(Math.Round(((DateTime)value - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString(CultureInfo.InvariantCulture));
        }
    }
}
