using Bitfinex.Net.Interfaces.Clients;
using Bitfinex.Net.Interfaces.Clients.ExchangeApi;
using Bitfinex.Net.Objects.Options;
using CryptoExchange.Net.SharedApis;
using Microsoft.Extensions.Options;

namespace Bitfinex.Net.Clients
{
    /// <inheritdoc />
    public class BitfinexSharedApiClient : SharedApiClientBase, IBitfinexSharedApiClient
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
            IBitfinexSocketClient socketClient,
            IOptions<BitfinexOptions> options)
            : base(options.Value.SharedApi.PreferredTransport,
                  restClient.ExchangeApi.SharedApi,
                  socketClient.ExchangeApi.SharedApi)
        {
            Rest = restClient.ExchangeApi.SharedApi;
            Socket = socketClient.ExchangeApi.SharedApi;
        }
    }
}
