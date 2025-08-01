﻿using Bitfinex.Net.Objects.Internal;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Sockets.Queries
{
    internal class BitfinexAuthQuery : Query<BitfinexResponse>
    {
        public BitfinexAuthQuery(BitfinexAuthentication authRequest) : base(authRequest, true)
        {
            MessageMatcher = MessageMatcher.Create<BitfinexResponse>("auth", HandleMessage);
        }

        public CallResult<BitfinexResponse> HandleMessage(SocketConnection connection, DataEvent<BitfinexResponse> message)
        {
            if (message.Data.Status != "OK")
                return new CallResult<BitfinexResponse>(new ServerError(message.Data.Code!.Value, message.Data.Message!));

            return new CallResult<BitfinexResponse>(message.Data, message.OriginalData, null);
        }
    }
}
