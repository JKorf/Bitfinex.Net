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
