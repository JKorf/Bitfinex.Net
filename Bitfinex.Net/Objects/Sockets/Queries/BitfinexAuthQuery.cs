using Bitfinex.Net.Objects.Internal;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitfinex.Net.Objects.Sockets.Queries
{
    internal class BitfinexAuthQuery : Query<BitfinexResponse>
    {
        public override HashSet<string> ListenerIdentifiers { get; set; } = new HashSet<string> { "auth" };

        public BitfinexAuthQuery(BitfinexAuthentication authRequest) : base(authRequest, true)
        {
        }

        public override Task<CallResult<BitfinexResponse>> HandleMessageAsync(SocketConnection connection, DataEvent<BitfinexResponse> message)
        {
            if (message.Data.Message != "OK")
                return Task.FromResult(new CallResult<BitfinexResponse>(new ServerError(message.Data.Code!.Value, message.Data.Message!)));

            return Task.FromResult(new CallResult<BitfinexResponse>(message.Data, message.OriginalData, null));
        }
    }
}
