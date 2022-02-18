using Bitfinex.Net.Clients;
using Bitfinex.Net.Interfaces.Clients;
using Bitfinex.Net.Objects;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.RegularExpressions;

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
        /// <param name="defaultOptionsCallback">Set default options for the client</param>
        /// <param name="socketClientLifeTime">The lifetime of the IBitfinexSocketClient for the service collection. Defaults to Scoped.</param>
        /// <returns></returns>
        public static IServiceCollection AddBitfinex(
            this IServiceCollection services, 
            Action<BitfinexClientOptions, BitfinexSocketClientOptions>? defaultOptionsCallback = null,
            ServiceLifetime? socketClientLifeTime = null)
        {
            if (defaultOptionsCallback != null)
            {
                var options = new BitfinexClientOptions();
                var socketOptions = new BitfinexSocketClientOptions();
                defaultOptionsCallback?.Invoke(options, socketOptions);

                BitfinexClient.SetDefaultOptions(options);
                BitfinexSocketClient.SetDefaultOptions(socketOptions);
            }

            services.AddTransient<IBitfinexClient, BitfinexClient>();
            if (socketClientLifeTime == null)
                services.AddScoped<IBitfinexSocketClient, BitfinexSocketClient>();
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
