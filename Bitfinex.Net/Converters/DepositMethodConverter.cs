using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class DepositMethodConverter: JsonConverter
    {
        private readonly bool quotes;

        public DepositMethodConverter()
        {
            quotes = true;
        }

        public DepositMethodConverter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<DepositMethod, string> values = new Dictionary<DepositMethod, string>()
        {
            { DepositMethod.Bitcoin, "bitcoin" },
            { DepositMethod.Litecoin, "litecoin" },
            { DepositMethod.Ethereum, "ethereum" },
            { DepositMethod.Tether, "tetheruso" },
            { DepositMethod.EthereumClassic, "ethereumc" },
            { DepositMethod.ZCash, "zcash" },
            { DepositMethod.Monero, "monero" },
            { DepositMethod.Iota, "iota" },
            { DepositMethod.BCash, "bcash" },
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(DepositMethod)value]);
            else
                writer.WriteRawValue(values[(DepositMethod)value]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == reader.Value.ToString()).Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DepositMethod);
        }
    }
}
