using CryptoExchange.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bitfinex.Net.Objects.Sockets.Queries
{
    internal class BitfinexSubQuery : Query<BitfinexResponse>
    {
        public override List<string> StreamIdentifiers { get; }

        public BitfinexSubQuery(string evnt, string channel, string symbol, string precision, string frequency, string length, string key) : base(new BitfinexBookRequest
        {
            Channel = channel,
            Symbol = symbol,
            Event = evnt,
            Frequency = frequency,
            Length = length,
            Precision = precision,
            Key = key
        }, false, 1)
        {
            if (evnt == "subscribe" || evnt == "unsubscribe")
                evnt += "d";

            StreamIdentifiers = new List<string> { evnt + channel + symbol + precision + frequency + length + key };
        }
    }
}
