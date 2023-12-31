using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Sockets.Queries;
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
        private string _channel;
        private string? _symbol;
        private string? _precision;
        private string? _frequency;
        private string? _length;
        private string? _key;
        private int _channelId;
        private bool _firstUpdate;
        private Action<DataEvent<IEnumerable<T>>> _handler;
        private Action<DataEvent<int>>? _checksumHandler;

        private List<string> _streamIdentifiers;

        public override Dictionary<string, Type> TypeMapping { get; } = new Dictionary<string, Type>
        {
            { "cs-single", typeof(BitfinexChecksum) },
            { "hb-single", typeof(BitfinexUpdate<string>) },
            { "-single", typeof(BitfinexUpdate<T>) },
            { "-array", typeof(BitfinexUpdate<IEnumerable<T>>) },
            { "te-single", typeof(BitfinexUpdate3<T>) },
            { "te-array", typeof(BitfinexUpdate3<IEnumerable<T>>) },
            { "tu-single", typeof(BitfinexUpdate3<T>) },
            { "tu-array", typeof(BitfinexUpdate3<IEnumerable<T>>) },
        };

        public override List<string> StreamIdentifiers => _streamIdentifiers;

        public BitfinexSubscription(ILogger logger,
            string channel,
            string? symbol,
            Action<DataEvent<IEnumerable<T>>> handler,
            Action<DataEvent<int>>? checksumHandler = null, 
            bool authenticated = false,
            Precision? precision = null,
            Frequency? frequency = null,
            int? length = null,
            string? key = null)
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
        }

        public override void HandleSubQueryResponse(ParsedMessage<BitfinexResponse> message)
        {
            // TODO this doesn't update when reconnecting/subscribing
            _channelId = message.TypedData.ChannelId.Value;
            _firstUpdate = true;
            // Doesn't work, as the subscription is immediately added along with the identifiers
            // Would maybe be better to wait with adding the subscription untill it is confirmed?
            _streamIdentifiers = new List<string> { _channelId.ToString() };
        }

        public override BaseQuery? GetSubQuery(SocketConnection connection)
        {
            return new BitfinexQuery("subscribe", _channel, _symbol, _precision, _frequency, _length, _key);
        }
        public override BaseQuery? GetUnsubQuery()
        {
            return new BitfinexQuery("unsubscribe", _channel, _symbol, _precision, _frequency, _length, _key);
        }

        public override Task<CallResult> DoHandleMessageAsync(SocketConnection connection, DataEvent<BaseParsedMessage> message)
        {
            // var typedObject = message.As(TypeMapping[message.Data.TypeIdentifier])


            if (message.Data.TypeIdentifier == "hb")
                return Task.FromResult(new CallResult(null));
            else if (message.Data.TypeIdentifier == "cs")
                _checksumHandler?.Invoke(message.As(((BitfinexChecksum)message.Data.Data).Checksum));
            else if (message.Data.TypeIdentifier == "single1")
                _handler.Invoke(message.As<IEnumerable<T>>(new[] { ((BitfinexUpdate<T>)message.Data.Data).Data }, _symbol, SocketUpdateType.Update));
            else if (message.Data.TypeIdentifier == "single2")
                _handler.Invoke(message.As<IEnumerable<T>>(new[] { ((BitfinexUpdate3<T>)message.Data.Data).Data }, _symbol, SocketUpdateType.Update));
            else if (message.Data.TypeIdentifier == "array1")
                _handler.Invoke(message.As(((BitfinexUpdate<IEnumerable<T>>)message.Data.Data).Data, _symbol, _firstUpdate ? SocketUpdateType.Snapshot : SocketUpdateType.Update));
            else if (message.Data.TypeIdentifier == "array2")
                _handler.Invoke(message.As(((BitfinexUpdate3<IEnumerable<T>>)message.Data.Data).Data, _symbol, _firstUpdate ? SocketUpdateType.Snapshot : SocketUpdateType.Update));

            return Task.FromResult(new CallResult(null));
        }
    }
}
