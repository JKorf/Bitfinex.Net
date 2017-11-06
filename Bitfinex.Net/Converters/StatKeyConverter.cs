using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class StatKeyConverter: JsonConverter
    {
        private readonly bool quotes;

        public StatKeyConverter()
        {
            quotes = true;
        }

        public StatKeyConverter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<StatKey, string> values = new Dictionary<StatKey, string>()
        {
            { StatKey.ActiveFundingInPositions, "credits.size" },
            { StatKey.ActiveFundingInPositionsPerTradingSymbol, "credits.size.sym" },
            { StatKey.TotalActiveFunding , "funding.size" },
            { StatKey.TotalOpenPosition , "pos.size" },
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(StatKey)value]);
            else
                writer.WriteRawValue(values[(StatKey)value]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == reader.Value.ToString()).Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(StatKey);
        }
    }
}
