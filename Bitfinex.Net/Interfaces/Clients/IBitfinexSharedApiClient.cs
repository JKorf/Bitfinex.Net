using Bitfinex.Net.Interfaces.Clients.ExchangeApi;

namespace Bitfinex.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for the shared REST and WebSocket API implementations of Bitfinex
    /// </summary>
    public interface IBitfinexSharedApiClient
    {
        /// <summary>
        /// REST shared API implementations
        /// </summary>
        IBitfinexRestClientExchangeSharedApi Rest { get; }

        /// <summary>
        /// WebSocket shared API implementations
        /// </summary>
        IBitfinexSocketClientExchangeSharedApi Socket { get; }
    }
}
