using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class TimeFrameConverter: JsonConverter
    {
        private readonly bool quotes;

        public TimeFrameConverter()
        {
            quotes = true;
        }

        public TimeFrameConverter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<TimeFrame, string> values = new Dictionary<TimeFrame, string>()
        {
            { TimeFrame.OneMinute, "1m" },
            { TimeFrame.FiveMinute, "5m" },
            { TimeFrame.FiveteenMinute, "15m" },
            { TimeFrame.ThirtyMinute, "30m" },
            { TimeFrame.OneHour, "1h" },
            { TimeFrame.ThreeHour, "3h" },
            { TimeFrame.SixHour, "6h" },
            { TimeFrame.TwelfHour, "12h" },
            { TimeFrame.OneDay, "1D" },
            { TimeFrame.SevenDay, "1D" },
            { TimeFrame.FourteenDay, "1D" },
            { TimeFrame.OneMonth, "1M" },
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(TimeFrame)value]);
            else
                writer.WriteRawValue(values[(TimeFrame)value]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == reader.Value.ToString()).Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeFrame);
        }
    }
}
