using Bitfinex.Net.Enums;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients.SpotApi
{
    internal partial class BitfinexSocketClientSpotApi : IBitfinexSocketClientSpotApiShared
    {
        private const string _topicId = "BitfinexSpot";

        public string Exchange => BitfinexExchange.ExchangeName;
        public TradingMode[] SupportedTradingModes { get; } = new[] { TradingMode.Spot };

        public void SetDefaultExchangeParameter(string key, object value) => ExchangeParameters.SetStaticParameter(Exchange, key, value);
        public void ResetDefaultExchangeParameters() => ExchangeParameters.ResetStaticParameters();

        #region Ticker client
        EndpointOptions<SubscribeTickerRequest> ITickerSocketClient.SubscribeTickerOptions { get; } = new EndpointOptions<SubscribeTickerRequest>(false);
        async Task<ExchangeResult<UpdateSubscription>> ITickerSocketClient.SubscribeToTickerUpdatesAsync(SubscribeTickerRequest request, Action<ExchangeEvent<SharedSpotTicker>> handler, CancellationToken ct)
        {
            var validationError = ((ITickerSocketClient)this).SubscribeTickerOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await SubscribeToTickerUpdatesAsync(symbol, update => handler(update.AsExchangeEvent(Exchange, new SharedSpotTicker(ExchangeSymbolCache.ParseSymbol(_topicId, symbol), symbol, update.Data.LastPrice, update.Data.HighPrice, update.Data.LowPrice, update.Data.Volume, Math.Round(update.Data.DailyChangePercentage * 100, 2)))), ct).ConfigureAwait(false);

            return new ExchangeResult<UpdateSubscription>(Exchange, result);
        }
        #endregion

        #region Trade client

        EndpointOptions<SubscribeTradeRequest> ITradeSocketClient.SubscribeTradeOptions { get; } = new EndpointOptions<SubscribeTradeRequest>(false);
        async Task<ExchangeResult<UpdateSubscription>> ITradeSocketClient.SubscribeToTradeUpdatesAsync(SubscribeTradeRequest request, Action<ExchangeEvent<SharedTrade[]>> handler, CancellationToken ct)
        {
            var validationError = ((ITradeSocketClient)this).SubscribeTradeOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await SubscribeToTradeUpdatesAsync(symbol, update =>
            {
                if (update.UpdateType == SocketUpdateType.Snapshot || update.StreamId!.EndsWith(".tu"))
                    return;

                handler(update.AsExchangeEvent<SharedTrade[]>(Exchange, update.Data.Select(x => new SharedTrade(x.QuantityAbs, x.Price, x.Timestamp)
                {
                    Side = x.Quantity > 0 ? SharedOrderSide.Buy : SharedOrderSide.Sell
                }).ToArray()));
            }, ct).ConfigureAwait(false);

            return new ExchangeResult<UpdateSubscription>(Exchange, result);
        }
        #endregion

        #region Book Ticker client

        EndpointOptions<SubscribeBookTickerRequest> IBookTickerSocketClient.SubscribeBookTickerOptions { get; } = new EndpointOptions<SubscribeBookTickerRequest>(false);
        async Task<ExchangeResult<UpdateSubscription>> IBookTickerSocketClient.SubscribeToBookTickerUpdatesAsync(SubscribeBookTickerRequest request, Action<ExchangeEvent<SharedBookTicker>> handler, CancellationToken ct)
        {
            var validationError = ((IBookTickerSocketClient)this).SubscribeBookTickerOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await SubscribeToTickerUpdatesAsync(symbol, update => handler(update.AsExchangeEvent(Exchange, new SharedBookTicker(ExchangeSymbolCache.ParseSymbol(_topicId, symbol), symbol, update.Data.BestAskPrice, update.Data.BestAskQuantity, update.Data.BestBidPrice, update.Data.BestBidQuantity))), ct).ConfigureAwait(false);

            return new ExchangeResult<UpdateSubscription>(Exchange, result);
        }
        #endregion

        #region Balance client
        EndpointOptions<SubscribeBalancesRequest> IBalanceSocketClient.SubscribeBalanceOptions { get; } = new EndpointOptions<SubscribeBalancesRequest>(false);
        async Task<ExchangeResult<UpdateSubscription>> IBalanceSocketClient.SubscribeToBalanceUpdatesAsync(SubscribeBalancesRequest request, Action<ExchangeEvent<SharedBalance[]>> handler, CancellationToken ct)
        {
            var validationError = ((IBalanceSocketClient)this).SubscribeBalanceOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var result = await SubscribeToUserUpdatesAsync(
                walletHandler: update => {
                    if (update.UpdateType == SocketUpdateType.Snapshot)
                        return;

                    var updateData = update.Data.Where(x => x.Type == Enums.WalletType.Exchange);
                    if (!updateData.Any())
                        return;

                    handler(update.AsExchangeEvent<SharedBalance[]>(Exchange, updateData.Select(x => 
                        new SharedBalance(BitfinexExchange.AssetAliases.ExchangeToCommonName(x.Asset), x.Available ?? x.Total, x.Total)).ToArray()));
                },
                ct: ct).ConfigureAwait(false);

            return new ExchangeResult<UpdateSubscription>(Exchange, result);
        }
        #endregion

        #region Spot Order client
        EndpointOptions<SubscribeSpotOrderRequest> ISpotOrderSocketClient.SubscribeSpotOrderOptions { get; } = new EndpointOptions<SubscribeSpotOrderRequest>(false);
        async Task<ExchangeResult<UpdateSubscription>> ISpotOrderSocketClient.SubscribeToSpotOrderUpdatesAsync(SubscribeSpotOrderRequest request, Action<ExchangeEvent<SharedSpotOrder[]>> handler, CancellationToken ct)
        {
            var validationError = ((ISpotOrderSocketClient)this).SubscribeSpotOrderOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var result = await SubscribeToUserUpdatesAsync(
                orderHandler: update =>
                {
                    if (update.UpdateType == SocketUpdateType.Snapshot)
                        return;

                    handler(update.AsExchangeEvent<SharedSpotOrder[]>(Exchange, update.Data.Select(x =>
                        new SharedSpotOrder(
                            ExchangeSymbolCache.ParseSymbol(_topicId, x.Symbol),
                            x.Symbol,
                            x.Id.ToString(),
                            ParseOrderType(x.Type),
                            x.Side == Enums.OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                            x.Status == Enums.OrderStatus.Canceled ? SharedOrderStatus.Canceled : (x.Status == Enums.OrderStatus.Active || x.Status == Enums.OrderStatus.PartiallyFilled) ? SharedOrderStatus.Open : SharedOrderStatus.Filled,
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

            return new ExchangeResult<UpdateSubscription>(Exchange, result);
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

        #region User Trade client
        EndpointOptions<SubscribeUserTradeRequest> IUserTradeSocketClient.SubscribeUserTradeOptions { get; } = new EndpointOptions<SubscribeUserTradeRequest>(false);
        async Task<ExchangeResult<UpdateSubscription>> IUserTradeSocketClient.SubscribeToUserTradeUpdatesAsync(SubscribeUserTradeRequest request, Action<ExchangeEvent<SharedUserTrade[]>> handler, CancellationToken ct)
        {
            var result = await SubscribeToUserUpdatesAsync(
                tradeHandler: update => handler(update.AsExchangeEvent<SharedUserTrade[]>(Exchange, new[] {
                    new SharedUserTrade(
                        ExchangeSymbolCache.ParseSymbol(_topicId, update.Data.Symbol),
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
                        Role = update.Data.Maker == true ? SharedRole.Maker: SharedRole.Taker
                    }
                })),
                ct: ct).ConfigureAwait(false);

            return new ExchangeResult<UpdateSubscription>(Exchange, result);
        }
        #endregion

        #region Kline client
        SubscribeKlineOptions IKlineSocketClient.SubscribeKlineOptions { get; } = new SubscribeKlineOptions(false,
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
        async Task<ExchangeResult<UpdateSubscription>> IKlineSocketClient.SubscribeToKlineUpdatesAsync(SubscribeKlineRequest request, Action<ExchangeEvent<SharedKline>> handler, CancellationToken ct)
        {
            var interval = (Enums.KlineInterval)request.Interval;
            if (!Enum.IsDefined(typeof(Enums.KlineInterval), interval))
                return new ExchangeResult<UpdateSubscription>(Exchange, ArgumentError.Invalid(nameof(SubscribeKlineRequest.Interval), "Interval not supported"));

            var validationError = ((IKlineSocketClient)this).SubscribeKlineOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeResult<UpdateSubscription>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await SubscribeToKlineUpdatesAsync(symbol, interval, update => {
                if (update.UpdateType == SocketUpdateType.Snapshot)
                    return;

                foreach (var item in update.Data)
                    handler(update.AsExchangeEvent(Exchange, new SharedKline(item.OpenTime, item.ClosePrice, item.HighPrice, item.LowPrice, item.OpenPrice, item.Volume)));
                }
            , ct).ConfigureAwait(false);

            return new ExchangeResult<UpdateSubscription>(Exchange, result);
        }
        #endregion
    }
}
