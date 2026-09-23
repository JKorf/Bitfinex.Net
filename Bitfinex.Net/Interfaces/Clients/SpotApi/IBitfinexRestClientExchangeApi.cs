using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces.Clients;
using System;

namespace Bitfinex.Net.Interfaces.Clients.ExchangeApi
{
    /// <summary>
    /// API endpoints
    /// </summary>
    public interface IBitfinexRestClientExchangeApi : IRestApiClient<BitfinexCredentials>, IDisposable
    {
        /// <summary>
        /// Endpoints related to account settings, info or actions
        /// </summary>
        /// <see cref="IBitfinexRestClientExchangeApiAccount"/>
        public IBitfinexRestClientExchangeApiAccount Account { get; }

        /// <summary>
        /// Endpoints related to retrieving market and system data
        /// </summary>
        /// <see cref="IBitfinexRestClientExchangeApiExchangeData"/>
        public IBitfinexRestClientExchangeApiExchangeData ExchangeData { get; }

        /// <summary>
        /// Endpoints related to orders and trades
        /// </summary>
        /// <see cref="IBitfinexRestClientExchangeApiTrading"/>
        public IBitfinexRestClientExchangeApiTrading Trading { get; }

        /// <summary>
        /// [V1] Get the shared rest requests client. For new implementations prefer using <see cref="SharedApi"/>
        /// </summary>
        public IBitfinexRestClientExchangeApiShared SharedClient { get; }
        /// <summary>
        /// [V2] Gets the aggregate Shared API interface. Shared APIs provide a common,
        /// exchange-independent contract for accessing functionality across different
        /// exchange client libraries.
        /// </summary>
        public IBitfinexRestClientExchangeSharedApi SharedApi { get; }
    }
}
