using Bitfinex.Net.Objects.Internal;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default;
using System;

namespace Bitfinex.Net.Objects.Sockets.Queries
{
    internal class BitfinexAuthQuery : Query<BitfinexResponse>
    {
        private readonly SocketApiClient _client;

        public BitfinexAuthQuery(SocketApiClient client, BitfinexAuthentication authRequest) : base(authRequest, true)
        {
            _client = client;
            MessageMatcher = MessageMatcher.Create<BitfinexResponse>("auth", HandleMessage);
            MessageRouter = MessageRouter.CreateWithoutTopicFilter<BitfinexResponse>("auth", HandleMessage);
        }

        public CallResult<BitfinexResponse> HandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexResponse message)
        {
            if (message.Status != "OK")
                return new CallResult<BitfinexResponse>(new ServerError(message.Code!.Value.ToString(), _client.GetErrorInfo(message.Code!.Value, message.Message!)));

            return new CallResult<BitfinexResponse>(message, originalData, null);
        }
    }
}
