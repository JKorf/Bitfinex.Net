using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Trackers.UserData;

namespace Bitfinex.Net.Interfaces
{
    /// <summary>
    /// Tracker factory
    /// </summary>
    public interface IBitfinexTrackerFactory: ITrackerFactory
    {
        IUserSpotDataTracker CreateUserSpotDataTracker(string userIdentifier, UserDataTrackerConfig config, ApiCredentials credentials, BitfinexEnvironment? environment = null);
        IUserSpotDataTracker CreateUserSpotDataTracker(UserDataTrackerConfig config);
    }
}
