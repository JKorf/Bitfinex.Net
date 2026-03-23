using Bitfinex.Net.Interfaces.Clients.SpotApi;
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
        /// Spot streams
        /// </summary>
        /// <see cref="IBitfinexSocketClientSpotApi"/>
        IBitfinexSocketClientSpotApi SpotApi { get; }
    }
}