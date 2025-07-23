using Bitfinex.Net.Objects.Internal;
using CryptoExchange.Net.Sockets;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Sockets.Queries
{
    internal class BitfinexConfQuery : Query<object>
    {
        public BitfinexConfQuery(int flags) : base(new BitfinexSocketConfig { Event = "conf", Flags = flags }, false, 1)
        {
            MessageMatcher = MessageMatcher.Create<object>("conf");
        }

    }
}
