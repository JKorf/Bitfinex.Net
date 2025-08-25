using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects.Internal;
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
    internal class BitfinexSubscription<TSingle, TArray, TItem> : Subscription<BitfinexResponse, BitfinexResponse>
        where TArray: BitfinexUpdate<TItem[]>
        where TSingle: BitfinexUpdate<TItem>
    {
        private string _channel;
        private string? _symbol;
        private bool _sendSymbol;
        private string? _precision;
        private string? _frequency;
        private string? _length;
        private string? _key;
        private int _channelId;
        private bool _firstUpdate;
        private Action<DataEvent<TItem[]>> _handler;

        public BitfinexSubscription(ILogger logger,
            string channel,
            string? symbol,
            Action<DataEvent<TItem[]>> handler,
            bool authenticated = false,
            Precision? precision = null,
            Frequency? frequency = null,
            int? length = null,
            string? key = null,
            bool sendSymbol = true)
            : base(logger, authenticated)
        {
            _handler = handler;
            _symbol = symbol;
            _key = key;
            _channel = channel;
            _precision = precision == null ? null : EnumConverter.GetString(precision);
            _frequency = frequency == null ? null: EnumConverter.GetString(frequency);
            _length = length?.ToString();
            _sendSymbol = sendSymbol;

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
                ]);
        }

        protected override Query? GetSubQuery(SocketConnection connection)
        {
            return new BitfinexSubQuery("subscribe", _channel, _sendSymbol ? _symbol : null, _precision, _frequency, _length, _key);
        }

        protected override Query? GetUnsubQuery(SocketConnection connection)
        {
            if (_channelId == 0)
                return null;

            return new BitfinexUnsubQuery(_channelId);
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
