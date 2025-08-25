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
    internal class BitfinexTradeSubscription: Subscription<BitfinexResponse, BitfinexResponse>
    {
        private string _channel;
        private string _symbol;
        private int _channelId;
        private bool _firstUpdate;
        private Action<DataEvent<BitfinexTradeSimple[]>> _handler;

        public BitfinexTradeSubscription(ILogger logger,
            string symbol,
            Action<DataEvent<BitfinexTradeSimple[]>> handler,
            bool authenticated = false)
            : base(logger, authenticated)
        {
            _handler = handler;
            _symbol = symbol;
            _channel = "trades";

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
                new MessageHandlerLink<BitfinexTradeUpdate>(_channelId.ToString() + "single", DoHandleMessage),
                new MessageHandlerLink<BitfinexTradeArrayUpdate>(_channelId.ToString() + "array", DoHandleMessage),
                ]);
        }

        protected override Query? GetSubQuery(SocketConnection connection)
        {
            return new BitfinexSubQuery("subscribe", _channel, _symbol, null, null, null, null);
        }
        protected override Query? GetUnsubQuery(SocketConnection connection)
        {
            if (_channelId == 0)
                return null;

            return new BitfinexUnsubQuery(_channelId);
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<BitfinexTradeUpdate> message)
        {
            _handler?.Invoke(message.As<BitfinexTradeSimple[]>([message.Data.Data], _channel, _symbol, _firstUpdate ? SocketUpdateType.Snapshot : SocketUpdateType.Update));
            _firstUpdate = false;
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<BitfinexTradeArrayUpdate> message)
        {
            _handler?.Invoke(message.As(message.Data.Data, _channel, _symbol, _firstUpdate ? SocketUpdateType.Snapshot : SocketUpdateType.Update));
            _firstUpdate = false;
            return CallResult.SuccessResult;
        }
    }
}
