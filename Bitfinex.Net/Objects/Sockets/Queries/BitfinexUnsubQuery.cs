using Bitfinex.Net.Objects.Internal;
using CryptoExchange.Net.Sockets;

namespace Bitfinex.Net.Objects.Sockets.Queries
{
    internal class BitfinexUnsubQuery : Query<BitfinexResponse>
    {
        public BitfinexUnsubQuery(int channelId) : base(new BitfinexUnsubscribeRequest(channelId), false, 1)
        {
            MessageRouter = MessageRouter.CreateWithoutHandler<BitfinexResponse>(channelId.ToString() + "unsubscribed");
            MessageMatcher = MessageMatcher.Create<BitfinexResponse>(channelId.ToString() + "unsubscribed");
        }
    }
}
