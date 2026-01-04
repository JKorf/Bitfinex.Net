using Bitfinex.Net.Clients.SpotApi;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects.Sockets.Queries;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default;
using Microsoft.Extensions.Logging;
using System;

namespace Bitfinex.Net.Objects.Sockets.Subscriptions
{
    internal class BitfinexSubscription<TSingle, TArray, TItem> : Subscription
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
        private BitfinexSocketClientSpotApi _client;
        private Action<DateTime, string?, SocketUpdateType, TItem[], long, DateTime> _handler;

        public BitfinexSubscription(ILogger logger,
            BitfinexSocketClientSpotApi client,
            string channel,
            string? symbol,
            Action<DateTime, string?, SocketUpdateType, TItem[], long, DateTime> handler,
            bool authenticated = false,
            Precision? precision = null,
            Frequency? frequency = null,
            int? length = null,
            string? key = null,
            bool sendSymbol = true)
            : base(logger, authenticated)
        {
            _client = client;

            _handler = handler;
            _symbol = symbol;
            _key = key;
            _channel = channel;
            _precision = precision == null ? null : EnumConverter.GetString(precision);
            _frequency = frequency == null ? null: EnumConverter.GetString(frequency);
            _length = length?.ToString();
            _sendSymbol = sendSymbol;

            MessageMatcher = MessageMatcher.Create([]);
            MessageRouter = MessageRouter.Create([]);
        }

        public override void DoHandleReset()
        {
            _channelId = -1;
            _firstUpdate = true;
        }

        public override void HandleSubQueryResponse(object? message)
        {
            var data = (BitfinexResponse?)message;
            if (data == null)
                // Timeout or other connection error
                return;

            _channelId = data!.ChannelId!.Value;
            _firstUpdate = true;

            MessageRouter = MessageRouter.Create([
                MessageRoute<TSingle>.CreateWithoutTopicFilter(_channelId.ToString() + "single", DoHandleMessage),
                MessageRoute<TArray>.CreateWithoutTopicFilter(_channelId.ToString() + "array", DoHandleMessage),
                MessageRoute<BitfinexStringUpdate>.CreateWithoutTopicFilter(_channelId.ToString() + "hb", DoHandleHeartbeat),
                ]);
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

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, TSingle message)
        {
            _client.UpdateTimeOffset(message.Timestamp);

            _handler?.Invoke(receiveTime, originalData, _firstUpdate ? SocketUpdateType.Snapshot : SocketUpdateType.Update, [message.Data], message.Sequence, message.Timestamp);
            _firstUpdate = false;
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, TArray message)
        {
            _client.UpdateTimeOffset(message.Timestamp);

            _handler?.Invoke(receiveTime, originalData, _firstUpdate ? SocketUpdateType.Snapshot : SocketUpdateType.Update, message.Data, message.Sequence, message.Timestamp);
            _firstUpdate = false;
            return CallResult.SuccessResult;
        }

        public CallResult DoHandleHeartbeat(SocketConnection connection, DateTime receiveTime, string? originalData, BitfinexStringUpdate message)
        {
            return CallResult.SuccessResult;
        }
    }
}
