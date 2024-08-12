using Bitfinex.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.SharedApis.Interfaces.Socket;
using CryptoExchange.Net.SharedApis.Models.Socket;
using CryptoExchange.Net.SharedApis.RequestModels;
using CryptoExchange.Net.SharedApis.ResponseModels;
using CryptoExchange.Net.SharedApis.SubscribeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients.SpotApi
{
    internal partial class BitfinexSocketClientSpotApi : IBitfinexSocketClientSpotApiShared
    {
        public string Exchange => BitfinexExchange.ExchangeName;

        async Task<CallResult<UpdateSubscription>> ITickerSocketClient.SubscribeToTickerUpdatesAsync(TickerSubscribeRequest request, Action<DataEvent<SharedTicker>> handler, CancellationToken ct)
        {
            var symbol = FormatSymbol(request.BaseAsset, request.QuoteAsset, request.ApiType);
            var result = await SubscribeToTickerUpdatesAsync(symbol, update => handler(update.As(new SharedTicker(symbol, update.Data.LastPrice, update.Data.HighPrice, update.Data.LowPrice))), ct).ConfigureAwait(false);

            return result;
        }

        async Task<CallResult<UpdateSubscription>> ITradeSocketClient.SubscribeToTradeUpdatesAsync(TradeSubscribeRequest request, Action<DataEvent<IEnumerable<SharedTrade>>> handler, CancellationToken ct)
        {
            var symbol = FormatSymbol(request.BaseAsset, request.QuoteAsset, request.ApiType);
            var result = await SubscribeToTradeUpdatesAsync(symbol, update => handler(update.As(update.Data.Select(x => new SharedTrade(x.QuantityAbs, x.Price, x.Timestamp)))), ct).ConfigureAwait(false);

            return result;
        }

        async Task<CallResult<UpdateSubscription>> IBookTickerSocketClient.SubscribeToBookTickerUpdatesAsync(BookTickerSubscribeRequest request, Action<DataEvent<SharedBookTicker>> handler, CancellationToken ct)
        {
            var symbol = FormatSymbol(request.BaseAsset, request.QuoteAsset, request.ApiType);
            var result = await SubscribeToTickerUpdatesAsync(symbol, update => handler(update.As(new SharedBookTicker(update.Data.BestAskPrice, update.Data.BestAskQuantity, update.Data.BestBidPrice, update.Data.BestBidQuantity))), ct).ConfigureAwait(false);

            return result;
        }

        async Task<CallResult<UpdateSubscription>> IBalanceSocketClient.SubscribeToBalanceUpdatesAsync(SharedRequest request, Action<DataEvent<IEnumerable<SharedBalance>>> handler, CancellationToken ct)
        {
            var result = await SubscribeToUserUpdatesAsync(
                walletHandler: update => handler(update.As(update.Data.Select(x => new SharedBalance(x.Asset, x.Available ?? 0, x.Total)))),
                ct: ct).ConfigureAwait(false);

            return result;
        }

        async Task<CallResult<UpdateSubscription>> ISpotOrderSocketClient.SubscribeToOrderUpdatesAsync(SharedRequest request, Action<DataEvent<IEnumerable<SharedSpotOrder>>> handler, CancellationToken ct)
        {
            var result = await SubscribeToUserUpdatesAsync(
                orderHandler: update => handler(update.As(update.Data.Select(x => 
                    new SharedSpotOrder(
                        x.Symbol,
                        x.Id.ToString(),
                        x.Type == Enums.OrderType.Limit ? CryptoExchange.Net.SharedApis.Enums.SharedOrderType.Limit : x.Type == Enums.OrderType.Market ? CryptoExchange.Net.SharedApis.Enums.SharedOrderType.Market : CryptoExchange.Net.SharedApis.Enums.SharedOrderType.Other,
                        x.Side == Enums.OrderSide.Buy ? CryptoExchange.Net.SharedApis.Enums.SharedOrderSide.Buy : CryptoExchange.Net.SharedApis.Enums.SharedOrderSide.Sell,
                        x.Status == Enums.OrderStatus.Canceled ? CryptoExchange.Net.SharedApis.Enums.SharedOrderStatus.Canceled : (x.Status == Enums.OrderStatus.Active || x.Status == Enums.OrderStatus.PartiallyFilled) ? CryptoExchange.Net.SharedApis.Enums.SharedOrderStatus.PartiallyFilled : CryptoExchange.Net.SharedApis.Enums.SharedOrderStatus.Filled,
                        x.CreateTime)
                    {
                        ClientOrderId = x.ClientOrderId.ToString(),
                        Price = x.Price,
                        Quantity = x.Quantity,
                        QuantityFilled = x.Quantity - x.QuantityRemaining,
                        AveragePrice = x.PriceAverage,
                        UpdateTime = x.UpdateTime
                    }
                ))),
                ct: ct).ConfigureAwait(false);

            return result;
        }
    }
}
