using Bitfinex.Net;
using Bitfinex.Net.Clients;
using Bitfinex.Net.Interfaces;
using Bitfinex.Net.Interfaces.Clients;
using Bitfinex.Net.Objects.Options;
using Bitfinex.Net.SymbolOrderBooks;
using CryptoExchange.Net;
using CryptoExchange.Net.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions for DI
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add services such as the IBitfinexRestClient and IBitfinexSocketClient. Configures the services based on the provided configuration.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration(section) containing the options</param>
        /// <returns></returns>
        public static IServiceCollection AddBitfinex(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var options = new BitfinexOptions();
            // Reset environment so we know if they're overridden
            options.Rest.Environment = null!;
            options.Socket.Environment = null!;
            configuration.Bind(options);

            if (options.Rest == null || options.Socket == null)
                throw new ArgumentException("Options null");

            var restEnvName = options.Rest.Environment?.Name ?? options.Environment?.Name ?? BitfinexEnvironment.Live.Name;
            var socketEnvName = options.Socket.Environment?.Name ?? options.Environment?.Name ?? BitfinexEnvironment.Live.Name;
            options.Rest.Environment = BitfinexEnvironment.GetEnvironmentByName(restEnvName) ?? options.Rest.Environment!;
            options.Rest.ApiCredentials = options.Rest.ApiCredentials ?? options.ApiCredentials;
            options.Socket.Environment = BitfinexEnvironment.GetEnvironmentByName(socketEnvName) ?? options.Socket.Environment!;
            options.Socket.ApiCredentials = options.Socket.ApiCredentials ?? options.ApiCredentials;


            services.AddSingleton(x => Options.Options.Create(options.Rest));
            services.AddSingleton(x => Options.Options.Create(options.Socket));

            return AddBitfinexCore(services, options.SocketClientLifeTime);
        }

        /// <summary>
        /// Add services such as the IBitfinexRestClient and IBitfinexSocketClient. Services will be configured based on the provided options.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="optionsDelegate">Set options for the Bitfinex services</param>
        /// <returns></returns>
        public static IServiceCollection AddBitfinex(
            this IServiceCollection services,
            Action<BitfinexOptions>? optionsDelegate = null)
        {
            var options = new BitfinexOptions();
            // Reset environment so we know if they're overridden
            options.Rest.Environment = null!;
            options.Socket.Environment = null!;
            optionsDelegate?.Invoke(options);
            if (options.Rest == null || options.Socket == null)
                throw new ArgumentException("Options null");

            options.Rest.Environment = options.Rest.Environment ?? options.Environment ?? BitfinexEnvironment.Live;
            options.Rest.ApiCredentials = options.Rest.ApiCredentials ?? options.ApiCredentials;
            options.Socket.Environment = options.Socket.Environment ?? options.Environment ?? BitfinexEnvironment.Live;
            options.Socket.ApiCredentials = options.Socket.ApiCredentials ?? options.ApiCredentials;

            services.AddSingleton(x => Options.Options.Create(options.Rest));
            services.AddSingleton(x => Options.Options.Create(options.Socket));

            return AddBitfinexCore(services, options.SocketClientLifeTime);
        }

        private static IServiceCollection AddBitfinexCore(
            this IServiceCollection services,
            ServiceLifetime? socketClientLifeTime = null)
        {
            services.AddHttpClient<IBitfinexRestClient, BitfinexRestClient>((client, serviceProvider) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<BitfinexRestOptions>>().Value;
                client.Timeout = options.RequestTimeout;
                return new BitfinexRestClient(client, serviceProvider.GetRequiredService<ILoggerFactory>(), serviceProvider.GetRequiredService<IOptions<BitfinexRestOptions>>());
            }).ConfigurePrimaryHttpMessageHandler((serviceProvider) => {
                var options = serviceProvider.GetRequiredService<IOptions<BitfinexRestOptions>>().Value;
                return LibraryHelpers.CreateHttpClientMessageHandler(options.Proxy, options.HttpKeepAliveInterval);
            });
            services.Add(new ServiceDescriptor(typeof(IBitfinexSocketClient), x => { return new BitfinexSocketClient(x.GetRequiredService<IOptions<BitfinexSocketOptions>>(), x.GetRequiredService<ILoggerFactory>()); }, socketClientLifeTime ?? ServiceLifetime.Singleton));

            services.AddTransient<IBitfinexOrderBookFactory, BitfinexOrderBookFactory>();
            services.AddTransient<IBitfinexTrackerFactory, BitfinexTrackerFactory>();
            services.AddTransient<ITrackerFactory, BitfinexTrackerFactory>();
            services.AddSingleton<IBitfinexUserClientProvider, BitfinexUserClientProvider>(x =>
            new BitfinexUserClientProvider(
                x.GetRequiredService<HttpClient>(),
                x.GetRequiredService<ILoggerFactory>(),
                x.GetRequiredService<IOptions<BitfinexRestOptions>>(),
                x.GetRequiredService<IOptions<BitfinexSocketOptions>>()));

            services.RegisterSharedRestInterfaces(x => x.GetRequiredService<IBitfinexRestClient>().SpotApi.SharedClient);
            services.RegisterSharedSocketInterfaces(x => x.GetRequiredService<IBitfinexSocketClient>().SpotApi.SharedClient);
            
            return services;
        }
    }
}
