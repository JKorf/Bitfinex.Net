using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects.Options;
using CryptoExchange.Net.SharedApis;
using Microsoft.Extensions.Configuration;
using System;

namespace Bitfinex.Net.Objects.Options
{
    /// <summary>
    /// Bitfinex options
    /// </summary>
    public class BitfinexOptions : LibraryOptions<BitfinexRestOptions, BitfinexSocketOptions, BitfinexCredentials, BitfinexEnvironment>
    {
        /// <summary>
        /// Options for Shared API usage
        /// </summary>
        public SharedApiOptions SharedApi { get; set; } = new();

        /// <summary>Create options using the provided configuration action</summary>
        public static BitfinexOptions Create(Action<BitfinexOptions>? configure = null)
        {
            var options = CreateUnconfigured();
            configure?.Invoke(options);
            return Normalize(options);
        }

        /// <summary>Create options using the provided configuration</summary>
        public static BitfinexOptions CreateFromConfiguration(IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            var options = CreateUnconfigured();
            try { configuration.Bind(options); }
            catch (InvalidOperationException ex) { throw new InvalidOperationException("Invalid Bitfinex configuration provided", ex); }
            if (options.Rest?.Environment != null) options.Rest.Environment = BitfinexEnvironment.GetEnvironmentByName(options.Rest.Environment.Name) ?? options.Rest.Environment;
            if (options.Socket?.Environment != null) options.Socket.Environment = BitfinexEnvironment.GetEnvironmentByName(options.Socket.Environment.Name) ?? options.Socket.Environment;
            if (options.Environment != null) options.Environment = BitfinexEnvironment.GetEnvironmentByName(options.Environment.Name) ?? options.Environment;
            return Normalize(options);
        }

        private static BitfinexOptions CreateUnconfigured()
        {
            var options = new BitfinexOptions();
            options.Rest.Environment = null!;
            options.Socket.Environment = null!;
            return options;
        }

        private static BitfinexOptions Normalize(BitfinexOptions options)
        {
            if (options.Rest == null || options.Socket == null) throw new ArgumentException("Options null");
            options.Rest.Environment ??= options.Environment ?? BitfinexEnvironment.Live;
            options.Rest.ApiCredentials ??= options.ApiCredentials;
            options.Socket.Environment ??= options.Environment ?? BitfinexEnvironment.Live;
            options.Socket.ApiCredentials ??= options.ApiCredentials;
            return options;
        }
    }
}
