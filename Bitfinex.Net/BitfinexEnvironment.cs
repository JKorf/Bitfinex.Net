using Bitfinex.Net.Objects;
using CryptoExchange.Net.Objects;

namespace Bitfinex.Net
{
    /// <summary>
    /// Bitfinex environments
    /// </summary>
    public class BitfinexEnvironment : TradeEnvironment
    {
        /// <summary>
        /// Rest client address
        /// </summary>
        public string RestAddress { get; }

        /// <summary>
        /// Socket client address
        /// </summary>
        public string SocketAddress { get; }

        internal BitfinexEnvironment(string name, string restAddress, string socketAddress) :
            base(name)
        {
            RestAddress = restAddress;
            SocketAddress = socketAddress;
        }

        /// <summary>
        /// Live environment
        /// </summary>
        public static BitfinexEnvironment Live { get; }
            = new BitfinexEnvironment(TradeEnvironmentNames.Live,
                                     BitfinexApiAddresses.Default.RestClientAddress,
                                     BitfinexApiAddresses.Default.SocketClientAddress);

        /// <summary>
        /// Create a custom environment
        /// </summary>
        /// <param name="name"></param>
        /// <param name="restAddress"></param>
        /// <param name="socketAddress"></param>
        /// <returns></returns>
        public static BitfinexEnvironment CreateCustom(
                        string name,
                        string restAddress,
                        string socketAddress)
            => new BitfinexEnvironment(name, restAddress, socketAddress);

    }
}
