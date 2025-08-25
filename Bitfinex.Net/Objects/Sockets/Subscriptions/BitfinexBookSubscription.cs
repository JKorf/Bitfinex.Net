using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Sockets.Queries;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Bitfinex.Net.Objects.Sockets.Subscriptions
{
    internal class BitfinexBookSubscription<TSingle, TArray, TItem> : Subscription<BitfinexResponse, BitfinexResponse>
        where TArray : BitfinexUpdate<TItem[]>
        where TSingle : BitfinexUpdate<TItem>
    {
        private string _channel;
        private string? _symbol;
        private string? _precision;
        private string? _frequency;
        private string? _length;
        private int _channelId;
        private bool _firstUpdate;
        private Action<DataEvent<TItem[]>> _handler;
        private Action<DataEvent<int>>? _checksumHandler;

        public BitfinexBookSubscription(ILogger logger,
            string symbol,
            Action<DataEvent<TItem[]>> handler,
            Action<DataEvent<int>>? checksumHandler,
            Precision? precision = null,
            Frequency? frequency = null,
            int? length = null,
            bool authenticated = false)
            : base(logger, authenticated)
        {
            _handler = handler;
            _checksumHandler = checksumHandler;
            _symbol = symbol;
            _channel = "book";

            _precision = precision == null ? null : EnumConverter.GetString(precision);
            _frequency = frequency == null ? null : EnumConverter.GetString(frequency);
            _length = length?.ToString();

            MessageMatcher = MessageMatcher.Create([]);
        }

        public override void DoHandleReset()
        {
            _channelId = -1;
            _firstUpdate = true;
        }

        public override void HandleSubQueryResponse(BitfinexResponse message)
        {
            _channelId = message.ChannelId!.Value;
            _firstUpdate = true;

            MessageMatcher = MessageMatcher.Create([
                new MessageHandlerLink<TSingle>(_channelId.ToString() + "single", DoHandleMessage),
                new MessageHandlerLink<TArray>(_channelId.ToString() + "array", DoHandleMessage),
                new MessageHandlerLink<BitfinexChecksum>(_channelId.ToString() + "cs", DoHandleMessage),
                ]);
        }

        protected override Query? GetSubQuery(SocketConnection connection)
        {
            return new BitfinexSubQuery("subscribe", _channel, _symbol, _precision, _frequency, _length, null);
        }
        protected override Query? GetUnsubQuery(SocketConnection connection)
        {
            if (_channelId == 0)
                return null;

            return new BitfinexUnsubQuery(_channelId);
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<BitfinexChecksum> message)
        {
            _checksumHandler?.Invoke(message.As(message.Data.Checksum, _channel, _symbol, _firstUpdate ? SocketUpdateType.Snapshot : SocketUpdateType.Update));
            _firstUpdate = false;
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<TSingle> message)
        {
            _handler?.Invoke(message.As<TItem[]>([message.Data.Data], _channel, _symbol, _firstUpdate ? SocketUpdateType.Snapshot : SocketUpdateType.Update));
            _firstUpdate = false;
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<TArray> message)
        {
            _handler?.Invoke(message.As(message.Data.Data, _channel, _symbol, _firstUpdate ? SocketUpdateType.Snapshot : SocketUpdateType.Update));
            _firstUpdate = false;
            return CallResult.SuccessResult;
        }
    }
}
