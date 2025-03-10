using CryptoExchange.Net.Interfaces;
using System;

namespace Bitfinex.Net.Interfaces.Clients.SpotApi
{
    /// <summary>
    /// Spot API endpoints
    /// </summary>
    public interface IBitfinexRestClientSpotApi : IRestApiClient, IDisposable
    {
        /// <summary>
        /// Endpoints related to account settings, info or actions
        /// </summary>
        public IBitfinexRestClientSpotApiAccount Account { get; }

        /// <summary>
        /// Endpoints related to retrieving market and system data
        /// </summary>
        public IBitfinexRestClientSpotApiExchangeData ExchangeData { get; }

        /// <summary>
        /// Endpoints related to orders and trades
        /// </summary>
        public IBitfinexRestClientSpotApiTrading Trading { get; }

        /// <summary>
        /// Get the shared rest requests client. This interface is shared with other exchanges to allow for a common implementation for different exchanges.
        /// </summary>
        public IBitfinexRestClientSpotApiShared SharedClient { get; }
    }
}
