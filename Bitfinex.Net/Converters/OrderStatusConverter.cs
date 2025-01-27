using Bitfinex.Net.Enums;
using System;
using System.Text.Json;

namespace Bitfinex.Net.Converters
{
    internal class OrderStatusConverter : JsonConverter<OrderStatus>
    {
        public override OrderStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var statusStr = reader.GetString();
            if (statusStr == null)
                return OrderStatus.Unknown;

            if (statusStr == "ACTIVE")
                return OrderStatus.Active;
            if (statusStr == "EXECUTED")
                return OrderStatus.Executed;
            if (statusStr == "CANCELED")
                return OrderStatus.Canceled;
            if (statusStr == "FORCED EXECUTED")
                return OrderStatus.ForcefullyExecuted;
            if (statusStr == "PARTIALLY FILLED")
                return OrderStatus.PartiallyFilled;

            if (statusStr.Contains("CANCELED"))
                return OrderStatus.Canceled;

            if (statusStr.Contains("EXECUTED @"))
                return OrderStatus.Executed;

            return OrderStatus.Active;
        }

        public override void Write(Utf8JsonWriter writer, OrderStatus value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(EnumConverter.GetString(value));
        }
    }
}
