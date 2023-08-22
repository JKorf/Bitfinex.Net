using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Clients;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Interfaces.Clients;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Options;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.OrderBook;
using CryptoExchange.Net.Sockets;
using Force.Crc32;
using Microsoft.Extensions.Logging;

namespace Bitfinex.Net.SymbolOrderBooks
{
    /// <summary>
    /// Live order book implementation
    /// </summary>
    public class BitfinexSymbolOrderBook: SymbolOrderBook
    {
        private readonly IBitfinexSocketClient _socketClient;
        private readonly Precision _precision;
        private readonly TimeSpan _initialDataTimeout;
        private bool _initial = true;
        private readonly bool _clientOwner;

        /// <summary>
        /// Create a new order book instance
        /// </summary>
        /// <param name="symbol">The symbol the order book is for</param>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public BitfinexSymbolOrderBook(string symbol, Action<BitfinexOrderBookOptions>? optionsDelegate = null)
            : this(symbol, optionsDelegate, null, null)
        {
            _clientOwner = true;
        }

        /// <summary>
        /// Create a new order book instance
        /// </summary>
        /// <param name="symbol">The symbol the order book is for</param>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        /// <param name="logger">Logger</param>
        /// <param name="socketClient">Socket client instance</param>
        public BitfinexSymbolOrderBook(string symbol, 
            Action<BitfinexOrderBookOptions>? optionsDelegate, 
            ILogger<BitfinexSymbolOrderBook>? logger,
            IBitfinexSocketClient? socketClient) : base(logger, "Bitfinex", symbol)
        {
            symbol.ValidateBitfinexSymbol();

            var options = BitfinexOrderBookOptions.Default.Copy();
            if (optionsDelegate != null)
                optionsDelegate(options);
            Initialize(options);

            _socketClient = socketClient ?? new BitfinexSocketClient();
            _clientOwner = socketClient == null;
            _initialDataTimeout = options?.InitialDataTimeout ?? TimeSpan.FromSeconds(30);

            Levels = options?.Limit ?? 25;
            _precision = options?.Precision ?? Precision.PrecisionLevel0;
        }

        /// <inheritdoc />
        protected override async Task<CallResult<UpdateSubscription>> DoStartAsync(CancellationToken ct)
        {
            if(_precision == Precision.R0)
                throw new ArgumentException("Invalid precision: R0");

            var result = await _socketClient.SpotApi.SubscribeToOrderBookUpdatesAsync(Symbol, _precision, Frequency.Realtime, Levels!.Value, ProcessUpdate, ProcessChecksum).ConfigureAwait(false);
            if (!result)
                return result;

            if (ct.IsCancellationRequested)
            {
                await result.Data.CloseAsync().ConfigureAwait(false);
                return result.AsError<UpdateSubscription>(new CancellationRequestedError());
            }

            Status = OrderBookStatus.Syncing;
            
            var setResult = await WaitForSetOrderBookAsync(_initialDataTimeout, ct).ConfigureAwait(false);
            return setResult ? result : new CallResult<UpdateSubscription>(setResult.Error!);
        }

        /// <inheritdoc />
        protected override void DoReset()
        {
            _initial = true;
        }

        private void ProcessUpdate(DataEvent<IEnumerable<BitfinexOrderBookEntry>> data)
        {
            var entries = data.Data;
            if (_initial)
            {
                _initial = false;
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
                        if (entry.Quantity == -1)
                        {
                            entry.Quantity = 0;
                            askEntries.Add(entry);
                        }
                        else
                        {
                            entry.Quantity = 0;
                            bidEntries.Add(entry);
                        }
                    }
                    else
                    {
                        if (entry.Quantity < 0)
                        {
                            entry.Quantity *= -1;
                            askEntries.Add(entry);
                        }
                        else
                            bidEntries.Add(entry);
                    }
                }

                UpdateOrderBook(DateTime.UtcNow.Ticks, bidEntries, askEntries);
            }
        }

        /// <summary>
        /// Process a received checksum
        /// </summary>
        /// <param name="checksum"></param>
        protected void ProcessChecksum(DataEvent<int> checksum)
        {
            AddChecksum(checksum.Data);            
        }

        /// <summary>
        /// Process a checksum
        /// </summary>
        /// <param name="checksum"></param>
        /// <returns></returns>
        protected override bool DoChecksum(int checksum)
        {
            if (LastSequenceNumber == 0)
                return true; // No data yet?

            var checksumValues = new List<string>();
            for (var i = 0; i < 25; i++)
            {
                if (_bids.Count > i)
                {
                    var bid = (BitfinexOrderBookEntry)_bids.ElementAt(i).Value;
                    checksumValues.Add(bid.RawPrice);
                    checksumValues.Add(bid.RawQuantity);
                }
                else
                    _logger.Log(LogLevel.Trace, $"Skipping checksum bid level {i}, no data");

                if (_asks.Count > i)
                {
                    var ask = (BitfinexOrderBookEntry)_asks.ElementAt(i).Value;
                    checksumValues.Add(ask.RawPrice);
                    checksumValues.Add(ask.RawQuantity);
                }
                else
                    _logger.Log(LogLevel.Trace, $"Skipping checksum ask level {i}, no data");
            }
            var checksumString = string.Join(":", checksumValues);
            var ourChecksumUtf = (int)Crc32Algorithm.Compute(Encoding.UTF8.GetBytes(checksumString));

            if (ourChecksumUtf != checksum)
            {
                _logger.Log(LogLevel.Warning, $"{Symbol} Invalid checksum. Received from server: {checksum}, calculated local: {ourChecksumUtf}");
                return false;
            }
            
            return true;            
        }

        /// <inheritdoc />
        protected override async Task<CallResult<bool>> DoResyncAsync(CancellationToken ct)
        {
            return await WaitForSetOrderBookAsync(_initialDataTimeout, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if(_clientOwner)
                _socketClient?.Dispose();

            base.Dispose(disposing);
        }
    }
}
