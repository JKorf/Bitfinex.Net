using Bitfinex.Net.Clients;
using Bitfinex.Net.Interfaces;
using Bitfinex.Net.Interfaces.Clients;
using Bitfinex.Net.Objects.Options;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.OrderBook;
using CryptoExchange.Net.SharedApis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Bitfinex.Net.SymbolOrderBooks
{
    /// <summary>
    /// Implementation for a synchronized order book. After calling Start the order book will sync itself and keep up to date with new data. It will automatically try to reconnect and resync in case of a lost/interrupted connection.
    /// Make sure to check the State property to see if the order book is synced.
    /// </summary>
    public class BitfinexSymbolOrderBook2 : CryptoExchange.Net.OrderBook.V2.ClientOrderBook
    {
        /// <summary>
        /// Create a new order book instance
        /// </summary>
        /// <param name="symbol">The symbol the order book is for</param>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public BitfinexSymbolOrderBook2(string baseAsset, string quoteAsset, Action<BitfinexOrderBookOptions>? optionsDelegate = null)
            : this(
                  new SharedSymbol(TradingMode.Spot, baseAsset, quoteAsset),
                  optionsDelegate,
                  null,
                  new BitfinexSharedApiClient(
                      new BitfinexRestClient(),
                      new BitfinexSocketClient(),
                      Options.Create(new BitfinexOptions())
                      ))
        {
        }

        /// <summary>
        /// Create a new order book instance
        /// </summary>
        /// <param name="symbol">The symbol the order book is for</param>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        /// <param name="logger">Logger</param>
        /// <param name="restClient">Rest client instance</param>
        /// <param name="socketClient">Socket client instance</param>
        public BitfinexSymbolOrderBook2(
            SharedSymbol symbol,
            Action<BitfinexOrderBookOptions>? optionsDelegate,
            ILoggerFactory? logger,
            IBitfinexSharedApiClient client) : base(
                logger, 
                symbol, 
                incrementalApi: client.Socket)
        {
            var options = BitfinexOrderBookOptions.Default.Copy();
            if (optionsDelegate != null)
                optionsDelegate(options);
            Initialize(options);

            _strictLevels = false;
        }
    }
}
