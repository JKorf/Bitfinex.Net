using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class TimestampSecondsConverter : JsonConverter
    {
        private readonly bool quotes;

        public TimestampSecondsConverter()
        {
            quotes = true;
        }

        public TimestampSecondsConverter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var t = Convert.ToInt64(Math.Round(double.Parse(reader.Value.ToString()) * 1000));
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(t);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(Math.Round(((DateTime)value - new DateTime(1970, 1, 1)).TotalMilliseconds / 1000));
            else
                writer.WriteRawValue(Math.Round(((DateTime)value - new DateTime(1970, 1, 1)).TotalMilliseconds / 1000).ToString(CultureInfo.InvariantCulture));
        }
    }
}
