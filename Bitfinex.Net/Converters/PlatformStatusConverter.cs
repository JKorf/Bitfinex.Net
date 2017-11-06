using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class PlatformStatusConverter: JsonConverter
    {
        private readonly bool quotes;

        public PlatformStatusConverter()
        {
            quotes = true;
        }

        public PlatformStatusConverter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<PlatformStatus, string> values = new Dictionary<PlatformStatus, string>()
        {
            { PlatformStatus.Maintenance, "0" },
            { PlatformStatus.Operative, "1" },
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(PlatformStatus)value]);
            else
                writer.WriteRawValue(values[(PlatformStatus)value]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == reader.Value.ToString()).Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PlatformStatus);
        }
    }
}
