using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitfinex.Net.Clients.ExchangeApi;
using Bitfinex.Net.Objects.Options;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets.Default;
using CryptoExchange.Net.Sockets.Default.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Bitfinex.Net.UnitTests
{
    /// <summary>
    /// Messages on the authenticated channel carry two sequence numbers:
    /// [0, TYPE, PAYLOAD, MESSAGE_SEQUENCE, ACCOUNT_SEQUENCE, TIMESTAMP]
    /// Only MESSAGE_SEQUENCE increases by one per message. Reading ACCOUNT_SEQUENCE instead makes the next
    /// message look out of sequence and forces a reconnect. These frames are verbatim captures from a live account.
    /// </summary>
    [TestFixture]
    public class BitfinexUnhandledMessageSequenceTests
    {
        /// <summary>Authenticated notification, e.g. the one Bitfinex sends for a wallet transfer.</summary>
        private const string AuthNotification =
            "[0,\"n\",[1786327545417,\"wallet_transfer\",null,null,null,null,\"SUCCESS\",\"100.0 Tether USDt transferred from Margin to Exchange\"],143,15214,1786327545418]";

        /// <summary>Authenticated wallet update - the message that arrived right after the notification.</summary>
        private const string AuthWalletUpdate =
            "[0,\"wu\",[\"margin\",\"UST\",28435.19117142,0,null,null,null],144,15215,1786327545453]";

        /// <summary>Authenticated heartbeat - no payload, so the sequence sits one position earlier.</summary>
        private const string AuthHeartbeat = "[0,\"hb\",40,1786327081453]";

        /// <summary>Public order book frame - single sequence number in the second to last field.</summary>
        private const string PublicBookFrame = "[141234,[[64928,1,0.5]],9912,1786327081453]";

        [TestCase(AuthNotification, 143, TestName = "Auth notification reads MESSAGE_SEQUENCE, not ACCOUNT_SEQUENCE")]
        [TestCase(AuthWalletUpdate, 144, TestName = "Auth wallet update reads MESSAGE_SEQUENCE")]
        [TestCase(AuthHeartbeat, 40, TestName = "Auth heartbeat reads its sequence one position earlier")]
        [TestCase(PublicBookFrame, 9912, TestName = "Public frame is unchanged - second to last field")]
        public void HandleUnhandledMessage_RecordsTheCorrectSequenceNumber(string frame, long expectedSequence)
        {
            var logs = new List<string>();
            var api = CreateApi(logs);
            var connection = CreateConnection(api, logs);

            var handled = api.InvokeHandleUnhandledMessage(connection, "irrelevant", Encoding.UTF8.GetBytes(frame));

            Assert.That(handled, Is.True, "the message should be reported as handled");

            var line = logs.SingleOrDefault(l => l.Contains("Setting connection sequence number to"));
            Assert.That(line, Is.Not.Null, "expected the sequence number to be recorded");
            Assert.That(line, Does.Contain($"to {expectedSequence} "),
                $"wrong field read from {frame}");
        }

        /// <summary>
        /// The regression itself. The connection is seeded with 142 - the value the last ROUTED message recorded,
        /// exactly as it stood live - then the unrouted notification (143) arrives. Reading ACCOUNT_SEQUENCE here
        /// records 15214 against a last-seen 142, which is the gap that forced the reconnect.
        /// </summary>
        [Test]
        public void UnroutedAuthMessage_DoesNotBreakSequenceFromARoutedMessage()
        {
            var logs = new List<string>();
            var api = CreateApi(logs);
            var connection = CreateConnection(api, logs);

            //the routed wu that preceded the transfer notification live
            connection.UpdateSequenceNumber(142);

            api.InvokeHandleUnhandledMessage(connection, "0n", Encoding.UTF8.GetBytes(AuthNotification));
            api.InvokeHandleUnhandledMessage(connection, "0wu", Encoding.UTF8.GetBytes(AuthWalletUpdate));

            Assert.That(logs.Any(l => l.Contains("not in sequence")), Is.False,
                "an unrouted authenticated message must not be treated as a sequence gap");
        }

        private static TestableSocketApi CreateApi(List<string> logs)
        {
            var loggerFactory = new Mock<ILoggerFactory>();
            loggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(new CapturingLogger(logs));
            return new TestableSocketApi(loggerFactory.Object, new BitfinexSocketOptions());
        }

        /// <summary>The connection logs the "not in sequence" warning itself, so it shares the capture list.</summary>
        private static SocketConnection CreateConnection(TestableSocketApi api, List<string> logs)
        {
            var socket = new Mock<IWebsocket>();
            var factory = new Mock<IWebsocketFactory>();
            factory.Setup(f => f.CreateWebsocket(It.IsAny<ILogger>(), It.IsAny<SocketConnection>(), It.IsAny<WebSocketParameters>()))
                   .Returns(socket.Object);

            var parameters = new WebSocketParameters(new Uri("wss://api.bitfinex.com/ws/2"), ReconnectPolicy.FixedDelay);
            return new SocketConnection(new CapturingLogger(logs), factory.Object, parameters, api);
        }

        /// <summary>Exposes the protected handler. The api client is internal, which the test assembly can see.</summary>
        private class TestableSocketApi : BitfinexSocketClientExchangeApi
        {
            public TestableSocketApi(ILoggerFactory loggerFactory, BitfinexSocketOptions options)
                : base(loggerFactory, options)
            {
            }

            public bool InvokeHandleUnhandledMessage(SocketConnection connection, string typeIdentifier, byte[] data)
                => HandleUnhandledMessage(connection, typeIdentifier, data);
        }

        private class CapturingLogger : ILogger
        {
            private readonly List<string> _messages;

            public CapturingLogger(List<string> messages) => _messages = messages;

            public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
                Func<TState, Exception?, string> formatter)
                => _messages.Add(formatter(state, exception));

            private sealed class NullScope : IDisposable
            {
                public static readonly NullScope Instance = new();
                public void Dispose() { }
            }
        }
    }
}
