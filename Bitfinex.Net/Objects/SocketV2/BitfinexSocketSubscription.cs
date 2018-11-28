using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bitfinex.Net.Objects.SocketV2
{
    internal class BitfinexSocketSubscription: SocketSubscription
    {
        public BitfinexEventType[] EventTypes { get; set; }

        public BitfinexSocketSubscription(IWebsocket socket) : base(socket) { }
    }
}
