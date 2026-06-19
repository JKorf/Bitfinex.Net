using CryptoExchange.Net.SharedApis;

namespace Bitfinex.Net.Interfaces.Clients.ExchangeApi
{
    /// <summary>
    /// Shared interface for socket API usage
    /// </summary>
    public interface IBitfinexSocketClientExchangeApiShared :
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
