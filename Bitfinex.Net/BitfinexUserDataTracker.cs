using Bitfinex.Net.Interfaces.Clients;
using CryptoExchange.Net.Trackers.UserData;
using Microsoft.Extensions.Logging;

namespace Bitfinex.Net
{
    public class BitfinexUserSpotDataTracker : UserSpotDataTracker
    {
        public BitfinexUserSpotDataTracker(
            ILogger<BitfinexUserSpotDataTracker> logger,
            IBitfinexRestClient restClient,
            IBitfinexSocketClient socketClient,
            string? userIdentifier,
            UserDataTrackerConfig config) : base(logger, restClient.SpotApi.SharedClient, socketClient.SpotApi.SharedClient, userIdentifier, config)
        {

        }
    }
}
