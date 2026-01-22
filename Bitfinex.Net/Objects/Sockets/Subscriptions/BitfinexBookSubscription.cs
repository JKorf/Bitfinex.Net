using Bitfinex.Net.Clients.SpotApi;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Sockets.Queries;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Sockets.Subscriptions
{
    internal class BitfinexBookSubscription<TSingle, TArray, TItem> : Subscription
        where TArray : BitfinexUpdate<TItem[]>
        where TSingle : BitfinexUpdate<TItem>
    {
        private BitfinexSocketClientSpotApi _client;

        private string _channel;
        private string? _symbol;
        private string? _precision;
        private string? _frequency;
        private string? _length;
        private int _channelId;
        private bool _firstUpdate;
        private Action<DateTime, string?, SocketUpdateType, TItem[], long, DateTime> _handler;
        private Action<DataEvent<int>>? _checksumHandler;

        public BitfinexBookSubscription(ILogger logger,
            BitfinexSocketClientSpotApi client,
            string symbol,
            Action<DateTime, string?, SocketUpdateType, TItem[], long, DateTime> handler,
            Action<DataEvent<int>>? checksumHandler,
            Precision? precision = null,
            Frequency? frequency = null,
            int? length = null,
            bool authenticated = false)
            : base(logger, authenticated)
        {
            _client = client;

            _handler = handler;
            _checksumHandler = checksumHandler;
            _symbol = symbol;
            _channel = "book";

            _precision = precision == null ? null : EnumConverter.GetString(precision);
            _frequency = frequency == null ? null : EnumConverter.GetString(frequency);
            _length = length?.ToString();

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

            _channelId = data.ChannelId!.Value;
            _firstUpdate = true;

            MessageRouter = MessageRouter.Create([
                MessageRoute<TSingle>.CreateWithoutTopicFilter(_channelId.ToString() + "single", DoHandleMessage),
                MessageRoute<TArray>.CreateWithoutTopicFilter(_channelId.ToString() + "array", DoHandleMessage),
                MessageRoute<BitfinexChecksum>.CreateWithoutTopicFilter(_channelId.ToString() + "cs", DoHandleMessage),
                MessageRoute<BitfinexStringUpdate>.CreateWithoutTopicFilter(_channelId.ToString() + "hb", DoHandleHeartbeat),
                ]);
        }

        protected override Query? GetSubQuery(SocketConnection connection)
        {
            return new BitfinexSubQuery("subscribe", _channel, _symbol, _precision, _frequency, _length, null);
        }
        protected override Query? GetUnsubQuery(SocketConnection connection)
        {
            if (_channelId == 0 || _channelId == -1)
                return null;

            return new BitfinexUnsubQuery(_channelId);
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexChecksum message)
        {
            connection.UpdateSequenceNumber(message.Sequence);

            _checksumHandler?.Invoke(
                new DataEvent<int>(BitfinexExchange.ExchangeName, message.Checksum, receiveTime, originalData)
                    .WithStreamId(_channel)
                    .WithSymbol(_symbol)
                    .WithUpdateType(_firstUpdate ? SocketUpdateType.Snapshot : SocketUpdateType.Update)
                );
            _firstUpdate = false;
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, TSingle message)
        {
            _client.UpdateTimeOffset(message.Timestamp);
            connection.UpdateSequenceNumber(message.Sequence);

            _handler?.Invoke(receiveTime, originalData, _firstUpdate ? SocketUpdateType.Snapshot : SocketUpdateType.Update, [message.Data], message.Sequence, message.Timestamp);
            _firstUpdate = false;
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, TArray message)
        {
            _client.UpdateTimeOffset(message.Timestamp);
            connection.UpdateSequenceNumber(message.Sequence);

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
