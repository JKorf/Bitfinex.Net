using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class SplitMergeConverter: JsonConverter
    {
        private readonly bool quotes;

        public SplitMergeConverter()
        {
            quotes = true;
        }

        public SplitMergeConverter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<SplitMerge, string> values = new Dictionary<SplitMerge, string>()
        {
            { SplitMerge.Split, "1" },
            { SplitMerge.Merge, "-1" }
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(SplitMerge)value]);
            else
                writer.WriteRawValue(values[(SplitMerge)value]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == reader.Value.ToString()).Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SplitMerge);
        }
    }
}
