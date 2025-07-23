using Bitfinex.Net.Objects.Models.Socket;
using Bitfinex.Net.Objects.Sockets.Queries;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Sockets.Subscriptions
{
    internal class BitfinexInfoSubscription : SystemSubscription
    {
        private readonly bool _bulkUpdates;

        public BitfinexInfoSubscription(ILogger logger, bool bulkUpdates) : base(logger, false)
        {
            _bulkUpdates = bulkUpdates;

            MessageMatcher = MessageMatcher.Create<BitfinexSocketInfo>("info", HandleMessage);
        }

        public CallResult HandleMessage(SocketConnection connection, DataEvent<BitfinexSocketInfo> message)
        {
            if (message.Data.Code == null)
            {
                // welcome event, send a config message
                _ = connection.SendAndWaitQueryAsync(new BitfinexConfQuery(
                    131072 + // Send checksum messages
                    (_bulkUpdates ? 536870912 : 0) // Bulk updates
                    ));
                return CallResult.SuccessResult;
            }

            var code = message.Data.Code;
            switch (code)
            {
                case 20051:
                    _logger.Log(LogLevel.Information, $"[Sckt {connection.SocketId}] code {code} received, reconnecting socket");
                    connection.PausedActivity = true; // Prevent new operations to be send
                    _ = connection.TriggerReconnectAsync();
                    break;
                case 20060:
                    _logger.Log(LogLevel.Information, $"[Sckt {connection.SocketId}] code {code} received, entering maintenance mode");
                    connection.PausedActivity = true;
                    break;
                case 20061:
                    _logger.Log(LogLevel.Information, $"[Sckt  {connection.SocketId} ] code {code} received, leaving maintenance mode. Reconnecting/Resubscribing socket.");
                    _ = connection.TriggerReconnectAsync(); // Closing it via socket will automatically reconnect
                    break;
                default:
                    _logger.Log(LogLevel.Warning, $"[Sckt {connection.SocketId}] unknown info code received: {code}");
                    break;
            }

            return CallResult.SuccessResult;
        }
    }
}
