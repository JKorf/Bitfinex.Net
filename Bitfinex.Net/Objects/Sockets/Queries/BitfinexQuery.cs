using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Models.Socket;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitfinex.Net.Objects.Sockets.Queries
{
    internal class BitfinexQuery<T> : Query<BitfinexSocketEvent<BitfinexNotification<T>>>
    {
        private static readonly MessagePath _1Path = MessagePath.Get().Index(1);
        public override HashSet<string> ListenerIdentifiers { get; set; } = new HashSet<string> { "0" };

        public BitfinexQuery(BitfinexSocketQuery request) : base(request, true, 1)
        {
        }

        public override Type? GetMessageType(IMessageAccessor message)
        {
            if (message.GetValue<string>(_1Path) != "n")
                return null;

            return typeof(BitfinexSocketEvent<BitfinexNotification<T>>);
        }

        public override CallResult<BitfinexSocketEvent<BitfinexNotification<T>>> HandleMessage(SocketConnection connection, DataEvent<BitfinexSocketEvent<BitfinexNotification<T>>> message)
        {
            if (message.Data.Data.Result != "SUCCESS")
                return new CallResult<BitfinexSocketEvent<BitfinexNotification<T>>>(new ServerError(message.Data.Data.ErrorMessage!));

            return new CallResult<BitfinexSocketEvent<BitfinexNotification<T>>>(message.Data);
        }
    }
}
