using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class PositionStatusConverter: JsonConverter
    {
        private readonly bool quotes;

        public PositionStatusConverter()
        {
            quotes = true;
        }

        public PositionStatusConverter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<PositionStatus, string> values = new Dictionary<PositionStatus, string>()
        {
            { PositionStatus.Closed, "CLOSED" },
            { PositionStatus.Active, "ACTIVE" }
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(PositionStatus)value]);
            else
                writer.WriteRawValue(values[(PositionStatus)value]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == reader.Value.ToString()).Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PositionStatus);
        }
    }
}
