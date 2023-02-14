using CryptoExchange.Net.Interfaces;
using System;

namespace Bitfinex.Net.Interfaces.Clients.GeneralApi
{
    /// <summary>
    /// General API endpoints
    /// </summary>
    public interface IBitfinexClientGeneralApi : IRestApiClient, IDisposable
    {
        /// <summary>
        /// Endpoints related to funding
        /// </summary>
        public IBitfinexClientGeneralApiFunding Funding { get; }
    }
}
