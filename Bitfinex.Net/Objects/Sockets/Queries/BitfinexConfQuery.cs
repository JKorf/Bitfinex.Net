using Bitfinex.Net.Objects.Internal;
using CryptoExchange.Net.Sockets;

namespace Bitfinex.Net.Objects.Sockets.Queries
{
    internal class BitfinexConfQuery : Query<object>
    {
        public BitfinexConfQuery(int flags) : base(new BitfinexSocketConfig { Event = "conf", Flags = flags }, false, 1)
        {
            MessageRouter = MessageRouter.CreateWithoutHandler<object>("conf");
        }

    }
}
