using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default;
using System;

namespace Bitfinex.Net.Objects.Sockets.Queries
{
    internal class BitfinexSubQuery : Query<BitfinexResponse>
    {
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

            MessageRouter = MessageRouter.CreateWithoutTopicFilter<BitfinexResponse>(
                [evnt + channel + symbol + precision + frequency + length + key,
                "error" + channel + symbol + precision + frequency + length + key],
                HandleMessage);
        }

        public CallResult<BitfinexResponse> HandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexResponse message)
        {
            if (string.Equals(message.Event, "error", StringComparison.Ordinal))
            {
                // Additional check for "dup" which means the subscription is already active
                if (!message.Message.Equals("subscribe: dup", StringComparison.Ordinal))
                    return new CallResult<BitfinexResponse>(new ServerError(ErrorInfo.Unknown with { Message = message.Message! }), originalData);
            }

            return new CallResult<BitfinexResponse>(message, originalData, null);
        }
    }
}
