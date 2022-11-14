using CryptoExchange.Net.Interfaces;
using System;

namespace Bitfinex.Net.Interfaces.Clients.GeneralApi
{
    /// <summary>
    /// General API endpoints
    /// </summary>
    public interface IBitfinexClientGeneralApi : IDisposable
    {
        /// <summary>
        /// The factory for creating requests. Used for unit testing
        /// </summary>
        IRequestFactory RequestFactory { get; set; }

        /// <summary>
        /// Endpoints related to funding
        /// </summary>
        public IBitfinexClientGeneralApiFunding Funding { get; }
    }
}
