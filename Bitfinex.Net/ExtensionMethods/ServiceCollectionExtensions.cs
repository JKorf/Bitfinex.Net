using Bitfinex.Net;
using Bitfinex.Net.Clients;
using Bitfinex.Net.Interfaces;
using Bitfinex.Net.Interfaces.Clients;
using Bitfinex.Net.Objects.Options;
using Bitfinex.Net.SymbolOrderBooks;
using CryptoExchange.Net;
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
            // Reset environment so we know if theyre overriden
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
            // Reset environment so we know if theyre overriden
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

        /// <summary>
        /// DEPRECATED; use <see cref="AddBitfinex(IServiceCollection, Action{BitfinexOptions}?)" /> instead
        /// </summary>
        public static IServiceCollection AddBitfinex(
            this IServiceCollection services,
            Action<BitfinexRestOptions> restDelegate,
            Action<BitfinexSocketOptions>? socketDelegate = null,
            ServiceLifetime? socketClientLifeTime = null)
        {
            services.Configure<BitfinexRestOptions>((x) => { restDelegate?.Invoke(x); });
            services.Configure<BitfinexSocketOptions>((x) => { socketDelegate?.Invoke(x); });

            return AddBitfinexCore(services, socketClientLifeTime);
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
                var handler = new HttpClientHandler();
                try
                {
                    handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                    handler.DefaultProxyCredentials = CredentialCache.DefaultCredentials;
                }
                catch (PlatformNotSupportedException) { }
                catch (NotImplementedException) { } // Mono runtime throws NotImplementedException for DefaultProxyCredentials setting

                var options = serviceProvider.GetRequiredService<IOptions<BitfinexRestOptions>>().Value;
                if (options.Proxy != null)
                {
                    handler.Proxy = new WebProxy
                    {
                        Address = new Uri($"{options.Proxy.Host}:{options.Proxy.Port}"),
                        Credentials = options.Proxy.Password == null ? null : new NetworkCredential(options.Proxy.Login, options.Proxy.Password)
                    };
                }
                return handler;
            });
            services.Add(new ServiceDescriptor(typeof(IBitfinexSocketClient), x => { return new BitfinexSocketClient(x.GetRequiredService<IOptions<BitfinexSocketOptions>>(), x.GetRequiredService<ILoggerFactory>()); }, socketClientLifeTime ?? ServiceLifetime.Singleton));

            services.AddTransient<IBitfinexOrderBookFactory, BitfinexOrderBookFactory>();
            services.AddTransient<IBitfinexTrackerFactory, BitfinexTrackerFactory>();

            services.RegisterSharedRestInterfaces(x => x.GetRequiredService<IBitfinexRestClient>().SpotApi.SharedClient);
            services.RegisterSharedSocketInterfaces(x => x.GetRequiredService<IBitfinexSocketClient>().SpotApi.SharedClient);
            
            return services;
        }
    }
}
