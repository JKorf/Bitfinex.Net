using Bitfinex.Net.Clients;
using Bitfinex.Net.Interfaces.Clients;

namespace CryptoExchange.Net.Interfaces
{
    /// <summary>
    /// Extensions for the ICryptoRestClient and ICryptoSocketClient interfaces
    /// </summary>
    public static class CryptoClientExtensions
    {
        /// <summary>
        /// Get the Bitfinex REST Api client
        /// </summary>
        /// <param name="baseClient"></param>
        /// <returns></returns>
        public static IBitfinexRestClient Bitfinex(this ICryptoRestClient baseClient) => baseClient.TryGet<IBitfinexRestClient>(() => new BitfinexRestClient());

        /// <summary>
        /// Get the Bitfinex Websocket Api client
        /// </summary>
        /// <param name="baseClient"></param>
        /// <returns></returns>
        public static IBitfinexSocketClient Bitfinex(this ICryptoSocketClient baseClient) => baseClient.TryGet<IBitfinexSocketClient>(() => new BitfinexSocketClient());
    }
}
