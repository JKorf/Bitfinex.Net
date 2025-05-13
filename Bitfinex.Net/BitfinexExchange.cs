using Bitfinex.Net.Converters;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.RateLimiting;
using CryptoExchange.Net.RateLimiting.Filters;
using CryptoExchange.Net.RateLimiting.Guards;
using CryptoExchange.Net.RateLimiting.Interfaces;
using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bitfinex.Net
{
    /// <summary>
    /// Bitfinex exchange information and configuration
    /// </summary>
    public static class BitfinexExchange
    {
        /// <summary>
        /// Exchange name
        /// </summary>
        public static string ExchangeName => "Bitfinex";

        /// <summary>
        /// Exchange name
        /// </summary>
        public static string DisplayName => "Bitfinex";

        /// <summary>
        /// Url to exchange image
        /// </summary>
        public static string ImageUrl { get; } = "https://raw.githubusercontent.com/JKorf/Bitfinex.Net/master/Bitfinex.Net/Icon/icon.png";

        /// <summary>
        /// Url to the main website
        /// </summary>
        public static string Url { get; } = "https://www.bitfinex.com";

        /// <summary>
        /// Urls to the API documentation
        /// </summary>
        public static string[] ApiDocsUrl { get; } = new[] {
            "https://docs.bitfinex.com/docs/introduction"
            };

        /// <summary>
        /// Type of exchange
        /// </summary>
        public static ExchangeType Type { get; } = ExchangeType.CEX;

        internal static JsonSerializerContext _serializerContext = JsonSerializerContextCache.GetOrCreate<BitfinexSourceGenerationContext>();

        /// <summary>
        /// Format a base and quote asset to a Bitfinex recognized symbol 
        /// </summary>
        /// <param name="baseAsset">Base asset</param>
        /// <param name="quoteAsset">Quote asset</param>
        /// <param name="tradingMode">Trading mode</param>
        /// <param name="deliverTime">Delivery time for delivery futures</param>
        /// <returns></returns>
        public static string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverTime = null)
        {
            baseAsset = AssetAliases.CommonToExchangeName(baseAsset);
            quoteAsset = AssetAliases.CommonToExchangeName(quoteAsset);

            if (baseAsset.Length != 3)
                return $"t{baseAsset.ToUpperInvariant()}:{quoteAsset.ToUpperInvariant()}";

            return $"t{baseAsset.ToUpperInvariant()}{quoteAsset.ToUpperInvariant()}";
        }

        /// <summary>
        /// Aliases for Bitfinex assets
        /// </summary>
        public static AssetAliasConfiguration AssetAliases { get; } = new AssetAliasConfiguration
        {
            Aliases = [new AssetAlias("UST", "USDT")]
        };

        /// <summary>
        /// Rate limiter configuration for the Bitfinex API
        /// </summary>
        public static BitfinexRateLimiters RateLimiter { get; } = new BitfinexRateLimiters();

    }

    /// <summary>
    /// Rate limiter configuration for the GateIo API
    /// </summary>
    public class BitfinexRateLimiters
    {
        /// <summary>
        /// Event for when a rate limit is triggered
        /// </summary>
        public event Action<RateLimitEvent> RateLimitTriggered;

        /// <summary>
        /// Event when the rate limit is updated. Note that it's only updated when a request is send, so there are no specific updates when the current usage is decaying.
        /// </summary>
        public event Action<RateLimitUpdateEvent> RateLimitUpdated;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        internal BitfinexRateLimiters()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Initialize();
        }

        private void Initialize()
        {
            Overall = new RateLimitGate("Overall");
            RestConf = new RateLimitGate("Rest Config")
                .AddGuard(new RateLimitGuard(RateLimitGuard.PerHost, [], 90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding)); // 90 requests per minute shared by all /conf endpoints
            RestStats = new RateLimitGate("Rest Stats")
                .AddGuard(new RateLimitGuard(RateLimitGuard.PerHost, [], 15, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding)); // 15 requests per minute shared by all /stats endpoints
            Websocket = new RateLimitGate("Websocket")
                                    .AddGuard(new RateLimitGuard(RateLimitGuard.PerHost, [new HostFilter("wss://api.bitfinex.com"), new LimitItemTypeFilter(RateLimitItemType.Connection)], 5, TimeSpan.FromSeconds(15), RateLimitWindowType.Sliding)) // Limit of 5 connection requests per 15 seconds
                                    .AddGuard(new RateLimitGuard(RateLimitGuard.PerHost, [new HostFilter("wss://api-pub.bitfinex.com"), new LimitItemTypeFilter(RateLimitItemType.Connection)], 20, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding)); // Limit of 20 connection requests per 60 seconds

            Overall.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);
            Overall.RateLimitUpdated += (x) => RateLimitUpdated?.Invoke(x);
            RestConf.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);
            RestConf.RateLimitUpdated += (x) => RateLimitUpdated?.Invoke(x);
            RestStats.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);
            RestStats.RateLimitUpdated += (x) => RateLimitUpdated?.Invoke(x);
            Websocket.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);
            Websocket.RateLimitUpdated += (x) => RateLimitUpdated?.Invoke(x);
        }


        internal IRateLimitGate Overall { get; private set; }
        internal IRateLimitGate RestConf { get; private set; }
        internal IRateLimitGate RestStats { get; private set; }
        internal IRateLimitGate Websocket { get; private set; }
    }
}
