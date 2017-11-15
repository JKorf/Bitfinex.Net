using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class WithdrawalDespositTypeConverter: JsonConverter
    {
        private readonly bool quotes;

        public WithdrawalDespositTypeConverter()
        {
            quotes = true;
        }

        public WithdrawalDespositTypeConverter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<WithdrawalDespositType, string> values = new Dictionary<WithdrawalDespositType, string>()
        {
            { WithdrawalDespositType.Deposit, "DEPOSIT" },
            { WithdrawalDespositType.Withdrawal, "WITHDRAWAL" },
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(WithdrawalDespositType)value]);
            else
                writer.WriteRawValue(values[(WithdrawalDespositType)value]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == reader.Value.ToString()).Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(WithdrawalDespositType);
        }
    }
}
