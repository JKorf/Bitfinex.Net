using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Sockets.Queries;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitfinex.Net.Objects.Sockets.Subscriptions
{
    internal class BitfinexSubscription<T> : Subscription<BitfinexResponse, BitfinexResponse>
    {
        private static readonly MessagePath _1Path = MessagePath.Get().Index(1);
        private static readonly MessagePath _10Path = MessagePath.Get().Index(1).Index(0);
        private static readonly MessagePath _20Path = MessagePath.Get().Index(2).Index(0);

        private string _channel;
        private string? _symbol;
        private bool _sendSymbol;
        private string? _precision;
        private string? _frequency;
        private string? _length;
        private string? _key;
        private int _channelId;
        private bool _firstUpdate;
        private Action<DataEvent<IEnumerable<T>>> _handler;
        private Action<DataEvent<int>>? _checksumHandler;

        public override HashSet<string> ListenerIdentifiers { get; set; } = new HashSet<string>();

        public BitfinexSubscription(ILogger logger,
            string channel,
            string? symbol,
            Action<DataEvent<IEnumerable<T>>> handler,
            Action<DataEvent<int>>? checksumHandler = null, 
            bool authenticated = false,
            Precision? precision = null,
            Frequency? frequency = null,
            int? length = null,
            string? key = null,
            bool sendSymbol = true)
            : base(logger, authenticated)
        {
            _handler = handler;
            _checksumHandler = checksumHandler;
            _symbol = symbol;
            _key = key;
            _channel = channel;
            _precision = precision == null ? null : JsonConvert.SerializeObject(precision, new PrecisionConverter(false));
            _frequency = frequency == null ? null: JsonConvert.SerializeObject(frequency, new FrequencyConverter(false));
            _length = length?.ToString();
            _sendSymbol = sendSymbol;
        }

        /// <inheritdoc />
        public override Type? GetMessageType(IMessageAccessor message)
        {
            var identifier = message.GetValue<string>(_1Path);

            if (string.Equals(identifier, "cs", StringComparison.Ordinal))
                return typeof(BitfinexChecksum);

            if (string.Equals(identifier, "hb", StringComparison.Ordinal))
                return typeof(BitfinexUpdate<string>);

            if (identifier == null)
            {
                var nodeType1 = message.GetNodeType(_10Path);
                return nodeType1 == NodeType.Array ? typeof(BitfinexUpdate<IEnumerable<T>>) : typeof(BitfinexUpdate<T>);
            }

            var nodeType = message.GetNodeType(_20Path);
            return nodeType == NodeType.Array ? typeof(BitfinexTopicUpdate<IEnumerable<T>>) : typeof(BitfinexTopicUpdate<T>);
        }

        public override void HandleSubQueryResponse(BitfinexResponse message)
        {
            _channelId = message.ChannelId!.Value;
            _firstUpdate = true;
            ListenerIdentifiers = new HashSet<string> { _channelId.ToString() };
        }

        public override Query? GetSubQuery(SocketConnection connection)
        {
            return new BitfinexSubQuery("subscribe", _channel, _sendSymbol ? _symbol : null, _precision, _frequency, _length, _key);
        }
        public override Query? GetUnsubQuery()
        {
            return new BitfinexUnsubQuery(_channelId);
        }

        public override CallResult DoHandleMessage(SocketConnection connection, DataEvent<object> message)
        {
            if (message.Data is BitfinexChecksum checksum)
                _checksumHandler?.Invoke(message.As(checksum.Checksum, _symbol));
            else if (message.Data is BitfinexUpdate<IEnumerable<T>> arrayUpdate)
                _handler?.Invoke(message.As(arrayUpdate.Data, _symbol, _firstUpdate ? SocketUpdateType.Snapshot : SocketUpdateType.Update));
            else if (message.Data is BitfinexUpdate<T> singleUpdate)
                _handler?.Invoke(message.As<IEnumerable<T>>(new[] { singleUpdate.Data }, _symbol, _firstUpdate ? SocketUpdateType.Snapshot : SocketUpdateType.Update));
            else if (message.Data is BitfinexTopicUpdate<IEnumerable<T>> array3Update)
                _handler?.Invoke(message.As(array3Update.Data, _symbol, _firstUpdate ? SocketUpdateType.Snapshot : SocketUpdateType.Update));
            else if (message.Data is BitfinexTopicUpdate<T> single3Update)
                _handler?.Invoke(message.As<IEnumerable<T>>(new[] { single3Update.Data }, _symbol, _firstUpdate ? SocketUpdateType.Snapshot : SocketUpdateType.Update));

            _firstUpdate = false;
            return new CallResult(null);
        }
    }
}
