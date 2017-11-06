using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class StatSectionConverter: JsonConverter
    {
        private readonly bool quotes;

        public StatSectionConverter()
        {
            quotes = true;
        }

        public StatSectionConverter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<StatSection, string> values = new Dictionary<StatSection, string>()
        {
            { StatSection.History, "hist" },
            { StatSection.Last, "last" }
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(StatSection)value]);
            else
                writer.WriteRawValue(values[(StatSection)value]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == reader.Value.ToString()).Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(StatSection);
        }
    }
}
