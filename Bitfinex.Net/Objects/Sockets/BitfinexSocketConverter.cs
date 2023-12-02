using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects.Sockets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bitfinex.Net.Objects.Sockets
{
    internal class BitfinexSocketConverter : SocketConverter
    {
        public override MessageInterpreterPipeline InterpreterPipeline { get; } = new MessageInterpreterPipeline
        {
            GetIdentity = GetIdentity
        };

        private static string GetIdentity(IMessageAccessor accessor)
        {
            if (!accessor.IsObject(null))
            {
                var channelId = accessor.GetArrayIntValue(null, 0);
                if (accessor.GetArrayStringValue(null, 1) == "hb")
                    return "hb";

                return channelId.ToString();
            }

            var evnt = accessor.GetStringValue("event");
            var channel = accessor.GetStringValue("channel");
            var symbol = accessor.GetStringValue("symbol");
            return evnt + channel + symbol;
        }
    }
}
