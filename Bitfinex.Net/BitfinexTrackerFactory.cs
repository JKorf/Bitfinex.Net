using Bitfinex.Net.Clients;
using Bitfinex.Net.Interfaces;
using Bitfinex.Net.Interfaces.Clients;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.SharedApis;
using CryptoExchange.Net.Trackers.Klines;
using CryptoExchange.Net.Trackers.Trades;
using CryptoExchange.Net.Trackers.UserData;
using CryptoExchange.Net.Trackers.UserData.Interfaces;
using CryptoExchange.Net.Trackers.UserData.Objects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;

namespace Bitfinex.Net
{
    /// <inheritdoc />
    public class BitfinexTrackerFactory : IBitfinexTrackerFactory
    {
        private readonly IServiceProvider? _serviceProvider;

        /// <summary>
        /// ctor
        /// </summary>
        public BitfinexTrackerFactory()
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="serviceProvider">Service provider for resolving logging and clients</param>
        public BitfinexTrackerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public bool CanCreateKlineTracker(SharedSymbol symbol, SharedKlineInterval interval)
        {
            var client = (_serviceProvider?.GetRequiredService<IBitfinexSocketClient>() ?? new BitfinexSocketClient()).SpotApi.SharedClient;
            return client.SubscribeKlineOptions.IsSupported(interval);
        }

        /// <inheritdoc />
        public bool CanCreateTradeTracker(SharedSymbol symbol) => true;

        /// <inheritdoc />
        public IKlineTracker CreateKlineTracker(SharedSymbol symbol, SharedKlineInterval interval, int? limit = null, TimeSpan? period = null)
        {
            var restClient = (_serviceProvider?.GetRequiredService<IBitfinexRestClient>() ?? new BitfinexRestClient()).SpotApi.SharedClient;
            var socketClient = (_serviceProvider?.GetRequiredService<IBitfinexSocketClient>() ?? new BitfinexSocketClient()).SpotApi.SharedClient;

            return new KlineTracker(
                _serviceProvider?.GetRequiredService<ILoggerFactory>().CreateLogger(restClient.Exchange),
                restClient,
                socketClient,
                symbol,
                interval,
                limit,
                period
                );
        }

        /// <inheritdoc />
        public ITradeTracker CreateTradeTracker(SharedSymbol symbol, int? limit = null, TimeSpan? period = null)
        {
            var restClient = (_serviceProvider?.GetRequiredService<IBitfinexRestClient>() ?? new BitfinexRestClient()).SpotApi.SharedClient;
            var socketClient = (_serviceProvider?.GetRequiredService<IBitfinexSocketClient>() ?? new BitfinexSocketClient()).SpotApi.SharedClient;

            return new TradeTracker(
                _serviceProvider?.GetRequiredService<ILoggerFactory>().CreateLogger(restClient.Exchange),
                null,
                restClient,
                socketClient,
                symbol,
                limit,
                period
                );
        }

        /// <inheritdoc />
        public IUserSpotDataTracker CreateUserSpotDataTracker(SpotUserDataTrackerConfig config)
        {
            var restClient = _serviceProvider?.GetRequiredService<IBitfinexRestClient>() ?? new BitfinexRestClient();
            var socketClient = _serviceProvider?.GetRequiredService<IBitfinexSocketClient>() ?? new BitfinexSocketClient();
            return new BitfinexUserSpotDataTracker(
                _serviceProvider?.GetRequiredService<ILogger<BitfinexUserSpotDataTracker>>() ?? new NullLogger<BitfinexUserSpotDataTracker>(),
                restClient,
                socketClient,
                null,
                config
                );
        }

        /// <inheritdoc />
        public IUserSpotDataTracker CreateUserSpotDataTracker(string userIdentifier, SpotUserDataTrackerConfig config, ApiCredentials credentials, BitfinexEnvironment? environment = null)
        {
            var clientProvider = _serviceProvider?.GetRequiredService<IBitfinexUserClientProvider>() ?? new BitfinexUserClientProvider();
            var restClient = clientProvider.GetRestClient(userIdentifier, credentials, environment);
            var socketClient = clientProvider.GetSocketClient(userIdentifier, credentials, environment);
            return new BitfinexUserSpotDataTracker(
                _serviceProvider?.GetRequiredService<ILogger<BitfinexUserSpotDataTracker>>() ?? new NullLogger<BitfinexUserSpotDataTracker>(),
                restClient,
                socketClient,
                userIdentifier,
                config
                );
        }
    }
}
