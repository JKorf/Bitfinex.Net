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
    }
}
