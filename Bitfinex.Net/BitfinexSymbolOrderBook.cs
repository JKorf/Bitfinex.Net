using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitfinex.Net.Interfaces;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.OrderBook;
using CryptoExchange.Net.Sockets;
using Force.Crc32;

namespace Bitfinex.Net
{
    /// <summary>
    /// Live order book implementation
    /// </summary>
    public class BitfinexSymbolOrderBook: SymbolOrderBook
    {
        private readonly IBitfinexSocketClient socketClient;
        private readonly Precision precision;

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

            Levels = limit;
            precision = precisionLevel;
        }

        /// <inheritdoc />
        protected override async Task<CallResult<UpdateSubscription>> DoStart()
        {
            if(precision == Precision.R0)
                throw new ArgumentException("Invalid precision: R0");


            var result = await socketClient.SubscribeToBookUpdatesAsync(Symbol, precision, Frequency.Realtime, Levels!.Value, ProcessUpdate, ProcessChecksum).ConfigureAwait(false);
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
        protected void ProcessChecksum(int checksum)
        {
            AddChecksum(checksum);            
        }

        /// <summary>
        /// Process a checksum
        /// </summary>
        /// <param name="checksum"></param>
        /// <returns></returns>
        protected override bool DoChecksum(int checksum)
        {
            var checksumValues = new List<string>();
            for (var i = 0; i < 25; i++)
            {
                if (bids.Count >= i)
                {
                    var bid = (BitfinexOrderBookEntry)bids.ElementAt(i).Value;
                    checksumValues.Add(bid.RawPrice);
                    checksumValues.Add(bid.RawQuantity);
                }
                if (asks.Count >= i)
                {
                    var ask = (BitfinexOrderBookEntry)asks.ElementAt(i).Value;
                    checksumValues.Add(ask.RawPrice);
                    checksumValues.Add(ask.RawQuantity);
                }
            }
            var checksumString = string.Join(":", checksumValues);
            var ourChecksumUtf = (int)Crc32Algorithm.Compute(Encoding.UTF8.GetBytes(checksumString));

            if (ourChecksumUtf != checksum)
            {
                log.Write(CryptoExchange.Net.Logging.LogVerbosity.Warning, $"Invalid checksum. Received from server: {checksum}, calculated local: {ourChecksumUtf}");
                return false;
            }
            
            return true;            
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

        // Format the value with the indicated number of significant digits.
        private string ToSignificantDigits(decimal value)
        {
            var stringValue = value.ToString(CultureInfo.InvariantCulture);
            var stringLength = stringValue.Length;
            if (stringValue.Contains('.'))
                stringLength -= 1;

            for (var i = 0;i < 5 - stringLength; i++)
            {
                if (!stringValue.Contains('.'))
                    stringValue += ".0";
                else
                    stringValue += "0";
            }

            return stringValue;
        }
    }
}
