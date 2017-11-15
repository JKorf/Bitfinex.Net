using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class SortingConverter : JsonConverter
    {
        private readonly bool quotes;

        public SortingConverter()
        {
            quotes = true;
        }

        public SortingConverter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<Sorting, string> values = new Dictionary<Sorting, string>()
        {
            { Sorting.NewFirst, "-1" },
            { Sorting.OldFirst, "1" }
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(Sorting)value]);
            else
                writer.WriteRawValue(values[(Sorting)value]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == reader.Value.ToString()).Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Sorting);
        }
    }
}
