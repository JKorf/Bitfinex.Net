using Bitfinex.Net.Objects.Models.Socket;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitfinex.Net.Objects.Sockets.Subscriptions
{
    internal class BitfinexInfoSubscription : SystemSubscription<BitfinexSocketInfo>
    {
        public override HashSet<string> ListenerIdentifiers { get; set; } = new HashSet<string> { "info" };

        public BitfinexInfoSubscription(ILogger logger) : base(logger, false)
        {
        }

        public override CallResult HandleMessage(SocketConnection connection, DataEvent<BitfinexSocketInfo> message)
        {
            if (message.Data.Code == null)
            {
                // welcome event, send a config message for receiving checsum updates for order book subscriptions
                _ = connection.SendAndWaitQueryAsync(new BitfinexConfQuery(131072));
                return new CallResult(null);
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

            return new CallResult(null);
        }
    }
}
