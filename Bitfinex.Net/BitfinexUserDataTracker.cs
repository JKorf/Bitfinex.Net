using Bitfinex.Net.Interfaces.Clients;
using CryptoExchange.Net.Trackers.UserData;
using CryptoExchange.Net.Trackers.UserData.Objects;
using Microsoft.Extensions.Logging;

namespace Bitfinex.Net
{
    /// <inheritdoc/>
    public class BitfinexUserSpotDataTracker : UserSpotDataTracker
    {
        /// <inheritdoc/>
        public BitfinexUserSpotDataTracker(
            ILogger<BitfinexUserSpotDataTracker> logger,
            IBitfinexRestClient restClient,
            IBitfinexSocketClient socketClient,
            string? userIdentifier,
            SpotUserDataTrackerConfig? config) : base(
                logger,
                restClient.ExchangeApi.SharedClient,
                restClient.ExchangeApi.SharedClient,
                socketClient.ExchangeApi.SharedClient,
                restClient.ExchangeApi.SharedClient,
                socketClient.ExchangeApi.SharedClient,
                socketClient.ExchangeApi.SharedClient,
                userIdentifier,
                config ?? new SpotUserDataTrackerConfig())
        {
        }
    }
}
