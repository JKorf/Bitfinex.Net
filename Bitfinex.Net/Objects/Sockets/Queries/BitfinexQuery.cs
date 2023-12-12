using CryptoExchange.Net.Sockets;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Sockets.Queries
{
    internal class BitfinexQuery : Query<BitfinexResponse>
    {
        public override List<string> Identifiers { get; }

        public BitfinexQuery(string evnt, string channel, string? symbol, bool authenticated, int weight = 1) : base(new BitfinexRequest { Channel = channel, Event = evnt, Symbol = symbol }, authenticated, weight)
        {
            if (evnt == "subscribe" || evnt == "unsubscribe")
                evnt += "d";

            Identifiers = new List<string> { evnt + channel + symbol };
        }

    }
}
