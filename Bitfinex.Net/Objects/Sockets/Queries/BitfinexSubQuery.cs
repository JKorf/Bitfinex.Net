using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using System;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Sockets.Queries
{
    internal class BitfinexSubQuery : Query<BitfinexResponse>
    {
        public override HashSet<string> ListenerIdentifiers { get; set; }

        public BitfinexSubQuery(string evnt, string channel, string? symbol, string? precision, string? frequency, string? length, string? key) : base(new BitfinexBookRequest
        {
            Channel = channel,
            Symbol = symbol,
            Event = evnt,
            Frequency = frequency,
            Length = length,
            Precision = precision,
            Key = key
        }, false, 1)
        {
            if (string.Equals(evnt, "subscribe", StringComparison.Ordinal) || string.Equals(evnt, "unsubscribe", StringComparison.Ordinal))
                evnt += "d";

            ListenerIdentifiers = new HashSet<string> 
            { 
                evnt + channel + symbol + precision + frequency + length + key, 
                "error" + channel + symbol + precision + frequency + length + key 
            };
        }

        public override CallResult<BitfinexResponse> HandleMessage(SocketConnection connection, DataEvent<BitfinexResponse> message)
        {
            if (string.Equals(message.Data.Event, "error", StringComparison.Ordinal))
            {
                // Additional check for "dup" which means the subscription is already active
                if (!message.Data.Message.Equals("subscribe: dup", StringComparison.Ordinal))
                    return new CallResult<BitfinexResponse>(new ServerError(message.Data.Message!));
            }

            return new CallResult<BitfinexResponse>(message.Data);
        }
    }
}
