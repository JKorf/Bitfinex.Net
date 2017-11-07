using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class FundingTypeConverter: JsonConverter
    {
        private readonly bool quotes;

        public FundingTypeConverter()
        {
            quotes = true;
        }

        public FundingTypeConverter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<FundingType, string> values = new Dictionary<FundingType, string>()
        {
            { FundingType.Lend, "lend" },
            { FundingType.Loan, "loan" },
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(FundingType)value]);
            else
                writer.WriteRawValue(values[(FundingType)value]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == reader.Value.ToString()).Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(FundingType);
        }
    }
}
