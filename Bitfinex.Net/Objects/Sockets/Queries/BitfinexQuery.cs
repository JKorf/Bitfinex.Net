using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Models.Socket;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using System;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Sockets.Queries
{
    internal class BitfinexQuery<T, U> : Query<T> where T : BitfinexSocketEvent<U> where U : BitfinexNotification
    {
        public BitfinexQuery(BitfinexSocketQuery request) : base(request, true, 1)
        {
            MessageMatcher = MessageMatcher.Create<T>("0n", HandleMessage);
        }

        public CallResult<T> HandleMessage(SocketConnection connection, DataEvent<T> message)
        {
            if (message.Data.Data.Result != "SUCCESS")
                return new CallResult<T>(new ServerError(ErrorInfo.Unknown with { Message = message.Data.Data.ErrorMessage! }));

            return new CallResult<T>(message.Data);
        }
    }
}
