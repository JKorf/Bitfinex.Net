using Bitfinex.Net.Clients;
using Bitfinex.Net.Interfaces.Clients;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;
using Bitfinex.Net.Interfaces;
using Bitfinex.Net.SymbolOrderBooks;

namespace Bitfinex.Net
{
    /// <summary>
    /// Helper functions
    /// </summary>
    public static class BitfinexHelpers
    {
        /// <summary>
        /// Add the IBitfinexClient and IBitfinexSocketClient to the sevice collection so they can be injected
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="defaultRestOptionsDelegate">Set default options for the rest client</param>
        /// <param name="defaultSocketOptionsDelegate">Set default options for the socket client</param>
        /// <param name="socketClientLifeTime">The lifetime of the IBitfinexSocketClient for the service collection. Defaults to Singleton.</param>
        /// <returns></returns>
        public static IServiceCollection AddBitfinex(
            this IServiceCollection services,
            Action<BitfinexRestOptions>? defaultRestOptionsDelegate = null,
            Action<BitfinexSocketOptions>? defaultSocketOptionsDelegate = null,
            ServiceLifetime? socketClientLifeTime = null)
        {
            var restOptions = BitfinexRestOptions.Default.Copy();

            if (defaultRestOptionsDelegate != null)
            {
                defaultRestOptionsDelegate(restOptions);
                BitfinexRestClient.SetDefaultOptions(defaultRestOptionsDelegate);
            }

            if (defaultSocketOptionsDelegate != null)
                BitfinexSocketClient.SetDefaultOptions(defaultSocketOptionsDelegate);

            services.AddHttpClient<IBitfinexRestClient, BitfinexRestClient>(options =>
            {
                options.Timeout = restOptions.RequestTimeout;
            }).ConfigurePrimaryHttpMessageHandler(() => {
                var handler = new HttpClientHandler();
                if (restOptions.Proxy != null)
                {
                    handler.Proxy = new WebProxy
                    {
                        Address = new Uri($"{restOptions.Proxy.Host}:{restOptions.Proxy.Port}"),
                        Credentials = restOptions.Proxy.Password == null ? null : new NetworkCredential(restOptions.Proxy.Login, restOptions.Proxy.Password)
                    };
                }
                return handler;
            });

            services.AddSingleton<IBitfinexOrderBookFactory, BitfinexOrderBookFactory>();
            services.AddTransient<IBitfinexRestClient, BitfinexRestClient>();
            if (socketClientLifeTime == null)
                services.AddSingleton<IBitfinexSocketClient, BitfinexSocketClient>();
            else
                services.Add(new ServiceDescriptor(typeof(IBitfinexSocketClient), typeof(BitfinexSocketClient), socketClientLifeTime.Value));
            return services;
        }

        /// <summary>
        /// Validate the string is a valid Bitfinex symbol.
        /// </summary>
        /// <param name="symbolString">string to validate</param>
        public static void ValidateBitfinexSymbol(this string symbolString)
        {
            if (string.IsNullOrEmpty(symbolString))
                throw new ArgumentException("Symbol is not provided");

            if (!Regex.IsMatch(symbolString, "^([t]([A-Z0-9|:]{6,}))$") && !Regex.IsMatch(symbolString, "^([f]([A-Z0-9]{3,}))$"))
                throw new ArgumentException($"{symbolString} is not a valid Bitfinex symbol. Should be [t][QuoteAsset][BaseAsset] for trading pairs " +
                     "or [f][Asset] for margin symbols, e.g. tBTCUSD or fUSD");
        }
    }
}
