using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default;
using Microsoft.Extensions.Logging;

namespace Bitfinex.Net.Objects.Sockets.Subscriptions
{
    internal class BitfinexHeartbeatSubscription : SystemSubscription
    {
        public BitfinexHeartbeatSubscription(ILogger logger) : base(logger, false)
        {
            MessageMatcher = MessageMatcher.Create<string>("hb");
            MessageRouter = MessageRouter.CreateWithoutHandler<string>("hb");
        }
    }
}
