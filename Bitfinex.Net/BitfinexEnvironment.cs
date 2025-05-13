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

        /// <summary>
        /// Socket client address
        /// </summary>
        public string SocketPublicAddress { get; }

        internal BitfinexEnvironment(string name, string restAddress, string socketAddress, string socketPublicAddress) :
            base(name)
        {
            RestAddress = restAddress;
            SocketAddress = socketAddress;
            SocketPublicAddress = socketPublicAddress;
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
        /// Available environment names
        /// </summary>
        /// <returns></returns>
        public static string[] All => [Live.Name];

        /// <summary>
        /// Live environment
        /// </summary>
        public static BitfinexEnvironment Live { get; }
            = new BitfinexEnvironment(TradeEnvironmentNames.Live,
                                     BitfinexApiAddresses.Default.RestClientAddress,
                                     BitfinexApiAddresses.Default.SocketClientAddress,
                                     BitfinexApiAddresses.Default.SocketClientPublicAddress);

        /// <summary>
        /// Create a custom environment
        /// </summary>
        public static BitfinexEnvironment CreateCustom(
                        string name,
                        string restAddress,
                        string socketAddress,
                        string socketPublicAddress)
            => new BitfinexEnvironment(name, restAddress, socketAddress, socketPublicAddress);

    }
}
