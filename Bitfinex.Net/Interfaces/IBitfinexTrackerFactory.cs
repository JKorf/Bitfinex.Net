using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.SharedApis;
using CryptoExchange.Net.Trackers.Klines;
using CryptoExchange.Net.Trackers.Trades;
using System;

namespace Bitfinex.Net.Interfaces
{
    /// <summary>
    /// Tracker factory
    /// </summary>
    public interface IBitfinexTrackerFactory: ITrackerFactory
    {
    }
}
