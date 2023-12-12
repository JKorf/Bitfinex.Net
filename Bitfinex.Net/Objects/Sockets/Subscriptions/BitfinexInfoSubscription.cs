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
    internal class BitfinexInfoSubscription : SystemSubscription
    {
        public override List<string> Identifiers { get; } = new List<string> { "info" };

        public override Type ExpectedMessageType => typeof(BitfinexSocketInfo);

        public BitfinexInfoSubscription(ILogger logger) : base(logger, false)
        {
        }

        public override Task<CallResult> DoHandleMessageAsync(SocketConnection connection, DataEvent<BaseParsedMessage> message) => Task.FromResult(new CallResult(null));
    }
}
