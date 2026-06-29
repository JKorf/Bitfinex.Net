using Bitfinex.Net.Interfaces.Clients.ExchangeApi;
using Bitfinex.Net.Interfaces.Clients.GeneralApi;
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
        /// Exchange endpoints
        /// </summary>
        /// <see cref="IBitfinexRestClientExchangeApi"/>
        IBitfinexRestClientExchangeApi ExchangeApi { get; }
    }
}