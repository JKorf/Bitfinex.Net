using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitfinex.Net.Objects.Sockets.Queries
{
    internal class BitfinexSubQuery : Query<BitfinexResponse>
    {
        public override HashSet<string> ListenerIdentifiers { get; set; }

        public BitfinexSubQuery(string evnt, string channel, string? symbol, string? precision, string? frequency, string? length, string? key) : base(new BitfinexBookRequest
        {
            Channel = channel,
            Symbol = symbol,
            Event = evnt,
            Frequency = frequency,
            Length = length,
            Precision = precision,
            Key = key
        }, false, 1)
        {
            if (evnt == "subscribe" || evnt == "unsubscribe")
                evnt += "d";

            ListenerIdentifiers = new HashSet<string> 
            { 
                evnt + channel + symbol + precision + frequency + length + key, 
                "error" + channel + symbol + precision + frequency + length + key 
            };
        }

        public override Task<CallResult<BitfinexResponse>> HandleMessageAsync(SocketConnection connection, DataEvent<BitfinexResponse> message)
        {
            if (message.Data.Event == "error")
                return Task.FromResult(new CallResult<BitfinexResponse>(new ServerError(message.Data.Message!)));

            return Task.FromResult(new CallResult<BitfinexResponse>(message.Data));
        }
    }
}
