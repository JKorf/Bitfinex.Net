using Bitfinex.Net.Enums;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients.SpotApi
{
    internal partial class BitfinexRestClientSpotApi : IBitfinexRestClientSpotApiShared
    {
        public string Exchange => BitfinexExchange.ExchangeName;
        public TradingMode[] SupportedTradingModes { get; } = new[] { TradingMode.Spot };
        public void SetDefaultExchangeParameter(string key, object value) => ExchangeParameters.SetStaticParameter(Exchange, key, value);
        public void ResetDefaultExchangeParameters() => ExchangeParameters.ResetStaticParameters();

        #region Kline client

        GetKlinesOptions IKlineRestClient.GetKlinesOptions { get; } = new GetKlinesOptions(SharedPaginationSupport.Descending, true, 10000, false);

        async Task<ExchangeWebResult<IEnumerable<SharedKline>>> IKlineRestClient.GetKlinesAsync(GetKlinesRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((IKlineRestClient)this).GetKlinesOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedKline>>(Exchange, validationError);

            var interval = (Enums.KlineInterval)request.Interval;
            if (!Enum.IsDefined(typeof(Enums.KlineInterval), interval))
                return new ExchangeWebResult<IEnumerable<SharedKline>>(Exchange, new ArgumentError("Interval not supported"));

            // Determine page token
            DateTime? fromTimestamp = null;
            if (pageToken is DateTimeToken dateTimeToken)
                fromTimestamp = dateTimeToken.LastTime;

            // Get data
            var baseAsset = request.Symbol.BaseAsset == "USDT" ? "UST" : request.Symbol.BaseAsset;
            var quoteAsset = request.Symbol.QuoteAsset == "USDT" ? "UST" : request.Symbol.QuoteAsset;

            var limit = request.Limit ?? 10000;
            var result = await ExchangeData.GetKlinesAsync(
                request.Symbol.GetSymbol(FormatSymbol),
                interval,
                startTime: request.StartTime,
                endTime: fromTimestamp ?? request.EndTime?.AddSeconds(-1),
                limit: limit,
                sorting: Sorting.NewFirst,
                ct: ct
                ).ConfigureAwait(false);

            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedKline>>(Exchange, null, default);

            // Get next token
            DateTimeToken? nextToken = null;
            if (result.Data.Count() == limit)
            {
                var minOpenTime = result.Data.Min(x => x.OpenTime);
                nextToken = new DateTimeToken(minOpenTime.AddSeconds(-(int)interval));
            }

            return result.AsExchangeResult<IEnumerable<SharedKline>>(Exchange, request.Symbol.TradingMode, result.Data.Select(x => new SharedKline(x.OpenTime, x.ClosePrice, x.HighPrice, x.LowPrice, x.OpenPrice, x.Volume)).ToArray(), nextToken);
        }

        #endregion

        #region Asset client
        EndpointOptions<GetAssetRequest> IAssetsRestClient.GetAssetOptions { get; } = new EndpointOptions<GetAssetRequest>(false);
        async Task<ExchangeWebResult<SharedAsset>> IAssetsRestClient.GetAssetAsync(GetAssetRequest request, CancellationToken ct)
        {
            var validationError = ((IAssetsRestClient)this).GetAssetOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedAsset>(Exchange, validationError);

            // Execute needed config requests in parallel
            var assetSymbols = ExchangeData.GetAssetSymbolsAsync(ct: ct);
            var assetList = ExchangeData.GetAssetsListAsync(ct: ct);
            var assetMethods = ExchangeData.GetAssetDepositWithdrawalMethodsAsync(ct: ct);
            var assetFees = ExchangeData.GetAssetWithdrawalFeesAsync(ct: ct);
            var assetTxStatus = ExchangeData.GetDepositWithdrawalStatusAsync(ct: ct);
            await Task.WhenAll(assetList, assetMethods).ConfigureAwait(false);
            if (!assetSymbols.Result)
                return assetSymbols.Result.AsExchangeResult<SharedAsset>(Exchange, TradingMode.Spot, default);
            if (!assetList.Result)
                return assetList.Result.AsExchangeResult<SharedAsset>(Exchange, TradingMode.Spot, default);
            if (!assetMethods.Result)
                return assetMethods.Result.AsExchangeResult<SharedAsset>(Exchange, TradingMode.Spot, default);
            if (!assetFees.Result)
                return assetFees.Result.AsExchangeResult<SharedAsset>(Exchange, TradingMode.Spot, default);
            if (!assetTxStatus.Result)
                return assetTxStatus.Result.AsExchangeResult<SharedAsset>(Exchange, TradingMode.Spot, default);

            var asset = assetList.Result.Data.SingleOrDefault(x => x.Name == request.Asset);
            var symbol = assetSymbols.Result.Data.SingleOrDefault(y => y.Key == asset.FullName).Value ?? asset.Name;
            var fees = assetFees.Result.Data.SingleOrDefault(y => y.Key.Equals(symbol, StringComparison.OrdinalIgnoreCase));

            var assetResult = new SharedAsset(symbol)
            {
                FullName = asset.FullName,
                Networks = assetMethods.Result.Data.Where(y => y.Value.Contains(symbol))?.Select(x =>
                {
                    var status = assetTxStatus.Result.Data.Single(s => s.Method.Equals(x.Key, StringComparison.OrdinalIgnoreCase));
                    return new SharedAssetNetwork(x.Key)
                    {
                        WithdrawFee = fees.Value?.Skip(1).First(),
                        DepositEnabled = status.DepositStatus,
                        WithdrawEnabled = status.WithdrawalStatus,
                        MinConfirmations = status.DepositConfirmations
                    };
                }).ToList()
            };


            return assetList.Result.AsExchangeResult(Exchange, TradingMode.Spot, assetResult);
        }

        EndpointOptions<GetAssetsRequest> IAssetsRestClient.GetAssetsOptions { get; } = new EndpointOptions<GetAssetsRequest>(false);

        async Task<ExchangeWebResult<IEnumerable<SharedAsset>>> IAssetsRestClient.GetAssetsAsync(GetAssetsRequest request, CancellationToken ct)
        {
            var validationError = ((IAssetsRestClient)this).GetAssetsOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedAsset>>(Exchange, validationError);

            // Execute needed config requests in parallel
            var assetSymbols = ExchangeData.GetAssetSymbolsAsync(ct: ct);
            var assetList = ExchangeData.GetAssetsListAsync(ct: ct);
            var assetMethods = ExchangeData.GetAssetDepositWithdrawalMethodsAsync(ct: ct);
            var assetFees = ExchangeData.GetAssetWithdrawalFeesAsync(ct: ct);
            var assetTxStatus = ExchangeData.GetDepositWithdrawalStatusAsync(ct: ct);
            await Task.WhenAll(assetList, assetMethods).ConfigureAwait(false);
            if (!assetSymbols.Result)
                return assetSymbols.Result.AsExchangeResult<IEnumerable<SharedAsset>>(Exchange, null, default);
            if (!assetList.Result)
                return assetList.Result.AsExchangeResult<IEnumerable<SharedAsset>>(Exchange, null, default);
            if (!assetMethods.Result)
                return assetMethods.Result.AsExchangeResult<IEnumerable<SharedAsset>>(Exchange, null, default);
            if (!assetFees.Result)
                return assetFees.Result.AsExchangeResult<IEnumerable<SharedAsset>>(Exchange, null, default);
            if (!assetTxStatus.Result)
                return assetTxStatus.Result.AsExchangeResult<IEnumerable<SharedAsset>>(Exchange, null, default);

            return assetList.Result.AsExchangeResult<IEnumerable<SharedAsset>>(Exchange, TradingMode.Spot, 
                assetList.Result.Data.Select(x => 
                {
                    var symbol = assetSymbols.Result.Data.SingleOrDefault(y => y.Key == x.FullName).Value ?? x.Name;
                    var fees = assetFees.Result.Data.SingleOrDefault(y => y.Key.Equals(symbol, StringComparison.OrdinalIgnoreCase));
                    if (fees.Key == null)
                        return null;
                
                    return new SharedAsset(symbol)
                    {
                        FullName = x.FullName,
                        Networks = assetMethods.Result.Data.Where(y => y.Value.Contains(symbol))?.Select(x =>
                        {
                            var status = assetTxStatus.Result.Data.Single(s => s.Method.Equals(x.Key, StringComparison.OrdinalIgnoreCase));
                            return new SharedAssetNetwork(x.Key)
                            {
                                WithdrawFee = fees.Value.Skip(1).First(),
                                DepositEnabled = status.DepositStatus,
                                WithdrawEnabled = status.WithdrawalStatus,
                                MinConfirmations = status.DepositConfirmations
                            };
                        }).ToArray()
                    };
                }).Where(x => x != null).ToArray()!
            );
        }

        #endregion

        #region Spot Symbol client

        EndpointOptions<GetSymbolsRequest> ISpotSymbolRestClient.GetSpotSymbolsOptions { get; } = new EndpointOptions<GetSymbolsRequest>(false);
        async Task<ExchangeWebResult<IEnumerable<SharedSpotSymbol>>> ISpotSymbolRestClient.GetSpotSymbolsAsync(GetSymbolsRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotSymbolRestClient)this).GetSpotSymbolsOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedSpotSymbol>>(Exchange, validationError);

            var result = await ExchangeData.GetSymbolsAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedSpotSymbol>>(Exchange, null, default);

            return result.AsExchangeResult<IEnumerable<SharedSpotSymbol>>(Exchange, TradingMode.Spot, result.Data.Select(s =>
            {
                var assets = GetAssets(s.Key);
                return new SharedSpotSymbol(assets.BaseAsset, assets.QuoteAsset, "t" + s.Key, true)
                {
                    // These apply to all symbols
                    PriceSignificantFigures = 5,
                    PriceDecimals = 8,
                    QuantityDecimals = 8,

                    MinTradeQuantity = s.Value.MinOrderQuantity,
                    MaxTradeQuantity = s.Value.MaxOrderQuantity
                };
            }).ToArray());

        }

        private (string BaseAsset, string QuoteAsset) GetAssets(string input)
        {
            if (input.Contains(":"))
            {
                var split = input.Split(':');
                return (split[0], split[1]);
            }

            return (input.Substring(0, 3), input.Substring(3));
        }

        #endregion

        #region Ticker client

        EndpointOptions<GetTickerRequest> ISpotTickerRestClient.GetSpotTickerOptions { get; } = new EndpointOptions<GetTickerRequest>(false);
        async Task<ExchangeWebResult<SharedSpotTicker>> ISpotTickerRestClient.GetSpotTickerAsync(GetTickerRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotTickerRestClient)this).GetSpotTickerOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedSpotTicker>(Exchange, validationError);

            var baseAsset = request.Symbol.BaseAsset == "USDT" ? "UST" : request.Symbol.BaseAsset;
            var quoteAsset = request.Symbol.QuoteAsset == "USDT" ? "UST" : request.Symbol.QuoteAsset;

            var result = await ExchangeData.GetTickerAsync(request.Symbol.GetSymbol(FormatSymbol), ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedSpotTicker>(Exchange, null, default);

            return result.AsExchangeResult(Exchange, TradingMode.Spot, new SharedSpotTicker(result.Data.Symbol, result.Data.LastPrice, result.Data.HighPrice, result.Data.LowPrice, result.Data.Volume, Math.Round(result.Data.DailyChangePercentage * 100, 2)));
        }

        EndpointOptions<GetTickersRequest> ISpotTickerRestClient.GetSpotTickersOptions { get; } = new EndpointOptions<GetTickersRequest>(false);
        async Task<ExchangeWebResult<IEnumerable<SharedSpotTicker>>> ISpotTickerRestClient.GetSpotTickersAsync(GetTickersRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotTickerRestClient)this).GetSpotTickersOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedSpotTicker>>(Exchange, validationError);

            var result = await ExchangeData.GetTickersAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedSpotTicker>>(Exchange, null, default);

            return result.AsExchangeResult<IEnumerable<SharedSpotTicker>>(Exchange, TradingMode.Spot, result.Data.Select(x => new SharedSpotTicker(x.Symbol, x.LastPrice, x.HighPrice, x.LowPrice, x.Volume, Math.Round(x.DailyChangePercentage * 100, 2))).ToArray());
        }

        #endregion

        #region Recent Trade client

        GetRecentTradesOptions IRecentTradeRestClient.GetRecentTradesOptions { get; } = new GetRecentTradesOptions(10000, false);

        async Task<ExchangeWebResult<IEnumerable<SharedTrade>>> IRecentTradeRestClient.GetRecentTradesAsync(GetRecentTradesRequest request, CancellationToken ct)
        {
            var validationError = ((IRecentTradeRestClient)this).GetRecentTradesOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedTrade>>(Exchange, validationError);

            var result = await ExchangeData.GetTradeHistoryAsync(
                request.Symbol.GetSymbol(FormatSymbol),
                limit: request.Limit,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedTrade>>(Exchange, null, default);

            return result.AsExchangeResult<IEnumerable<SharedTrade>>(Exchange, request.Symbol.TradingMode, result.Data.Select(x => new SharedTrade(Math.Abs(x.Quantity), x.Price, x.Timestamp)
            {
                Side = x.Quantity > 0 ? SharedOrderSide.Buy : SharedOrderSide.Sell
            }).ToArray());
        }

        #endregion

        #region Balance client
        EndpointOptions<GetBalancesRequest> IBalanceRestClient.GetBalancesOptions { get; } = new EndpointOptions<GetBalancesRequest>(true);

        async Task<ExchangeWebResult<IEnumerable<SharedBalance>>> IBalanceRestClient.GetBalancesAsync(GetBalancesRequest request, CancellationToken ct)
        {
            var validationError = ((IBalanceRestClient)this).GetBalancesOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedBalance>>(Exchange, validationError);

            var result = await Account.GetBalancesAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedBalance>>(Exchange, null, default);

            return result.AsExchangeResult<IEnumerable<SharedBalance>>(Exchange, SupportedTradingModes, result.Data.Where(x => x.Type == WalletType.Exchange).Select(x => new SharedBalance(x.Asset, x.Available ?? 0, x.Total)).ToArray());
        }

        #endregion

        #region Spot Order client

        SharedFeeDeductionType ISpotOrderRestClient.SpotFeeDeductionType => SharedFeeDeductionType.DeductFromOutput;
        SharedFeeAssetType ISpotOrderRestClient.SpotFeeAssetType => SharedFeeAssetType.OutputAsset;
        IEnumerable<SharedOrderType> ISpotOrderRestClient.SpotSupportedOrderTypes { get; } = new[] { SharedOrderType.Limit, SharedOrderType.Market };
        IEnumerable<SharedTimeInForce> ISpotOrderRestClient.SpotSupportedTimeInForce { get; } = new[] { SharedTimeInForce.GoodTillCanceled, SharedTimeInForce.ImmediateOrCancel, SharedTimeInForce.FillOrKill };
        SharedQuantitySupport ISpotOrderRestClient.SpotSupportedOrderQuantity { get; } = new SharedQuantitySupport(
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset);

        PlaceSpotOrderOptions ISpotOrderRestClient.PlaceSpotOrderOptions { get; } = new PlaceSpotOrderOptions();
        async Task<ExchangeWebResult<SharedId>> ISpotOrderRestClient.PlaceSpotOrderAsync(PlaceSpotOrderRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).PlaceSpotOrderOptions.ValidateRequest(
                Exchange,
                request,
                request.Symbol.TradingMode,
                SupportedTradingModes,
                ((ISpotOrderRestClient)this).SpotSupportedOrderTypes,
                ((ISpotOrderRestClient)this).SpotSupportedTimeInForce,
                ((ISpotOrderRestClient)this).SpotSupportedOrderQuantity);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            int clientOrderId = 0;
            if (request.ClientOrderId != null && !int.TryParse(request.ClientOrderId, out clientOrderId))
                return new ExchangeWebResult<SharedId>(Exchange, new ArgumentError("ClientOrderId needs to be parsable to `int` for `Bitfinex`"));

            var result = await Trading.PlaceOrderAsync(
                request.Symbol.GetSymbol(FormatSymbol),
                request.Side == SharedOrderSide.Buy ? Enums.OrderSide.Buy : Enums.OrderSide.Sell,
                GetPlaceOrderType(request.OrderType, request.TimeInForce),
                quantity: request.Quantity ?? 0,
                flags: request.OrderType == SharedOrderType.LimitMaker ? Enums.OrderFlags.PostOnly : null,
                price: request.Price ?? 0,
                clientOrderId: request.ClientOrderId != null ? clientOrderId: null,
                ct: ct).ConfigureAwait(false);

            if (!result)
                return result.AsExchangeResult<SharedId>(Exchange, null, default);

            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedId(result.Data.Data!.Id.ToString()));
        }

        EndpointOptions<GetOrderRequest> ISpotOrderRestClient.GetSpotOrderOptions { get; } = new EndpointOptions<GetOrderRequest>(true);
        async Task<ExchangeWebResult<SharedSpotOrder>> ISpotOrderRestClient.GetSpotOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).GetSpotOrderOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedSpotOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedSpotOrder>(Exchange, new ArgumentError("Invalid order id"));

            var symbol = request.Symbol.GetSymbol(FormatSymbol);
            var result = await Trading.GetOpenOrdersAsync(symbol, new[] { orderId }, ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedSpotOrder>(Exchange, null, null);

            if (!result.Data.Any())
                result = await Trading.GetClosedOrdersAsync(symbol, new[] { orderId }, ct: ct).ConfigureAwait(false);

            if (!result.Data.Any())
                return result.AsExchangeError<SharedSpotOrder>(Exchange, new ServerError($"Order with id {orderId} not found"));

            var order = result.Data.Single();
            return result.AsExchangeResult(Exchange, TradingMode.Spot, new SharedSpotOrder(
                order.Symbol,
                order.Id.ToString(),
                ParseOrderType(order.Type, order.Flags),
                order.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                ParseOrderStatus(order.Status),
                order.CreateTime)
            {
                ClientOrderId = order.ClientOrderId?.ToString(),
                AveragePrice = order.PriceAverage == 0 ? null : order.PriceAverage,
                OrderPrice = order.Price,
                Quantity = order.Quantity,
                QuantityFilled = order.Quantity - order.QuantityRemaining,
                TimeInForce = ParseTimeInForce(order.Type, order.Flags),
                UpdateTime = order.UpdateTime
            });
        }

        EndpointOptions<GetOpenOrdersRequest> ISpotOrderRestClient.GetOpenSpotOrdersOptions { get; } = new EndpointOptions<GetOpenOrdersRequest>(true);
        async Task<ExchangeWebResult<IEnumerable<SharedSpotOrder>>> ISpotOrderRestClient.GetOpenSpotOrdersAsync(GetOpenOrdersRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).GetOpenSpotOrdersOptions.ValidateRequest(Exchange, request, request.Symbol?.TradingMode ?? request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedSpotOrder>>(Exchange, validationError);

            var symbol = request.Symbol?.GetSymbol(FormatSymbol);
            var result = await Trading.GetOpenOrdersAsync(symbol, ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedSpotOrder>>(Exchange, null, null);

            return result.AsExchangeResult<IEnumerable<SharedSpotOrder>>(Exchange, TradingMode.Spot, result.Data.Select(x => new SharedSpotOrder(
                x.Symbol,
                x.Id.ToString(),
                ParseOrderType(x.Type, x.Flags),
                x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                ParseOrderStatus(x.Status),
                x.CreateTime)
            {
                ClientOrderId = x.ClientOrderId?.ToString(),
                AveragePrice = x.PriceAverage == 0 ? null : x.PriceAverage,
                OrderPrice = x.Price,
                Quantity = x.Quantity,
                QuantityFilled = x.Quantity - x.QuantityRemaining,
                TimeInForce = ParseTimeInForce(x.Type, x.Flags),
                UpdateTime = x.UpdateTime
            }).ToArray());
        }

        PaginatedEndpointOptions<GetClosedOrdersRequest> ISpotOrderRestClient.GetClosedSpotOrdersOptions { get; } = new PaginatedEndpointOptions<GetClosedOrdersRequest>(SharedPaginationSupport.Descending, true, 2500, true);

        async Task<ExchangeWebResult<IEnumerable<SharedSpotOrder>>> ISpotOrderRestClient.GetClosedSpotOrdersAsync(GetClosedOrdersRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).GetClosedSpotOrdersOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedSpotOrder>>(Exchange, validationError);

            // Determine page token
            DateTime? fromTimestamp = null;
            if (pageToken is DateTimeToken dateTimeToken)
                fromTimestamp = dateTimeToken.LastTime;

            // Get data
            var limit = request.Limit ?? 100;
            var result = await Trading.GetClosedOrdersAsync(request.Symbol.GetSymbol(FormatSymbol),
                startTime: request.StartTime,
                endTime: fromTimestamp ?? request.EndTime,
                limit: limit,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedSpotOrder>>(Exchange, null, null);

            // Get next token
            DateTimeToken? nextToken = null;
            if (result.Data.Count() == limit)
                nextToken = new DateTimeToken(result.Data.Min(o => o.CreateTime).AddMilliseconds(-1));

            return result.AsExchangeResult<IEnumerable<SharedSpotOrder>>(Exchange, TradingMode.Spot, result.Data.Select(x => new SharedSpotOrder(
                x.Symbol,
                x.Id.ToString(),
                ParseOrderType(x.Type, x.Flags),
                x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                ParseOrderStatus(x.Status),
                x.CreateTime)
            {
                ClientOrderId = x.ClientOrderId?.ToString(),
                AveragePrice = x.PriceAverage == 0 ? null : x.PriceAverage,
                OrderPrice = x.Price,
                Quantity = x.Quantity,
                QuantityFilled = x.Quantity - x.QuantityRemaining,
                TimeInForce = ParseTimeInForce(x.Type, x.Flags),
                UpdateTime = x.UpdateTime
            }).ToArray(), nextToken);
        }

        EndpointOptions<GetOrderTradesRequest> ISpotOrderRestClient.GetSpotOrderTradesOptions { get; } = new EndpointOptions<GetOrderTradesRequest>(true);
        async Task<ExchangeWebResult<IEnumerable<SharedUserTrade>>> ISpotOrderRestClient.GetSpotOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).GetSpotOrderTradesOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedUserTrade>>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<IEnumerable<SharedUserTrade>>(Exchange, new ArgumentError("Invalid order id"));

            var order = await Trading.GetOrderTradesAsync(
                request.Symbol.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!order)
                return order.AsExchangeResult<IEnumerable<SharedUserTrade>>(Exchange, null, default);

            return order.AsExchangeResult<IEnumerable<SharedUserTrade>>(Exchange, TradingMode.Spot, order.Data.Select(x => new SharedUserTrade(
                x.Symbol,
                x.OrderId.ToString(),
                x.Id.ToString(),
                x.QuantityRaw > 0 ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                x.Quantity,
                x.Price,
                x.Timestamp)
            {
                Fee = Math.Abs(x.Fee),
                FeeAsset = x.FeeAsset,
                Role = x.Maker == true ? SharedRole.Maker : x.Maker == false ? SharedRole.Taker : null
            }).ToArray());
        }

        PaginatedEndpointOptions<GetUserTradesRequest> ISpotOrderRestClient.GetSpotUserTradesOptions { get; } = new PaginatedEndpointOptions<GetUserTradesRequest>(SharedPaginationSupport.Descending, true, 2500, true);
        async Task<ExchangeWebResult<IEnumerable<SharedUserTrade>>> ISpotOrderRestClient.GetSpotUserTradesAsync(GetUserTradesRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).GetSpotUserTradesOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedUserTrade>>(Exchange, validationError);

            // Determine page token
            DateTime? fromTimestamp = null;
            if (pageToken is DateTimeToken dateTimeToken)
                fromTimestamp = dateTimeToken.LastTime;

            // Get data
            var limit = request.Limit ?? 1000;
            var order = await Trading.GetUserTradesAsync(
                request.Symbol.GetSymbol(FormatSymbol),
                startTime: request.StartTime,
                endTime: fromTimestamp ?? request.EndTime,
                limit: limit,
                ct: ct).ConfigureAwait(false);
            if (!order)
                return order.AsExchangeResult<IEnumerable<SharedUserTrade>>(Exchange, null, default);

            // Get next token
            DateTimeToken? nextToken = null;
            if (order.Data.Count() == limit)
                nextToken = new DateTimeToken(order.Data.Min(o => o.Timestamp).AddMilliseconds(-1));

            return order.AsExchangeResult<IEnumerable<SharedUserTrade>>(Exchange, TradingMode.Spot, order.Data.Select(x => new SharedUserTrade(
                x.Symbol,
                x.OrderId.ToString(),
                x.Id.ToString(),
                x.QuantityRaw > 0 ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                x.Quantity,
                x.Price,
                x.Timestamp)
            {
                Fee = Math.Abs(x.Fee),
                FeeAsset = x.FeeAsset,
                Role = x.Maker == true ? SharedRole.Maker : x.Maker == false ? SharedRole.Taker : null
            }).ToArray(), nextToken);
        }

        EndpointOptions<CancelOrderRequest> ISpotOrderRestClient.CancelSpotOrderOptions { get; } = new EndpointOptions<CancelOrderRequest>(true);
        async Task<ExchangeWebResult<SharedId>> ISpotOrderRestClient.CancelSpotOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).CancelSpotOrderOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedId>(Exchange, new ArgumentError("Invalid order id"));

            var order = await Trading.CancelOrderAsync(orderId, ct: ct).ConfigureAwait(false);
            if (!order)
                return order.AsExchangeResult<SharedId>(Exchange, null, default);

            return order.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedId(order.Data.ToString()));
        }

        private SharedOrderStatus ParseOrderStatus(Enums.OrderStatus status)
        {
            if (status == Enums.OrderStatus.Active || status == Enums.OrderStatus.PartiallyFilled) return SharedOrderStatus.Open;
            if (status == Enums.OrderStatus.Canceled) return SharedOrderStatus.Canceled;
            return SharedOrderStatus.Filled;
        }

        private SharedOrderType ParseOrderType(Enums.OrderType type, OrderFlags? flags)
        {
            if (type == Enums.OrderType.ExchangeMarket) return SharedOrderType.Market;
            if (type == Enums.OrderType.ExchangeLimit && (flags != null && flags.Value.HasFlag(OrderFlags.PostOnly))) return SharedOrderType.LimitMaker;
            if (type == Enums.OrderType.ExchangeLimit) return SharedOrderType.Limit;
            if (type == Enums.OrderType.ExchangeFillOrKill) return SharedOrderType.Limit;
            if (type == Enums.OrderType.ExchangeImmediateOrCancel) return SharedOrderType.Limit;

            return SharedOrderType.Other;
        }

        private SharedTimeInForce? ParseTimeInForce(Enums.OrderType type, OrderFlags? flags)
        {
            if (type == OrderType.ExchangeFillOrKill) return SharedTimeInForce.FillOrKill;
            if (type == OrderType.ExchangeImmediateOrCancel) return SharedTimeInForce.ImmediateOrCancel;

            return null;
        }

        private Enums.OrderType GetPlaceOrderType(SharedOrderType type, SharedTimeInForce? tif)
        {
            if ((type == SharedOrderType.Limit || type == SharedOrderType.LimitMaker) && (tif == null || tif == SharedTimeInForce.GoodTillCanceled)) return Enums.OrderType.ExchangeLimit;
            if (type == SharedOrderType.Limit && tif == SharedTimeInForce.ImmediateOrCancel) return Enums.OrderType.ExchangeImmediateOrCancel;
            if (type == SharedOrderType.Limit && tif == SharedTimeInForce.FillOrKill) return Enums.OrderType.ExchangeFillOrKill;
            if (type == SharedOrderType.Market) return Enums.OrderType.ExchangeMarket;

            throw new ArgumentException($"The combination of order type `{type}` and time in force `{tif}` in invalid");
        }

        #endregion

        #region Deposit client

        EndpointOptions<GetDepositAddressesRequest> IDepositRestClient.GetDepositAddressesOptions { get; } = new EndpointOptions<GetDepositAddressesRequest>(true)
        {
            RequiredOptionalParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(GetDepositAddressesRequest.Network), typeof(string), "The network the deposit address should be for", "bitcoin")
            }
        };

        async Task<ExchangeWebResult<IEnumerable<SharedDepositAddress>>> IDepositRestClient.GetDepositAddressesAsync(GetDepositAddressesRequest request, CancellationToken ct)
        {
            var validationError = ((IDepositRestClient)this).GetDepositAddressesOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedDepositAddress>>(Exchange, validationError);

            var depositAddresses = await Account.GetDepositAddressAsync(request.Network!, WithdrawWallet.Deposit, false, ct).ConfigureAwait(false);
            if (!depositAddresses)
                return depositAddresses.AsExchangeResult<IEnumerable<SharedDepositAddress>>(Exchange, null, default);

            return depositAddresses.AsExchangeResult<IEnumerable<SharedDepositAddress>>(Exchange, TradingMode.Spot, new[] { new SharedDepositAddress(depositAddresses.Data.Data!.Asset, !string.IsNullOrEmpty(depositAddresses.Data.Data.PoolAddress) ? depositAddresses.Data.Data.PoolAddress : depositAddresses.Data.Data.Address)
            {
                Network = request.Network,
                TagOrMemo = !string.IsNullOrEmpty(depositAddresses.Data.Data.PoolAddress) ? depositAddresses.Data.Data.Address : null,
            }
            });
        }

        GetDepositsOptions IDepositRestClient.GetDepositsOptions { get; } = new GetDepositsOptions(SharedPaginationSupport.Descending, true, 1000);
        async Task<ExchangeWebResult<IEnumerable<SharedDeposit>>> IDepositRestClient.GetDepositsAsync(GetDepositsRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((IDepositRestClient)this).GetDepositsOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedDeposit>>(Exchange, validationError);

            // Determine page token
            DateTime? offset = null;
            if (pageToken is DateTimeToken timeToken)
                offset = timeToken.LastTime;

            // Get data
            var limit = request.Limit ?? 1000;
            var deposits = await Account.GetMovementsAsync(
                request.Asset,
                startTime: request.StartTime,
                endTime: offset ?? request.EndTime,
                limit: limit,
                ct: ct).ConfigureAwait(false);
            if (!deposits)
                return deposits.AsExchangeResult<IEnumerable<SharedDeposit>>(Exchange, null, default);

            var data = deposits.Data.Where(x => x.Quantity < 0);

            // Determine next token
            DateTimeToken? nextToken = null;
            if (deposits.Data.Count() == limit)
                nextToken = new DateTimeToken(deposits.Data.Min(x => x.StartTime));

            return deposits.AsExchangeResult<IEnumerable<SharedDeposit>>(Exchange, TradingMode.Spot, data.Where(x => x.Quantity > 0).Select(x => new SharedDeposit(x.Asset, x.Quantity, x.Status == "COMPLETED", x.StartTime)
            {
                Id = x.Id,
                TransactionId = x.TransactionId
            }).ToArray(), nextToken);
        }

        #endregion

        #region Order Book client
        GetOrderBookOptions IOrderBookRestClient.GetOrderBookOptions { get; } = new GetOrderBookOptions(new[] { 1, 25, 100 }, false);
        async Task<ExchangeWebResult<SharedOrderBook>> IOrderBookRestClient.GetOrderBookAsync(GetOrderBookRequest request, CancellationToken ct)
        {
            var validationError = ((IOrderBookRestClient)this).GetOrderBookOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedOrderBook>(Exchange, validationError);

            var result = await ExchangeData.GetOrderBookAsync(
                request.Symbol.GetSymbol(FormatSymbol),
                Precision.PrecisionLevel0,
                limit: request.Limit,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedOrderBook>(Exchange, null, default);

            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedOrderBook(result.Data.Asks, result.Data.Bids));
        }
        #endregion

        #region Trade History client

        GetTradeHistoryOptions ITradeHistoryRestClient.GetTradeHistoryOptions { get; } = new GetTradeHistoryOptions(SharedPaginationSupport.Descending, true, 10000, false);
        async Task<ExchangeWebResult<IEnumerable<SharedTrade>>> ITradeHistoryRestClient.GetTradeHistoryAsync(GetTradeHistoryRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((ITradeHistoryRestClient)this).GetTradeHistoryOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedTrade>>(Exchange, validationError);

            // Determine page token
            DateTime? fromTimestamp = null;
            if (pageToken is DateTimeToken dateTimeToken)
                fromTimestamp = dateTimeToken.LastTime;

            // Get data
            var limit = request.Limit ?? 10000;
            var result = await ExchangeData.GetTradeHistoryAsync(
                request.Symbol.GetSymbol(FormatSymbol),
                startTime: request.StartTime,
                endTime: fromTimestamp ?? request.EndTime,
                limit: limit,
                sorting: Sorting.NewFirst,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedTrade>>(Exchange, null, default);

            // Get next token
            DateTimeToken? nextToken = null;
            if (result.Data.Count() == limit)
                nextToken = new DateTimeToken(result.Data.Min(o => o.Timestamp.AddMilliseconds(-1)));

            // Return
            return result.AsExchangeResult<IEnumerable<SharedTrade>>(Exchange, request.Symbol.TradingMode, result.Data./*Where(x => x. < request.EndTime).*/Select(x => new SharedTrade(Math.Abs(x.Quantity), x.Price, x.Timestamp)
            {
                Side = x.Quantity > 0 ? SharedOrderSide.Buy : SharedOrderSide.Sell
            }).ToArray(), nextToken);
        }
        #endregion

        #region Withdrawal client

        GetWithdrawalsOptions IWithdrawalRestClient.GetWithdrawalsOptions { get; } = new GetWithdrawalsOptions(SharedPaginationSupport.Descending, true, 1000);
        async Task<ExchangeWebResult<IEnumerable<SharedWithdrawal>>> IWithdrawalRestClient.GetWithdrawalsAsync(GetWithdrawalsRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((IWithdrawalRestClient)this).GetWithdrawalsOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedWithdrawal>>(Exchange, validationError);

            // Determine page token
            DateTime? offset = null;
            if (pageToken is DateTimeToken timeToken)
                offset = timeToken.LastTime;

            // Get data
            var limit = request.Limit ?? 1000;
            var withdrawals = await Account.GetMovementsAsync(
                request.Asset,
                startTime: request.StartTime,
                endTime: offset ?? request.EndTime,
                limit: limit,
                ct: ct).ConfigureAwait(false);
            if (!withdrawals)
                return withdrawals.AsExchangeResult<IEnumerable<SharedWithdrawal>>(Exchange, null, default);

            var data = withdrawals.Data.Where(x => x.Quantity < 0);

            // Determine next token
            DateTimeToken? nextToken = null;
            if (withdrawals.Data.Count() == limit)
                nextToken = new DateTimeToken(withdrawals.Data.Min(x => x.StartTime));

            return withdrawals.AsExchangeResult<IEnumerable<SharedWithdrawal>>(Exchange, TradingMode.Spot, data.Select(x => new SharedWithdrawal(x.Asset, x.Address, Math.Abs(x.Quantity), x.Status == "COMPLETED", x.StartTime)
            {
                Id = x.Id,
                TransactionId = x.TransactionId
            }).ToArray(), nextToken);
        }

        #endregion

        #region Withdraw client

        WithdrawOptions IWithdrawRestClient.WithdrawOptions { get; } = new WithdrawOptions();

        async Task<ExchangeWebResult<SharedId>> IWithdrawRestClient.WithdrawAsync(WithdrawRequest request, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(request.Network))
                return new ExchangeWebResult<SharedId>(Exchange, new ArgumentError("Network is required"));

            var validationError = ((IWithdrawRestClient)this).WithdrawOptions.ValidateRequest(Exchange, request, TradingMode.Spot, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            // Get data
            var withdrawal = await Account.WithdrawV2Async(
                request.Network!,
                WithdrawWallet.Deposit,
                request.Quantity,
                address: request.Address,
                paymentId: request.AddressTag,
                ct: ct).ConfigureAwait(false);
            if (!withdrawal)
                return withdrawal.AsExchangeResult<SharedId>(Exchange, null, default);

            return withdrawal.AsExchangeResult(Exchange, TradingMode.Spot, new SharedId(withdrawal.Data.Data.WithdrawalId.ToString()));
        }

        #endregion

        #region Fee Client
        EndpointOptions<GetFeeRequest> IFeeRestClient.GetFeeOptions { get; } = new EndpointOptions<GetFeeRequest>(true);

        async Task<ExchangeWebResult<SharedFee>> IFeeRestClient.GetFeesAsync(GetFeeRequest request, CancellationToken ct)
        {
            var validationError = ((IFeeRestClient)this).GetFeeOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFee>(Exchange, validationError);

            // Get data
            var result = await Account.Get30DaySummaryAndFeesAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedFee>(Exchange, null, default);

            // Return
            return result.AsExchangeResult(Exchange, TradingMode.Spot, new SharedFee(result.Data.Fees.MakerFees.MakerFee * 100, result.Data.Fees.TakerFees.TakerFeeCrypto * 100));
        }
        #endregion
    }
}
