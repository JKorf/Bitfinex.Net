using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace Bitfinex.Net.Objects.Sockets.Subscriptions
{
    internal class BitfinexHeartbeatSubscription : SystemSubscription
    {
        public BitfinexHeartbeatSubscription(ILogger logger) : base(logger, false)
        {
            MessageMatcher = MessageMatcher.Create<string>("hb");
        }
    }
}
