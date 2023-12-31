using Bitfinex.Net.Objects.Models.Socket;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitfinex.Net.Objects.Sockets.Subscriptions
{
    internal class BitfinexInfoSubscription : SystemSubscription<BitfinexSocketInfo>
    {
        public override List<string> StreamIdentifiers { get; } = new List<string> { "info" };

        public BitfinexInfoSubscription(ILogger logger) : base(logger, false)
        {
        }

        public override Task<CallResult> HandleMessageAsync(SocketConnection connection, DataEvent<ParsedMessage<BitfinexSocketInfo>> message) => Task.FromResult(new CallResult(null));
    }
}
