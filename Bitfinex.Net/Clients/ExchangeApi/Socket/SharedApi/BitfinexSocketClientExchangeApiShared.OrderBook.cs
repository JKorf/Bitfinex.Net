using Bitfinex.Net.Enums;
using Bitfinex.Net.Interfaces.Clients.ExchangeApi;
using Bitfinex.Net.Objects.Models;
using CryptoExchange.Net;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.SharedApis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients.ExchangeApi
{
    internal partial class BitfinexSocketClientExchangeSharedApi
    {
        #region Subscribe To Incremental Order Book Updates

        public SubscribeIncrementalOrderBookOptions SubscribeIncrementalOrderBookOptions { get; }
            = new SubscribeIncrementalOrderBookOptions(_exchangeName, false, [25, 100], SharedOrderBookSubscriptionType.SnapshotThenIncremental)
            {
            };
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToIncrementalOrderBookUpdatesAsync
            (SubscribeOrderBookRequest request, Action<DataEvent<SharedIncrementalOrderBook>> handler, CancellationToken ct)
        {
            var validationError = SubscribeIncrementalOrderBookOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbol = request.SymbolName(FormatSymbol);
            var limit = request.Limit ?? 25;
            var result = await _api.SubscribeToOrderBookUpdatesAsync(symbol, Precision.PrecisionLevel0, Frequency.Realtime, limit, update => 
            {
                var entries = update.Data;
                if (update.UpdateType == SocketUpdateType.Snapshot)
                {
                    var bidEntries = new BitfinexOrderBookEntry[limit];
                    var askEntries = new BitfinexOrderBookEntry[limit];
                    var offset = 0;
                    for (var i = 0; i < entries.Length; i++)
                    {
                        var item = entries[i];
                        if (item.Quantity > 0)
                        {
                            bidEntries[i - offset] = (item);
                        }
                        else
                        {
                            if (offset == 0)
                                offset = i;

                            askEntries[i - offset] = item with { Quantity = -item.Quantity };
                        }
                    }

                    handler(
                        update.ToType(
                            new SharedIncrementalOrderBook(
                                SharedQuantityType.BaseAsset,
                                update.SequenceNumber,
                                update.SequenceNumber,
                                askEntries,
                                bidEntries)));
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
                            {
                                bidEntries.Add(entry);
                            }
                        }
                    }

                    handler(
                        update.ToType(
                            new SharedIncrementalOrderBook(SharedQuantityType.BaseAsset, update.SequenceNumber, update.SequenceNumber, askEntries.ToArray(), bidEntries.ToArray())));
                }
            }, ct: ct).ConfigureAwait(false);

            return result;
        }

        #endregion
    }
}
