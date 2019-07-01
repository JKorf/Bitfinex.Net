using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.OrderBook;
using CryptoExchange.Net.Sockets;

namespace Bitfinex.Net
{
    public class BitfinexSymbolOrderBook: SymbolOrderBook
    {
        private readonly BitfinexSocketClient socketClient;
        private bool initialSnapshotDone;
        private readonly Precision precision;
        private readonly int limit;

        public BitfinexSymbolOrderBook(string symbol, Precision precisionLevel, int limit, LogVerbosity logVerbosity = LogVerbosity.Info, IEnumerable<TextWriter> logWriters = null) : base("Bitfinex", symbol, false, logVerbosity, logWriters)
        {
            socketClient = new BitfinexSocketClient();

            this.limit = limit;
            precision = precisionLevel;
        }

        protected override async Task<CallResult<UpdateSubscription>> DoStart()
        {
            if(precision == Precision.R0)
                return new CallResult<UpdateSubscription>(null, new ArgumentError("Invalid precision: R0"));

            var result = await socketClient.SubscribeToBookUpdatesAsync(Symbol, precision, Frequency.Realtime, limit, ProcessUpdate).ConfigureAwait(false);
            if (!result.Success)
                return result;

            Status = OrderBookStatus.Syncing;

            while (!initialSnapshotDone)
                await Task.Delay(10).ConfigureAwait(false); // Wait for first update to fill the order book

            return result;
        }

        protected override void DoReset()
        {
            initialSnapshotDone = false;
        }

        private void ProcessUpdate(BitfinexOrderBookEntry[] entries)
        {
            if (!initialSnapshotDone)
            {
                var asks = entries.Where(e => e.Quantity < 0).ToList();
                var bids = entries.Where(e => e.Quantity > 0).ToList();
                foreach (var entry in asks)
                    entry.Quantity = -entry.Quantity; // Bitfinex sends the asks as negative numbers, invert them
                
                SetInitialOrderBook(DateTime.UtcNow.Ticks, asks, bids);
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

        protected override async Task<CallResult<bool>> DoResync()
        {
            while (!initialSnapshotDone)
                await Task.Delay(10).ConfigureAwait(false); // Wait for first update to fill the order book

            return new CallResult<bool>(true, null);
        }

        public override void Dispose()
        {
            processBuffer.Clear();
            asks.Clear();
            bids.Clear();

            socketClient?.Dispose();
        }
    }
}
