using Bitfinex.Net.Objects.Internal;
using CryptoExchange.Net.Sockets;
using System.Collections.Generic;
using System.Globalization;

namespace Bitfinex.Net.Objects.Sockets.Queries
{
    internal class BitfinexAuthQuery : Query<BitfinexResponse>
    {
        public override List<string> StreamIdentifiers { get; } = new List<string> { "auth" };

        public BitfinexAuthQuery(BitfinexAuthentication authRequest) : base(authRequest, true)
        {
        }
    }
}
