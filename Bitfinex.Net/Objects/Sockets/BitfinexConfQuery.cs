using Bitfinex.Net.Objects.Internal;
using CryptoExchange.Net.Sockets;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Sockets
{
    internal class BitfinexConfQuery : Query<object>
    {
        public override HashSet<string> ListenerIdentifiers { get; set; } = new HashSet<string> { "conf" };

        public BitfinexConfQuery(int flags) : base(new BitfinexSocketConfig { Event = "conf", Flags = flags }, false, 1)
        {
        }

    }
}
