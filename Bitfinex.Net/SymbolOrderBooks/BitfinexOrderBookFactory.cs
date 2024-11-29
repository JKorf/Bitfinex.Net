using Bitfinex.Net.Interfaces;
using Bitfinex.Net.Interfaces.Clients;
using Bitfinex.Net.Objects.Options;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.OrderBook;
using CryptoExchange.Net.SharedApis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Bitfinex.Net.SymbolOrderBooks
{
    /// <summary>
    /// Bitfinex order book factory
    /// </summary>
    public class BitfinexOrderBookFactory : IBitfinexOrderBookFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="serviceProvider">Service provider for resolving logging and clients</param>
        public BitfinexOrderBookFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            Spot = new OrderBookFactory<BitfinexOrderBookOptions>(Create, Create);
        }

        /// <inheritdoc />
        public IOrderBookFactory<BitfinexOrderBookOptions> Spot { get; }

        /// <inheritdoc />
        public ISymbolOrderBook Create(SharedSymbol symbol, Action<BitfinexOrderBookOptions>? options = null)
        {       
            var symbolName = symbol.GetSymbol(BitfinexExchange.FormatSymbol);
            return Create(symbolName, options);
        }

        /// <inheritdoc />
        public ISymbolOrderBook Create(string symbol, Action<BitfinexOrderBookOptions>? options = null)
            => new BitfinexSymbolOrderBook(symbol,
                                             options,
                                             _serviceProvider.GetRequiredService<ILoggerFactory>(),
                                             _serviceProvider.GetRequiredService<IBitfinexSocketClient>());
    }
}
