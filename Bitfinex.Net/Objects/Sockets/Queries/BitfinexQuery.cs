using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Models.Socket;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bitfinex.Net.Objects.Sockets.Queries
{
    internal class BitfinexQuery<T> : Query<BitfinexSocketEvent<BitfinexNotification<T>>>
    {
        public override List<string> StreamIdentifiers { get; } = new List<string> { "0" };

        public BitfinexQuery(BitfinexSocketQuery request) : base(request, true, 1)
        {
            TypeMapping = new Dictionary<string, Type>
            {
                { "n-single", typeof(BitfinexSocketEvent<BitfinexNotification<T>>) },
                { "n-array", typeof(BitfinexSocketEvent<BitfinexNotification<T>>) }
            };
        }

        public override Task<CallResult<BitfinexSocketEvent<BitfinexNotification<T>>>> HandleMessageAsync(SocketConnection connection, DataEvent<ParsedMessage<BitfinexSocketEvent<BitfinexNotification<T>>>> message)
        {
            if (message.Data.TypedData.Data.Result != "SUCCESS")
                return Task.FromResult(new CallResult<BitfinexSocketEvent<BitfinexNotification<T>>>(new ServerError(message.Data.TypedData.Data.ErrorMessage)));

            return Task.FromResult(new CallResult<BitfinexSocketEvent<BitfinexNotification<T>>>((BitfinexSocketEvent<BitfinexNotification<T>>)message.Data.Data));
        }
    }
}
