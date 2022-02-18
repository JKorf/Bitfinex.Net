namespace Bitfinex.Net.Objects
{
    /// <summary>
    /// Api addresses usable for the Bitfinex clients
    /// </summary>
    public class BitfinexApiAddresses
    {
        /// <summary>
        /// The address used by the BitfinexClient for the rest API
        /// </summary>
        public string RestClientAddress { get; set; } = "";
        /// <summary>
        /// The address used by the BitfinexSocketClient for the socket API
        /// </summary>
        public string SocketClientAddress { get; set; } = "";

        /// <summary>
        /// The default addresses to connect to the Bitfinex.com API
        /// </summary>
        public static BitfinexApiAddresses Default = new BitfinexApiAddresses
        {
            RestClientAddress = "https://api.bitfinex.com",
            SocketClientAddress = "wss://api.bitfinex.com/ws/2"
        };
    }
}
