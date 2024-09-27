using CryptoExchange.Net.SharedApis;

namespace Bitfinex.Net.Interfaces.Clients.SpotApi
{
    /// <summary>
    /// Shared interface for Spot socket API usage
    /// </summary>
    public interface IBitfinexSocketClientSpotApiShared :
        ITickerSocketClient,
        ITradeSocketClient,
        IBookTickerSocketClient,
        IBalanceSocketClient,
        ISpotOrderSocketClient,
        IKlineSocketClient,
        IUserTradeSocketClient
    {
    }
}
