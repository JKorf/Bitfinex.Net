using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bitfinex.Net.Interfaces;
using Bitfinex.Net.Objects;
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
        private bool initialSnapshotDone;
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
                return new CallResult<UpdateSubscription>(null, new ArgumentError("Invalid precision: R0"));

            var result = await socketClient.SubscribeToBookUpdatesAsync(Symbol, precision, Frequency.Realtime, limit, ProcessUpdate).ConfigureAwait(false);
            if (!result)
                return result;

            Status = OrderBookStatus.Syncing;

            while (!initialSnapshotDone)
                await Task.Delay(10).ConfigureAwait(false); // Wait for first update to fill the order book

            return result;
        }

        /// <inheritdoc />
        protected override void DoReset()
        {
            initialSnapshotDone = false;
        }

        private void ProcessUpdate(IEnumerable<BitfinexOrderBookEntry> entries)
        {
            if (!initialSnapshotDone)
            {
                var askEntries = entries.Where(e => e.Quantity < 0).ToList();
                var bidEntries = entries.Where(e => e.Quantity > 0).ToList();
                foreach (var entry in askEntries)
                    entry.Quantity = -entry.Quantity; // Bitfinex sends the asks as negative numbers, invert them
                
                SetInitialOrderBook(DateTime.UtcNow.Ticks, askEntries, bidEntries);
                initialSnapshotDone = true;
            }
            else
            {
                var processEntries = new List<ProcessEntry>();
                foreach (var entry in entries)
                {
                    if (entry.Count == 0)
                    {
                        processEntries.Add(entry.Quantity == -1
                            ? new ProcessEntry(OrderBookEntryType.Ask, new OrderBookEntry(entry.Price, 0))
                            : new ProcessEntry(OrderBookEntryType.Bid, new OrderBookEntry(entry.Price, 0)));
                    }
                    else
                    {
                        processEntries.Add(entry.Quantity < 0
                            ? new ProcessEntry(OrderBookEntryType.Ask, new OrderBookEntry(entry.Price, -entry.Quantity))
                            : new ProcessEntry(OrderBookEntryType.Bid, entry));
                    }
                }

                UpdateOrderBook(DateTime.UtcNow.Ticks, DateTime.UtcNow.Ticks, processEntries);
            }
        }

        /// <inheritdoc />
        protected override async Task<CallResult<bool>> DoResync()
        {
            while (!initialSnapshotDone)
                await Task.Delay(10).ConfigureAwait(false); // Wait for first update to fill the order book

            return new CallResult<bool>(true, null);
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
