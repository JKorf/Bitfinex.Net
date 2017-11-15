using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class MarginFundingTypeConverter: JsonConverter
    {
        private readonly bool quotes;

        public MarginFundingTypeConverter()
        {
            quotes = true;
        }

        public MarginFundingTypeConverter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<MarginFundingType, string> values = new Dictionary<MarginFundingType, string>()
        {
            { MarginFundingType.Daily, "0" },
            { MarginFundingType.Term, "1" }
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(MarginFundingType)value]);
            else
                writer.WriteRawValue(values[(MarginFundingType)value]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == reader.Value.ToString()).Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MarginFundingType);
        }
    }
}
