using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net.Converters
{
    public class OrderType2Converter: JsonConverter
    {
        private readonly bool quotes;

        public OrderType2Converter()
        {
            quotes = true;
        }

        public OrderType2Converter(bool useQuotes = true)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<OrderType2, string> values = new Dictionary<OrderType2, string>()
        {
            { OrderType2.Market, "market" },
            { OrderType2.Limit, "limit" },
            { OrderType2.Stop, "stop" },
            { OrderType2.TrailingStop, "trailing-stop" },
            { OrderType2.FillOrKill, "fill-or-kill" },
            { OrderType2.ExchangeMarket, "exchange market" },
            { OrderType2.ExchangeLimit, "exchange limit" },
            { OrderType2.ExchangeStop, "exchange stop" },
            { OrderType2.ExchangeTrailingStop, "exchange trailing-stop" },
            { OrderType2.ExchangeFillOrKill, "exchange fill-or-kill" },
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(OrderType2)value]);
            else
                writer.WriteRawValue(values[(OrderType2)value]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == reader.Value.ToString()).Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(OrderType2);
        }
    }
}
