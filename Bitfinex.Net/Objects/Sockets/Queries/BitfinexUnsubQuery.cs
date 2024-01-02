using Bitfinex.Net.Objects.Internal;
using CryptoExchange.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bitfinex.Net.Objects.Sockets.Queries
{
    internal class BitfinexUnsubQuery : Query<BitfinexSubscribeResponse>
    {
        public override List<string> StreamIdentifiers { get; }

        public BitfinexUnsubQuery(int channelId) : base(new BitfinexUnsubscribeRequest(channelId), false, 1)
        {
            StreamIdentifiers = new List<string> { channelId.ToString() + "unsubscribed" };
        }
    }
}
