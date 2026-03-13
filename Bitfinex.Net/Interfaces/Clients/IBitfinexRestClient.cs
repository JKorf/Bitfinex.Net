using Bitfinex.Net.Interfaces.Clients.GeneralApi;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces.Clients;
using CryptoExchange.Net.Objects.Options;

namespace Bitfinex.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for accessing the Bitfinex API. 
    /// </summary>
    public interface IBitfinexRestClient : IRestClient<BitfinexCredentials>
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
    }
}