using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bitfinex.Net.Interfaces;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.OrderBook;
using CryptoExchange.Net.Sockets;

namespace Bitfinex.Net
{
    /// <summary>
    /// Live order book implementation
    /// </summary>
    public class BitfinexSymbolOrderBook: SymbolOrderBook
    {
        private readonly IBitfinexSocketClient socketClient;
        private readonly Precision precision;
        private readonly int limit;

        /// <summary>
        /// Create a new order book instance
        /// </summary>
        /// <param name="symbol">The symbol the order book is for</param>
        /// <param name="precisionLevel">The precision level of the order book</param>
        /// <param name="limit">The limit of entries in the order book, either 25 or 100</param>
        /// <param name="options">Options for the order book</param>
        public BitfinexSymbolOrderBook(string symbol, Precision precisionLevel, int limit, BitfinexOrderBookOptions? options = null) : base(symbol, options ?? new BitfinexOrderBookOptions())
        {
            symbol.ValidateBitfinexSymbol();
            socketClient = options?.SocketClient ?? new BitfinexSocketClient();

            this.limit = limit;
            precision = precisionLevel;
        }

        /// <inheritdoc />
        protected override async Task<CallResult<UpdateSubscription>> DoStart()
        {
            if(precision == Precision.R0)
                throw new ArgumentException("Invalid precision: R0");

            var result = await socketClient.SubscribeToBookUpdatesAsync(Symbol, precision, Frequency.Realtime, limit, ProcessUpdate).ConfigureAwait(false);
            if (!result)
                return result;

            Status = OrderBookStatus.Syncing;

            var setResult = await WaitForSetOrderBook(10000).ConfigureAwait(false);
            return setResult ? result : new CallResult<UpdateSubscription>(null, setResult.Error);
        }

        /// <inheritdoc />
        protected override void DoReset()
        {
        }

        private void ProcessUpdate(IEnumerable<BitfinexOrderBookEntry> entries)
        {
            if (!bookSet)
            {
                var askEntries = entries.Where(e => e.Quantity < 0).ToList();
                var bidEntries = entries.Where(e => e.Quantity > 0).ToList();
                foreach (var entry in askEntries)
                    entry.Quantity = -entry.Quantity; // Bitfinex sends the asks as negative numbers, invert them
                
                SetInitialOrderBook(DateTime.UtcNow.Ticks, bidEntries, askEntries);
            }
            else
            {
                var askEntries = new List<ISymbolOrderBookEntry>();
                var bidEntries = new List<ISymbolOrderBookEntry>();
                foreach (var entry in entries)
                {
                    if (entry.Count == 0)
                    {
                        var bookEntry = new BitfinexOrderBookEntry() { Price = entry.Price, Quantity = 0 };
                        if (entry.Quantity == -1)
                            askEntries.Add(bookEntry);
                        else
                            bidEntries.Add(bookEntry);
                    }
                    else
                    {
                        if (entry.Quantity < 0)
                            askEntries.Add(new BitfinexOrderBookEntry() { Price = entry.Price, Quantity = -entry.Quantity });
                        else
                            bidEntries.Add(entry);
                    }
                }

                UpdateOrderBook(DateTime.UtcNow.Ticks, bidEntries, askEntries);
            }
        }

        /// <inheritdoc />
        protected override async Task<CallResult<bool>> DoResync()
        {
            return await WaitForSetOrderBook(10000).ConfigureAwait(false);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public override void Dispose()
        {
            processBuffer.Clear();
            asks.Clear();
            bids.Clear();

            socketClient?.Dispose();
        }
    }
}
