using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Models.Socket;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default;
using System;

namespace Bitfinex.Net.Objects.Sockets.Queries
{
    internal class BitfinexQuery<T, U> : Query<T> where T : BitfinexSocketEvent<U> where U : BitfinexNotification
    {
        public BitfinexQuery(BitfinexSocketQuery request) : base(request, true, 1)
        {
            MessageMatcher = MessageMatcher.Create<T>("0n", HandleMessage);
            MessageRouter = MessageRouter.CreateWithoutTopicFilter<T>("0n", HandleMessage);
        }

        public CallResult<T> HandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, T message)
        {
            if (message.Data.Result != "SUCCESS")
                return new CallResult<T>(new ServerError(ErrorInfo.Unknown with { Message = message.Data.ErrorMessage! }), originalData);

            return new CallResult<T>(message, originalData, null);
        }
    }
}
