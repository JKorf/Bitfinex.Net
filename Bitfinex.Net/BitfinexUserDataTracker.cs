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
            SpotUserDataTrackerConfig config) : base(
                logger,
                restClient.SpotApi.SharedClient,
                null,
                restClient.SpotApi.SharedClient,
                socketClient.SpotApi.SharedClient,
                restClient.SpotApi.SharedClient,
                socketClient.SpotApi.SharedClient,
                socketClient.SpotApi.SharedClient,
                userIdentifier,
                config)
        {
        }
    }
}
