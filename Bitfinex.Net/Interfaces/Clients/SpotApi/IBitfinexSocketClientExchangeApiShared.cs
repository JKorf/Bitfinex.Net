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
        IUserTradeSocketClient,
        IFuturesOrderSocketClient,
        IPositionSocketClient,
        ISpotOrderManagementSocketClient,
        IFuturesOrderManagementSocketClient
    {
    }

    /// <summary>
    /// Shared API interface. Shared APIs provide a common,
    /// exchange-independent contract for accessing functionality across different
    /// exchange client libraries.
    /// </summary>
    public interface IBitfinexSocketClientExchangeSharedApi :
        ISubscribeTickerSocket,
        ISubscribeTradesSocket,
        ISubscribeBookTickerSocket,
        ISubscribeBalancesSocket,
        ISubscribeSpotOrdersSocket,
        ISubscribeKlinesSocket,
        ISubscribeUserTradesSocket,
        ISubscribeFuturesOrdersSocket,
        ISubscribePositionsSocket,
        IPlaceSpotOrderSocket,
        ICancelSpotOrderSocket,
        IPlaceFuturesOrderSocket,
        ICancelFuturesOrderSocket
    { }
}
