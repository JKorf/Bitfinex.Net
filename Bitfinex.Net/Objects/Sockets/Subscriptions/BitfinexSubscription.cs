using Bitfinex.Net.Objects.Sockets.Queries;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bitfinex.Net.Objects.Sockets.Subscriptions
{
    internal class BitfinexSubscription<T> : Subscription<BitfinexResponse, BitfinexUpdate<T>>
    {
        private string _channel;
        private string _symbol;
        private int _channelId;
        private Action<DataEvent<T>> _handler;

        private List<string> _identifiers;

        public override List<string> Identifiers => _identifiers;

        public BitfinexSubscription(ILogger logger, string channel, string symbol, Action<DataEvent<T>> handler, bool authenticated) : base(logger, authenticated)
        {
            _handler = handler;
            _channel = channel;
            _symbol = symbol;
        }

        public override void HandleSubQueryResponse(ParsedMessage<BitfinexResponse> message)
        {
            _channelId = message.TypedData.ChannelId.Value;

            // Doesn't work, as the subscription is immediately added along with the identifiers
            // Would maybe be better to wait with adding the subscription untill it is confirmed?
            _identifiers = new List<string> { _channelId.ToString() };
        }

        public override BaseQuery? GetSubQuery(SocketConnection connection)
        {
            return new BitfinexQuery("subscribe", _channel, _symbol, Authenticated);
        }
        public override BaseQuery? GetUnsubQuery()
        {
            return new BitfinexQuery("unsubscribe", _channel, _symbol, Authenticated);
        }

        public override Task<CallResult> HandleEventAsync(SocketConnection connection, DataEvent<ParsedMessage<BitfinexUpdate<T>>> message)
        {
            _handler.Invoke(message.As(message.Data.TypedData.Data)); // TODo
            return Task.FromResult(new CallResult(null));
        }
    }
}
