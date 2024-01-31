using Bitfinex.Net.Objects.Internal;
using CryptoExchange.Net.Sockets;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Sockets.Queries
{
    internal class BitfinexUnsubQuery : Query<BitfinexResponse>
    {
        public override HashSet<string> ListenerIdentifiers { get; set; }

        public BitfinexUnsubQuery(int channelId) : base(new BitfinexUnsubscribeRequest(channelId), false, 1)
        {
            ListenerIdentifiers = new HashSet<string> { channelId.ToString() + "unsubscribed" };
        }
    }
}
