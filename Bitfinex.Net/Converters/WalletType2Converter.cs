using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class WalletType2Converter: JsonConverter
    {
        private readonly bool quotes;

        public WalletType2Converter()
        {
            quotes = true;
        }

        public WalletType2Converter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<WalletType2, string> values = new Dictionary<WalletType2, string>()
        {
            { WalletType2.Deposit, "deposit" },
            { WalletType2.Exchange, "exchange" },
            { WalletType2.Trading, "trading" },
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(WalletType2)value]);
            else
                writer.WriteRawValue(values[(WalletType2)value]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == reader.Value.ToString()).Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(WalletType2);
        }
    }
}
