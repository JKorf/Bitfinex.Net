using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Converters.MessageParsing.DynamicConverters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients.SpotApi
{
    internal class BitfinexSocketClientSpotApiMessageIdentifier2 : PreloadJsonConverter
    {
        public override JsonSerializerOptions Options { get; } = SerializerOptions.WithConverters(BitfinexExchange._serializerContext);

        protected override string? GetMessageIdentifier(JsonDocument document)
        {
            var type = document.RootElement.ValueKind;
            if (type == JsonValueKind.Array)
            {
                var id = document.RootElement[0].GetInt32().ToString();
                if (id.Equals("0", StringComparison.Ordinal))
                {
                    var topic = document.RootElement[1].GetString();
                    if (topic!.Equals("miu", StringComparison.Ordinal))
                    {
                        var marginType = document.RootElement[2][0].GetString();
                        return id + topic + marginType;
                    }

                    return id + topic;
                }
                var nodeType1 = document.RootElement[1].ValueKind;
                if (nodeType1 != JsonValueKind.Array)
                {
                    var nodeValue1 = document.RootElement[1].GetString();
                    if (nodeValue1!.Equals("hb") || nodeValue1.Equals("cs"))
                        return id + nodeValue1;

                    var nodeTypeData = document.RootElement[2][0].ValueKind;
                    return nodeTypeData == JsonValueKind.Array ? id + "array" : id + "single";
                }
                else
                {
                    var nodeTypeData = document.RootElement[1][0].ValueKind;
                    return nodeTypeData == JsonValueKind.Array ? id + "array" : id + "single";
                }
            }

            var evnt = document.RootElement.GetProperty("event").GetString();
            if (string.Equals(evnt, "info", StringComparison.Ordinal))
                return "info";

            var channel = StringOrEmpty(document, "channel");
            var symbol = StringOrEmpty(document, "symbol");
            var prec = StringOrEmpty(document, "prec");
            var freq = StringOrEmpty(document, "freq");
            var len = StringOrEmpty(document, "len");
            var key = StringOrEmpty(document, "key");
            var chanId = string.Equals(evnt, "unsubscribed", StringComparison.Ordinal) ? StringOrEmpty(document, "chanId") : "";
            return chanId + evnt + channel + symbol + prec + freq + len + key;
        }
    }
}
