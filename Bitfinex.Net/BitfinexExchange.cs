using CryptoExchange.Net.Objects;
using CryptoExchange.Net.SharedApis;
using System;

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
            if (baseAsset == "USDT")
                baseAsset = "UST";

            if (quoteAsset == "USDT")
                quoteAsset = "UST";

            return $"t{baseAsset.ToUpperInvariant()}{quoteAsset.ToUpperInvariant()}";
        }
    }
}
