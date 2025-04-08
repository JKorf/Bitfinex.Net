using CryptoExchange.Net.Interfaces;
using System;

namespace Bitfinex.Net.Interfaces.Clients.GeneralApi
{
    /// <summary>
    /// General API endpoints
    /// </summary>
    public interface IBitfinexRestClientGeneralApi : IRestApiClient, IDisposable
    {
        /// <summary>
        /// Endpoints related to funding
        /// </summary>
        /// <see cref="IBitfinexRestClientGeneralApiFunding"/>
        public IBitfinexRestClientGeneralApiFunding Funding { get; }
    }
}
