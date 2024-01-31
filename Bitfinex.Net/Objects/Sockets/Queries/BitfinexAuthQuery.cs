using Bitfinex.Net.Objects.Internal;
using CryptoExchange.Net.Sockets;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Sockets.Queries
{
    internal class BitfinexAuthQuery : Query<BitfinexResponse>
    {
        public override HashSet<string> ListenerIdentifiers { get; set; } = new HashSet<string> { "auth" };

        public BitfinexAuthQuery(BitfinexAuthentication authRequest) : base(authRequest, true)
        {
        }
    }
}
