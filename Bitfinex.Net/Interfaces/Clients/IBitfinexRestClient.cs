using Bitfinex.Net.Interfaces.Clients.GeneralApi;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;

namespace Bitfinex.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for accessing the Bitfinex API. 
    /// </summary>
    public interface IBitfinexRestClient : IRestClient
    {
        /// <summary>
        /// General endpoints
        /// </summary>
        IBitfinexRestClientGeneralApi GeneralApi { get; }

        /// <summary>
        /// Spot endpoints
        /// </summary>
        IBitfinexRestClientSpotApi SpotApi { get; }

        /// <summary>
        /// Set the API credentials for this client. All Api clients in this client will use the new credentials, regardless of earlier set options.
        /// </summary>
        /// <param name="credentials">The credentials to set</param>
        void SetApiCredentials(ApiCredentials credentials);
    }
}