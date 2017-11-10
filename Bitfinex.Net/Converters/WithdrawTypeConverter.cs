using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class WithdrawTypeConverter: JsonConverter
    {
        private readonly bool quotes;

        public WithdrawTypeConverter()
        {
            quotes = true;
        }

        public WithdrawTypeConverter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<WithdrawType, string> values = new Dictionary<WithdrawType, string>()
        {
            { WithdrawType.Bitcoin, "bitcoin" },
            { WithdrawType.Litecoin, "litecoin" },
            { WithdrawType.Ethereum, "ethereum" },
            { WithdrawType.EthereumClassic, "ethereumc" },
            { WithdrawType.MasterCoin, "mastercoin" },
            { WithdrawType.ZCash, "zcash" },
            { WithdrawType.Monero, "monero" },
            { WithdrawType.Wire, "wire" },
            { WithdrawType.Dash, "dash" },
            { WithdrawType.Ripple, "ripple" },
            { WithdrawType.EOS, "eos" },
            { WithdrawType.Neo, "neo" },
            { WithdrawType.Aventus, "aventus" },
            { WithdrawType.QTUM, "qtum" },
            { WithdrawType.Eidoo, "eidoo" },
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(WithdrawType)value]);
            else
                writer.WriteRawValue(values[(WithdrawType)value]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value.ToLower() == reader.Value.ToString().ToLower()).Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(WithdrawType);
        }
    }
}
