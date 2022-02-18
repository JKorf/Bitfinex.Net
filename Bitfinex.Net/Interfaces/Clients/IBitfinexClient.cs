using Bitfinex.Net.Interfaces.Clients.GeneralApi;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.Interfaces;

namespace Bitfinex.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for accessing the Bitfinex API. 
    /// </summary>
    public interface IBitfinexClient : IRestClient
    {
        /// <summary>
        /// General endpoints
        /// </summary>
        IBitfinexClientGeneralApi GeneralApi { get; }

        /// <summary>
        /// Spot endpoints
        /// </summary>
        IBitfinexClientSpotApi SpotApi { get; }
    }
}