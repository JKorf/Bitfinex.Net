using Bitfinex.Net.Enums;
using Bitfinex.Net.Interfaces.Clients.ExchangeApi;
using Bitfinex.Net.Objects.Models;
using CryptoExchange.Net;
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
    internal partial class BitfinexSocketClientExchangeApi : IBitfinexSocketClientExchangeApiShared
    {
        private const string _exchangeName = "Bitfinex";
        private const string _topicSpotId = "BitfinexSpot";
        private const string _topicFuturesId = "BitfinexFutures";

        public TradingMode[] SupportedTradingModes { get; } = new[] { TradingMode.Spot, TradingMode.PerpetualLinear };

        public void SetDefaultExchangeParameter(string key, object value) => ExchangeParameters.SetStaticParameter(Exchange, key, value);
        public void ResetDefaultExchangeParameters() => ExchangeParameters.ResetStaticParameters();
        public SharedClientInfo Discover() => SharedUtils.GetClientInfo(BitfinexExchange.Metadata, this);

        #region Ticker client
        SubscribeTickerOptions ITickerSocketClient.SubscribeTickerOptions { get; } = new SubscribeTickerOptions(_exchangeName);
        async Task<WebSocketResult<UpdateSubscription>> ITickerSocketClient.SubscribeToTickerUpdatesAsync(SubscribeTickerRequest request, Action<DataEvent<SharedSpotTicker>> handler, CancellationToken ct)
        {
            var validationError = ((ITickerSocketClient)this).SubscribeTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await SubscribeToTickerUpdatesAsync(symbol, update => handler(
                update.ToType(new SharedSpotTicker(
                    request.Symbol, 
                    symbol, 
                    update.Data.LastPrice, 
                    update.Data.HighPrice,
                    update.Data.LowPrice, 
                    update.Data.Volume,
                    Math.Round(update.Data.DailyChangePercentage * 100, 2)))), ct).ConfigureAwait(false);

            return result;
        }
        #endregion

        #region Trade client

        SubscribeTradeOptions ITradeSocketClient.SubscribeTradeOptions { get; } = new SubscribeTradeOptions(_exchangeName, false);
        async Task<WebSocketResult<UpdateSubscription>> ITradeSocketClient.SubscribeToTradeUpdatesAsync(SubscribeTradeRequest request, Action<DataEvent<SharedTrade[]>> handler, CancellationToken ct)
        {
            var validationError = ((ITradeSocketClient)this).SubscribeTradeOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await SubscribeToTradeUpdatesAsync(symbol, update =>
            {
                if (update.UpdateType == SocketUpdateType.Snapshot || update.StreamId!.EndsWith(".tu"))
                    return;

                handler(update.ToType<SharedTrade[]>(update.Data.Select(x => 
                new SharedTrade(
                    request.Symbol, 
                    symbol,
                    x.QuantityAbs,
                    x.Price,
                    x.Timestamp)
                {
                    Side = x.Quantity > 0 ? SharedOrderSide.Buy : SharedOrderSide.Sell
                }).ToArray()));
            }, ct).ConfigureAwait(false);

            return result;
        }
        #endregion

        #region Book Ticker client

        SubscribeBookTickerOptions IBookTickerSocketClient.SubscribeBookTickerOptions { get; } = new SubscribeBookTickerOptions(_exchangeName, false);
        async Task<WebSocketResult<UpdateSubscription>> IBookTickerSocketClient.SubscribeToBookTickerUpdatesAsync(SubscribeBookTickerRequest request, Action<DataEvent<SharedBookTicker>> handler, CancellationToken ct)
        {
            var validationError = ((IBookTickerSocketClient)this).SubscribeBookTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await SubscribeToTickerUpdatesAsync(symbol, update => handler(
                update.ToType(
                    new SharedBookTicker(
                        request.Symbol, 
                        symbol, 
                        update.Data.BestAskPrice,
                        update.Data.BestAskQuantity,
                        update.Data.BestBidPrice,
                        update.Data.BestBidQuantity))), ct).ConfigureAwait(false);

            return result;
        }
        #endregion

        #region Balance client
        SubscribeBalanceOptions IBalanceSocketClient.SubscribeBalanceOptions { get; } = new SubscribeBalanceOptions(_exchangeName, false);
        async Task<WebSocketResult<UpdateSubscription>> IBalanceSocketClient.SubscribeToBalanceUpdatesAsync(SubscribeBalancesRequest request, Action<DataEvent<SharedBalance[]>> handler, CancellationToken ct)
        {
            var validationError = ((IBalanceSocketClient)this).SubscribeBalanceOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var result = await SubscribeToUserUpdatesAsync(
                walletHandler: update => {
                    if (update.UpdateType == SocketUpdateType.Snapshot)
                        return;

                    var updateData = update.Data.Where(x => x.Type == Enums.WalletType.Exchange);
                    if (!updateData.Any())
                        return;

                    handler(update.ToType<SharedBalance[]>(updateData.Select(x => 
                        new SharedBalance(
                            SupportedTradingModes,
                            BitfinexExchange.AssetAliases.ExchangeToCommonName(x.Asset),
                            x.Available ?? x.Total,
                            x.Total)).ToArray()));
                },
                ct: ct).ConfigureAwait(false);

            return result;
        }
        #endregion

        #region Spot Order client
        SubscribeSpotOrderOptions ISpotOrderSocketClient.SubscribeSpotOrderOptions { get; } = new SubscribeSpotOrderOptions(_exchangeName, false);
        async Task<WebSocketResult<UpdateSubscription>> ISpotOrderSocketClient.SubscribeToSpotOrderUpdatesAsync(SubscribeSpotOrderRequest request, Action<DataEvent<SharedSpotOrder[]>> handler, CancellationToken ct)
        {
            var validationError = ((ISpotOrderSocketClient)this).SubscribeSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var result = await SubscribeToUserUpdatesAsync(
                orderHandler: update =>
                {
                    if (update.UpdateType == SocketUpdateType.Snapshot)
                        return;

                    var data = update.Data.Where(x => !x.Symbol.Contains("F0"));
                    if (!data.Any())
                        return;

                    handler(update.ToType<SharedSpotOrder[]>(data.Select(x =>
                        new SharedSpotOrder(
                            ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, x.Symbol),
                            x.Symbol,
                            x.Id.ToString(),
                            ParseOrderType(x.Type),
                            x.Side == Enums.OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                            ParseOrderStatus(x.Status),
                            x.CreateTime)
                        {
                            ClientOrderId = x.ClientOrderId.ToString(),
                            OrderQuantity = new SharedOrderQuantity(x.Quantity),
                            QuantityFilled = new SharedOrderQuantity(x.Quantity - x.QuantityRemaining),
                            AveragePrice = x.PriceAverage == 0 ? null : x.PriceAverage,
                            UpdateTime = x.UpdateTime,
                            IsTriggerOrder = x.Type == OrderType.ExchangeStop || x.Type == OrderType.ExchangeStopLimit,
                            OrderPrice = x.Type == OrderType.ExchangeStop || x.Type == OrderType.ExchangeStopLimit ? x.PriceAuxilliaryLimit : x.Price,
                            TriggerPrice = x.Type == OrderType.ExchangeStop || x.Type == OrderType.ExchangeStopLimit ? x.Price : null
                        }
                    ).ToArray()));
                },
                ct: ct).ConfigureAwait(false);

            return result;
        }

        private SharedOrderStatus ParseOrderStatus(OrderStatus status)
        {
            if (status == Enums.OrderStatus.Canceled)
                return SharedOrderStatus.Canceled;
            if (status == Enums.OrderStatus.Active || status == Enums.OrderStatus.PartiallyFilled)
                return SharedOrderStatus.Open;
            if (status == OrderStatus.Executed || status == OrderStatus.ForcefullyExecuted)
                return SharedOrderStatus.Filled;

            return SharedOrderStatus.Unknown;
        }

        private SharedOrderType ParseOrderType(OrderType type)
        {
            if (type == OrderType.ExchangeMarket || type == OrderType.ExchangeStop)
                return SharedOrderType.Market;

            if (type == OrderType.ExchangeLimit || type == OrderType.ExchangeStopLimit)
                return SharedOrderType.Limit;

            return SharedOrderType.Other;
        }
        #endregion

        #region Futures Order client

        SubscribeFuturesOrderOptions IFuturesOrderSocketClient.SubscribeFuturesOrderOptions { get; }
            = new SubscribeFuturesOrderOptions(_exchangeName, true);
        async Task<WebSocketResult<UpdateSubscription>> IFuturesOrderSocketClient.SubscribeToFuturesOrderUpdatesAsync(SubscribeFuturesOrderRequest request, Action<DataEvent<SharedFuturesOrder[]>> handler, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderSocketClient)this).SubscribeFuturesOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var result = await SubscribeToUserUpdatesAsync(
                orderHandler: update =>
                {
                    if (update.UpdateType == SocketUpdateType.Snapshot)
                        return;

                    var data = update.Data.Where(x => x.Symbol.Contains("F0"));
                    if (!data.Any())
                        return;

                    handler(update.ToType<SharedFuturesOrder[]>(data.Select(x =>
                        new SharedFuturesOrder(
                            ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, x.Symbol),
                            x.Symbol,
                            x.Id.ToString(),
                            ParseOrderType(x.Type),
                            x.Side == Enums.OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                            ParseOrderStatus(x.Status),
                            x.CreateTime)
                        {
                            ClientOrderId = x.ClientOrderId.ToString(),
                            OrderQuantity = new SharedOrderQuantity(x.Quantity),
                            QuantityFilled = new SharedOrderQuantity(x.Quantity - x.QuantityRemaining),
                            AveragePrice = x.PriceAverage == 0 ? null : x.PriceAverage,
                            UpdateTime = x.UpdateTime,
                            IsTriggerOrder = x.Type == OrderType.ExchangeStop || x.Type == OrderType.ExchangeStopLimit,
                            OrderPrice = x.Type == OrderType.ExchangeStop || x.Type == OrderType.ExchangeStopLimit ? x.PriceAuxilliaryLimit : x.Price,
                            TriggerPrice = x.Type == OrderType.ExchangeStop || x.Type == OrderType.ExchangeStopLimit ? x.Price : null
                        }
                    ).ToArray()));
                },
                ct: ct).ConfigureAwait(false);

            return result;
        }
        #endregion

        #region User Trade client
        SubscribeUserTradeOptions IUserTradeSocketClient.SubscribeUserTradeOptions { get; } = new SubscribeUserTradeOptions(_exchangeName, false);
        async Task<WebSocketResult<UpdateSubscription>> IUserTradeSocketClient.SubscribeToUserTradeUpdatesAsync(SubscribeUserTradeRequest request, Action<DataEvent<SharedUserTrade[]>> handler, CancellationToken ct)
        {
            var result = await SubscribeToUserUpdatesAsync(
                tradeHandler: update =>
                {
                    if (request.TradingMode == TradingMode.Spot && update.Data.Symbol.Contains("F0"))
                        return;
                    else if (request.TradingMode == TradingMode.PerpetualLinear && !update.Data.Symbol.Contains("F0"))
                        return;

                    handler(update.ToType<SharedUserTrade[]>(new[] {
                        new SharedUserTrade(
                            ExchangeSymbolCache.ParseSymbol(update.Data.Symbol.Contains("F0") ? _topicFuturesId : _topicSpotId, EnvironmentName, null, update.Data.Symbol),
                            update.Data.Symbol,
                            update.Data.OrderId.ToString(),
                            update.Data.Id.ToString(),
                            update.Data.QuantityRaw > 0 ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                            update.Data.Quantity,
                            update.Data.Price,
                            update.Data.Timestamp)
                        {
                            Fee = Math.Abs(update.Data.Fee),
                            FeeAsset = BitfinexExchange.AssetAliases.ExchangeToCommonName(update.Data.FeeAsset),
                            Role = update.Data.Maker == true ? SharedRole.Maker : SharedRole.Taker,
                            ClientOrderId = update.Data.ClientOrderId?.ToString()
                        }
                    }));
                },
                ct: ct).ConfigureAwait(false);

            return result;
        }
        #endregion

        #region Kline client
        SubscribeKlineOptions IKlineSocketClient.SubscribeKlineOptions { get; } = new SubscribeKlineOptions(_exchangeName, false,
            SharedKlineInterval.OneMinute,
            SharedKlineInterval.ThreeMinutes,
            SharedKlineInterval.FiveMinutes,
            SharedKlineInterval.FifteenMinutes,
            SharedKlineInterval.ThirtyMinutes,
            SharedKlineInterval.OneHour,
            SharedKlineInterval.SixHours,
            SharedKlineInterval.TwelveHours,
            SharedKlineInterval.OneDay,
            SharedKlineInterval.OneWeek,
            SharedKlineInterval.OneMonth);
        async Task<WebSocketResult<UpdateSubscription>> IKlineSocketClient.SubscribeToKlineUpdatesAsync(SubscribeKlineRequest request, Action<DataEvent<SharedKline>> handler, CancellationToken ct)
        {
            var interval = (Enums.KlineInterval)request.Interval;
            if (!Enum.IsDefined(typeof(Enums.KlineInterval), interval))
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, ArgumentError.Invalid(nameof(SubscribeKlineRequest.Interval), "Interval not supported"));

            var validationError = ((IKlineSocketClient)this).SubscribeKlineOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await SubscribeToKlineUpdatesAsync(symbol, interval, update => {
                if (update.UpdateType == SocketUpdateType.Snapshot)
                    return;

                foreach (var item in update.Data)
                    handler(update.ToType(new SharedKline(request.Symbol, symbol, item.OpenTime, item.ClosePrice, item.HighPrice, item.LowPrice, item.OpenPrice, item.Volume)));
                }
            , ct).ConfigureAwait(false);

            return result;
        }
        #endregion

        #region Position client
        SubscribePositionOptions IPositionSocketClient.SubscribePositionOptions { get; }
            = new SubscribePositionOptions(_exchangeName, true);
        async Task<WebSocketResult<UpdateSubscription>> IPositionSocketClient.SubscribeToPositionUpdatesAsync(SubscribePositionRequest request, Action<DataEvent<SharedPosition[]>> handler, CancellationToken ct)
        {
            var validationError = ((IPositionSocketClient)this).SubscribePositionOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var result = await SubscribeToUserUpdatesAsync(
                positionHandler: update => handler(update.ToType(update.Data.Select(x => 
                    new SharedPosition(
                        ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, x.Symbol),
                        x.Symbol,
                        Math.Abs(x.Quantity),
                        x.UpdateTime)
                    {
                        AverageOpenPrice = x.BasePrice,
                        LiquidationPrice = x.LiquidationPrice,
                        Leverage = x.Leverage,
                        PositionMode = SharedPositionMode.OneWay,
                        PositionSide = x.Quantity >= 0 ? SharedPositionSide.Long : SharedPositionSide.Short,
                        UnrealizedPnl = x.ProfitLoss
                    }).ToArray())),
                ct: ct).ConfigureAwait(false);

            return result;
        }

        #endregion
    }
}
