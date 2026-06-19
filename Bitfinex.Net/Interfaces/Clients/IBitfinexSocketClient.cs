using Bitfinex.Net.Interfaces.Clients.ExchangeApi;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces.Clients;
using CryptoExchange.Net.Objects.Options;

namespace Bitfinex.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for accessing the Bitfinex websocket API
    /// </summary>
    public interface IBitfinexSocketClient : ISocketClient<BitfinexCredentials>
    {
        /// <summary>
        /// Streams
        /// </summary>
        /// <see cref="IBitfinexSocketClientExchangeApi"/>
        IBitfinexSocketClientExchangeApi ExchangeApi { get; }
    }
}