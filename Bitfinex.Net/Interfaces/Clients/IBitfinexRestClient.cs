using Bitfinex.Net.Interfaces.Clients.GeneralApi;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects.Options;

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
        /// <see cref="IBitfinexRestClientGeneralApi"/>
        IBitfinexRestClientGeneralApi GeneralApi { get; }

        /// <summary>
        /// Spot endpoints
        /// </summary>
        /// <see cref="IBitfinexRestClientSpotApi"/>
        IBitfinexRestClientSpotApi SpotApi { get; }

        /// <summary>
        /// Update specific options
        /// </summary>
        /// <param name="options">Options to update. Only specific options are changeable after the client has been created</param>
        void SetOptions(UpdateOptions options);

        /// <summary>
        /// Set the API credentials for this client. All Api clients in this client will use the new credentials, regardless of earlier set options.
        /// </summary>
        /// <param name="credentials">The credentials to set</param>
        void SetApiCredentials(ApiCredentials credentials);
    }
}