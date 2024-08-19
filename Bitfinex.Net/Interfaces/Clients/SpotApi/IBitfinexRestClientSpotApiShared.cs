using CryptoExchange.Net.SharedApis.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bitfinex.Net.Interfaces.Clients.SpotApi
{
    public interface IBitfinexRestClientSpotApiShared :
        IAssetRestClient,
        IBalanceRestClient,
        //IDepositRestClient,
        IKlineRestClient,
        //IOrderBookRestClient,
        IRecentTradeRestClient,
        ISpotOrderRestClient,
        ISpotSymbolRestClient,
        ITickerRestClient
        //ITradeHistoryRestClient,
        //IWithdrawalRestClient,
        //IWithdrawRestClient
    {
    }
}
