using Bitfinex.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.Interfaces;

namespace Bitfinex.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for accessing the Bitfinex websocket API
    /// </summary>
    public interface IBitfinexSocketClient : ISocketClient
    {
        /// <summary>
        /// Spot streams
        /// </summary>
        IBitfinexSocketClientSpotStreams SpotStreams { get; }
    }
}