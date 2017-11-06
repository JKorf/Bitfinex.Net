using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class WalletTypeConverter: JsonConverter
    {
        private readonly bool quotes;

        public WalletTypeConverter()
        {
            quotes = true;
        }

        public WalletTypeConverter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<WalletType, string> values = new Dictionary<WalletType, string>()
        {
            { WalletType.Exchange, "exchange" },
            { WalletType.Funding, "funding" },
            { WalletType.Margin, "margin" },
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(WalletType)value]);
            else
                writer.WriteRawValue(values[(WalletType)value]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == reader.Value.ToString()).Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(WalletType);
        }
    }
}
