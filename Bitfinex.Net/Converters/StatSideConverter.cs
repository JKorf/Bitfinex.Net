using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class StatSideConverter: JsonConverter
    {
        private readonly bool quotes;

        public StatSideConverter()
        {
            quotes = true;
        }

        public StatSideConverter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<StatSide, string> values = new Dictionary<StatSide, string>()
        {
            { StatSide.Long, "long" },
            { StatSide.Short, "short" }
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(StatSide)value]);
            else
                writer.WriteRawValue(values[(StatSide)value]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == reader.Value.ToString()).Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(StatSide);
        }
    }
}
