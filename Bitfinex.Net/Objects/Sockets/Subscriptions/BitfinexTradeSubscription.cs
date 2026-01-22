using Bitfinex.Net.Clients.SpotApi;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Sockets.Queries;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default;
using Microsoft.Extensions.Logging;
using System;

namespace Bitfinex.Net.Objects.Sockets.Subscriptions
{
    internal class BitfinexTradeSubscription: Subscription
    {
        private BitfinexSocketClientSpotApi _client;

        private string _channel;
        private string _symbol;
        private int _channelId;
        private bool _firstUpdate;
        private Action<DateTime, string?, SocketUpdateType, BitfinexTradeSimple[], long, DateTime> _handler;

        public BitfinexTradeSubscription(ILogger logger,
            BitfinexSocketClientSpotApi client,
            string symbol,
            Action<DateTime, string?, SocketUpdateType, BitfinexTradeSimple[], long, DateTime> handler,
            bool authenticated = false)
            : base(logger, authenticated)
        {
            _client = client;
            _handler = handler;
            _symbol = symbol;
            _channel = "trades";

            MessageRouter = MessageRouter.Create([]);
        }

        public override void DoHandleReset()
        {
            _channelId = -1;
            _firstUpdate = true;
        }

        public override void HandleSubQueryResponse(SocketConnection connection, object? message)
        {
            var data = (BitfinexResponse?)message;
            if (data == null)
            {
                // Timeout or other connection error
                // We need to reconnect the connection as there might now be a subscription for which we don't know the channel id, which means we also can't unsubscribe it
                _ = connection.TriggerReconnectAsync();
                return;
            }

            _channelId = data!.ChannelId!.Value;
            _firstUpdate = true;

            MessageRouter = MessageRouter.Create([
                MessageRoute<BitfinexTradeUpdate>.CreateWithoutTopicFilter(_channelId.ToString() + "single", DoHandleMessage),
                MessageRoute<BitfinexTradeArrayUpdate>.CreateWithoutTopicFilter(_channelId.ToString() + "array", DoHandleMessage),
                MessageRoute<BitfinexStringUpdate>.CreateWithoutTopicFilter(_channelId.ToString() + "hb", DoHandleHeartbeat),
                ]);
        }

        protected override Query? GetSubQuery(SocketConnection connection)
        {
            return new BitfinexSubQuery("subscribe", _channel, _symbol, null, null, null, null);
        }
        protected override Query? GetUnsubQuery(SocketConnection connection)
        {
            if (_channelId == 0 || _channelId == -1)
                return null;

            return new BitfinexUnsubQuery(_channelId);
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexTradeUpdate message)
        {
            connection.UpdateSequenceNumber(message.Sequence);
            _client.UpdateTimeOffset(message.Timestamp);

            _handler?.Invoke(receiveTime, originalData, _firstUpdate ? SocketUpdateType.Snapshot : SocketUpdateType.Update, [message.Data], message.Sequence, message.Timestamp);
            _firstUpdate = false;
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexTradeArrayUpdate message)
        {
            connection.UpdateSequenceNumber(message.Sequence);
            _client.UpdateTimeOffset(message.Timestamp);

            _handler?.Invoke(receiveTime, originalData, _firstUpdate ? SocketUpdateType.Snapshot : SocketUpdateType.Update, message.Data, message.Sequence, message.Timestamp);
            _firstUpdate = false;
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleHeartbeat(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexStringUpdate message)
        {
            connection.UpdateSequenceNumber(message.Sequence);
            return CallResult.SuccessResult;
        }
    }
}
