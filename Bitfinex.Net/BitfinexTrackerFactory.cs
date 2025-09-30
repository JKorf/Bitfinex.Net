using Bitfinex.Net.Clients;
using Bitfinex.Net.Interfaces;
using Bitfinex.Net.Interfaces.Clients;
using CryptoExchange.Net.SharedApis;
using CryptoExchange.Net.Trackers.Klines;
using CryptoExchange.Net.Trackers.Trades;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
    }
}
