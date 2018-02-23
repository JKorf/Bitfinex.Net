using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.SocketObjects2;
using Bitfinex.Net.Objects.SocketObjets;

namespace Bitfinex.Net
{
    public partial class BitfinexSocketClient2
    {
        public async Task SubscribeToTicker(string symbol, Action<BitfinexSocketTradingPairTick[]> handler)
        {
            await SubscribeAndWait(new TickerSubscriptionRequest(symbol, handler));
        }

        public async Task SubscribeToTrades(string symbol, Action<BitfinexTradeSimple[]> handler)
        {
            await SubscribeAndWait(new TradesSubscriptionRequest(symbol, handler));
        }

        public async Task SubscribeToBook(string symbol, string precision, string frequency, int limit, Action<BitfinexOrderBookEntry[]> handler)
        {
            await SubscribeAndWait(new BookSubscriptionRequest(symbol, precision, frequency, limit, handler));
        }

        public async Task SubscribeToCandles(string symbol, string interval, Action<BitfinexCandle[]> handler)
        {
            await SubscribeAndWait(new CandleSubscriptionRequest(symbol, interval, handler));
        }
    }
}
