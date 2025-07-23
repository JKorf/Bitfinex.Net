using Bitfinex.Net.Objects.Internal;
using CryptoExchange.Net.Sockets;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Sockets.Queries
{
    internal class BitfinexUnsubQuery : Query<BitfinexResponse>
    {
        public BitfinexUnsubQuery(int channelId) : base(new BitfinexUnsubscribeRequest(channelId), false, 1)
        {
            MessageMatcher = MessageMatcher.Create<BitfinexResponse>(channelId.ToString() + "unsubscribed");
        }
    }
}
