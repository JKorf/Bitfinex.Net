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
        /// ctor for DI, use <see cref="CreateCustom"/> for creating a custom environment
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public BitfinexEnvironment() : base(TradeEnvironmentNames.Live)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        { }

        /// <summary>
        /// Get the Bitfinex environment by name
        /// </summary>
        public static BitfinexEnvironment? GetEnvironmentByName(string? name)
         => name switch
         {
             TradeEnvironmentNames.Live => Live,
             "" => Live,
             null => Live,
             _ => default
         };

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
