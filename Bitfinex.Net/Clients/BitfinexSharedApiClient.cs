using Bitfinex.Net.Interfaces.Clients;
using Bitfinex.Net.Interfaces.Clients.ExchangeApi;

namespace Bitfinex.Net.Clients
{
    /// <inheritdoc />
    public class BitfinexSharedApiClient : IBitfinexSharedApiClient
    {
        /// <inheritdoc />
        public IBitfinexRestClientExchangeSharedApi Rest { get; }
        /// <inheritdoc />
        public IBitfinexSocketClientExchangeSharedApi Socket { get; }

        /// <summary>
        /// ctor
        /// </summary>
        public BitfinexSharedApiClient(
            IBitfinexRestClient restClient,
            IBitfinexSocketClient socketClient)
        {
            Rest = restClient.ExchangeApi.SharedApi;
            Socket = socketClient.ExchangeApi.SharedApi;
        }
    }
}
