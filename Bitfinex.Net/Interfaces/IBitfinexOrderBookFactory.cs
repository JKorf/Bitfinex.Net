﻿using Bitfinex.Net.Objects.Options;
using CryptoExchange.Net.Interfaces;
using System;

namespace Bitfinex.Net.Interfaces
{
    /// <summary>
    /// Bitfinex order book factory
    /// </summary>
    public interface IBitfinexOrderBookFactory
    {
        /// <summary>
        /// Spot order book factory methods
        /// </summary>
        public IOrderBookFactory<BitfinexOrderBookOptions> Spot { get; }

        /// <summary>
        /// Create a SymbolOrderBook
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="options">Book options</param>
        /// <returns></returns>
        ISymbolOrderBook Create(string symbol, Action<BitfinexOrderBookOptions>? options = null);
    }
}
