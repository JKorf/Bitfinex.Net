using Bitfinex.Net.Enums;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using Bitfinex.Net.Objects.Models;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
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
        private const string _exchangeName = "Bitfinex";
        private const string _topicId = "BitfinexSpot";

        public string Exchange => BitfinexExchange.ExchangeName;
        public TradingMode[] SupportedTradingModes { get; } = new[] { TradingMode.Spot };
        public void SetDefaultExchangeParameter(string key, object value) => ExchangeParameters.SetStaticParameter(Exchange, key, value);
        public void ResetDefaultExchangeParameters() => ExchangeParameters.ResetStaticParameters();

        #region Kline client

        GetKlinesOptions IKlineRestClient.GetKlinesOptions { get; } = new GetKlinesOptions(_exchangeName, true, true, true, 10000, false);
        Task<HttpResult<SharedKline[]>> IKlineRestClient.GetKlinesAsync(GetKlinesRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<IKlineRestClient, GetKlinesRequest, SharedKline[]>(
                this,
                client => client.GetKlinesOptions,
                request,
                async () =>
                {
                    var interval = (Enums.KlineInterval)request.Interval;
                    if (!Enum.IsDefined(typeof(Enums.KlineInterval), interval))
                        return HttpResult.Fail<SharedKline[]>(Exchange, ArgumentError.Invalid(nameof(GetKlinesRequest.Interval), "Interval not supported"));

                    var direction = request.Direction ?? DataDirection.Ascending;
                    var symbol = request.Symbol!.GetSymbol(FormatSymbol);
                    var limit = request.Limit ?? 10000;
                    var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

                    // Get data
                    var result = await ExchangeData.GetKlinesAsync(
                        symbol,
                        interval,
                        startTime: pageParams.StartTime,
                        endTime: pageParams.EndTime,
                        limit: limit,
                        sorting: direction == DataDirection.Ascending ? Sorting.OldFirst : Sorting.NewFirst,
                        ct: ct
                        ).ConfigureAwait(false);

                    if (!result.Success)
                        return HttpResult.Fail<SharedKline[]>(result);

                    var nextPageRequest = Pagination.GetNextPageRequest(
                            () => direction == DataDirection.Ascending
                            ? Pagination.NextPageFromTime(pageParams, result.Data.Max(x => x.OpenTime))
                            : Pagination.NextPageFromTime(pageParams, result.Data.Min(x => x.OpenTime)),
                            result.Data.Length,
                            result.Data.Select(x => x.OpenTime),
                            request.StartTime,
                            request.EndTime ?? DateTime.UtcNow,
                            pageParams);

                    // Return
                    return HttpResult.Ok<SharedKline[]>(result, ExchangeHelpers.ApplyFilter(result.Data, x => x.OpenTime, request.StartTime, request.EndTime, direction)
                            .Select(x =>
                                new SharedKline(request.Symbol, symbol, x.OpenTime, x.ClosePrice, x.HighPrice, x.LowPrice, x.OpenPrice, x.Volume))
                            .ToArray(), nextPageRequest);

                });
        }

        #endregion

        #region Asset client
        EndpointOptions<GetAssetRequest, IAssetsRestClient> IAssetsRestClient.GetAssetOptions { get; } = new EndpointOptions<GetAssetRequest, IAssetsRestClient>(_exchangeName, false);
        Task<HttpResult<SharedAsset>> IAssetsRestClient.GetAssetAsync(GetAssetRequest request, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<IAssetsRestClient, GetAssetRequest, SharedAsset>(
                this,
                client => client.GetAssetOptions,
                request,
                async () =>
                {
                    // Execute needed config requests in parallel
                    var assetSymbols = ExchangeData.GetAssetSymbolsAsync(ct: ct);
                    var assetList = ExchangeData.GetAssetsListAsync(ct: ct);
                    var assetMethods = ExchangeData.GetAssetDepositWithdrawalMethodsAsync(ct: ct);
                    var assetFees = ExchangeData.GetAssetWithdrawalFeesAsync(ct: ct);
                    var assetTxStatus = ExchangeData.GetDepositWithdrawalStatusAsync(ct: ct);
                    await Task.WhenAll(assetList, assetMethods).ConfigureAwait(false);
                    if (!assetSymbols.Result.Success)
                        return HttpResult.Fail<SharedAsset>(assetSymbols.Result);
                    if (!assetList.Result.Success)
                        return HttpResult.Fail<SharedAsset>(assetList.Result);
                    if (!assetMethods.Result.Success)
                        return HttpResult.Fail<SharedAsset>(assetMethods.Result);
                    if (!assetFees.Result.Success)
                        return HttpResult.Fail<SharedAsset>(assetFees.Result);
                    if (!assetTxStatus.Result.Success)
                        return HttpResult.Fail<SharedAsset>(assetTxStatus.Result);

                    var asset = assetList.Result.Data.SingleOrDefault(x => x.Name == request.Asset);
                    if (asset == null)
                        return HttpResult.Fail<SharedAsset>(assetList.Result, new ServerError(new ErrorInfo(ErrorType.UnknownAsset, "Not found")));

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
                        }).ToArray()
                    };


                    return HttpResult.Ok(assetList.Result, assetResult);

                });
        }

        EndpointOptions<GetAssetsRequest, IAssetsRestClient> IAssetsRestClient.GetAssetsOptions { get; } = new EndpointOptions<GetAssetsRequest, IAssetsRestClient>(_exchangeName, false);

        Task<HttpResult<SharedAsset[]>> IAssetsRestClient.GetAssetsAsync(GetAssetsRequest request, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<IAssetsRestClient, GetAssetsRequest, SharedAsset[]>(
                this,
                client => client.GetAssetsOptions,
                request,
                async () =>
                {
                    // Execute needed config requests in parallel
                    var assetSymbols = ExchangeData.GetAssetSymbolsAsync(ct: ct);
                    var assetList = ExchangeData.GetAssetsListAsync(ct: ct);
                    var assetMethods = ExchangeData.GetAssetDepositWithdrawalMethodsAsync(ct: ct);
                    var assetFees = ExchangeData.GetAssetWithdrawalFeesAsync(ct: ct);
                    var assetTxStatus = ExchangeData.GetDepositWithdrawalStatusAsync(ct: ct);
                    await Task.WhenAll(assetList, assetMethods).ConfigureAwait(false);
                    if (!assetSymbols.Result.Success)
                        return HttpResult.Fail<SharedAsset[]>(assetSymbols.Result);
                    if (!assetList.Result.Success)
                        return HttpResult.Fail<SharedAsset[]>(assetList.Result);
                    if (!assetMethods.Result.Success)
                        return HttpResult.Fail<SharedAsset[]>(assetMethods.Result);
                    if (!assetFees.Result.Success)
                        return HttpResult.Fail<SharedAsset[]>(assetFees.Result);
                    if (!assetTxStatus.Result.Success)
                        return HttpResult.Fail<SharedAsset[]>(assetTxStatus.Result);

                    return HttpResult.Ok(assetList.Result, assetList.Result.Data.Select(x =>
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
                        }).Where(x => x != null).ToArray()!);

                });
        }

        #endregion

        #region Spot Symbol client

        EndpointOptions<GetSymbolsRequest, ISpotSymbolRestClient> ISpotSymbolRestClient.GetSpotSymbolsOptions { get; } = new EndpointOptions<GetSymbolsRequest, ISpotSymbolRestClient>(_exchangeName, false);
        Task<HttpResult<SharedSpotSymbol[]>> ISpotSymbolRestClient.GetSpotSymbolsAsync(GetSymbolsRequest request, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<ISpotSymbolRestClient, GetSymbolsRequest, SharedSpotSymbol[]>(
                this,
                client => client.GetSpotSymbolsOptions,
                request,
                async () =>
                {
                    var result = await ExchangeData.GetSymbolsAsync(ct: ct).ConfigureAwait(false);
                    if (!result.Success)
                        return HttpResult.Fail<SharedSpotSymbol[]>(result);

                    var response = result.Data.Select(s =>
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
                    }).ToArray();

                    ExchangeSymbolCache.UpdateSymbolInfo(_topicId, response);
                    return HttpResult.Ok(result, response);
                });
        }

        private (string BaseAsset, string QuoteAsset) GetAssets(string input)
        {
            string baseAsset;
            string quoteAsset;
            if (input.Contains(":"))
            {
                var split = input.Split(':');
                baseAsset = split[0];
                quoteAsset = split[1];
            }
            else
            {
                baseAsset = input.Substring(0, 3);
                quoteAsset = input.Substring(3);
            }

            return (BitfinexExchange.AssetAliases.ExchangeToCommonName(baseAsset), BitfinexExchange.AssetAliases.ExchangeToCommonName(quoteAsset));
        }
        async Task<ExchangeCallResult<SharedSymbol[]>> ISpotSymbolRestClient.GetSpotSymbolsForBaseAssetAsync(string baseAsset)
        {
            if (!ExchangeSymbolCache.HasCached(_topicId))
            {
                var symbols = await ((ISpotSymbolRestClient)this).GetSpotSymbolsAsync(new GetSymbolsRequest()).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<SharedSymbol[]>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<SharedSymbol[]>.Ok(Exchange, ExchangeSymbolCache.GetSymbolsForBaseAsset(_topicId, baseAsset));
        }

        async Task<ExchangeCallResult<bool>> ISpotSymbolRestClient.SupportsSpotSymbolAsync(SharedSymbol symbol)
        {
            if (symbol.TradingMode != TradingMode.Spot)
                throw new ArgumentException(nameof(symbol), "Only Spot symbols allowed");

            if (!ExchangeSymbolCache.HasCached(_topicId))
            {
                var symbols = await ((ISpotSymbolRestClient)this).GetSpotSymbolsAsync(new GetSymbolsRequest()).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicId, symbol));
        }

        async Task<ExchangeCallResult<bool>> ISpotSymbolRestClient.SupportsSpotSymbolAsync(string symbolName)
        {
            if (!ExchangeSymbolCache.HasCached(_topicId))
            {
                var symbols = await ((ISpotSymbolRestClient)this).GetSpotSymbolsAsync(new GetSymbolsRequest()).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicId, symbolName));
        }
        #endregion

        #region Ticker client

        GetSpotTickerOptions ISpotTickerRestClient.GetSpotTickerOptions { get; } = new GetSpotTickerOptions(_exchangeName);
        Task<HttpResult<SharedSpotTicker>> ISpotTickerRestClient.GetSpotTickerAsync(GetTickerRequest request, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<ISpotTickerRestClient, GetTickerRequest, SharedSpotTicker>(
                this,
                client => client.GetSpotTickerOptions,
                request,
                async () =>
                {
                    var result = await ExchangeData.GetTickerAsync(request.Symbol!.GetSymbol(FormatSymbol), ct).ConfigureAwait(false);
                    if (!result.Success)
                        return HttpResult.Fail<SharedSpotTicker>(result);

                    return HttpResult.Ok(result, new SharedSpotTicker(ExchangeSymbolCache.ParseSymbol(_topicId, result.Data.Symbol), result.Data.Symbol, result.Data.LastPrice, result.Data.HighPrice, result.Data.LowPrice, result.Data.Volume, Math.Round(result.Data.DailyChangePercentage * 100, 2)));

                });
        }

        GetSpotTickersOptions ISpotTickerRestClient.GetSpotTickersOptions { get; } = new GetSpotTickersOptions(_exchangeName);
        Task<HttpResult<SharedSpotTicker[]>> ISpotTickerRestClient.GetSpotTickersAsync(GetTickersRequest request, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<ISpotTickerRestClient, GetTickersRequest, SharedSpotTicker[]>(
                this,
                client => client.GetSpotTickersOptions,
                request,
                async () =>
                {
                    var result = await ExchangeData.GetTickersAsync(ct: ct).ConfigureAwait(false);
                    if (!result.Success)
                        return HttpResult.Fail<SharedSpotTicker[]>(result);

                    return HttpResult.Ok(result, result.Data.Select(x => new SharedSpotTicker(ExchangeSymbolCache.ParseSymbol(_topicId, x.Symbol), x.Symbol, x.LastPrice, x.HighPrice, x.LowPrice, x.Volume, Math.Round(x.DailyChangePercentage * 100, 2))).ToArray());

                });
        }

        #endregion

        #region Book Ticker client

        EndpointOptions<GetBookTickerRequest, IBookTickerRestClient> IBookTickerRestClient.GetBookTickerOptions { get; } = new EndpointOptions<GetBookTickerRequest, IBookTickerRestClient>(_exchangeName, false);
        Task<HttpResult<SharedBookTicker>> IBookTickerRestClient.GetBookTickerAsync(GetBookTickerRequest request, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<IBookTickerRestClient, GetBookTickerRequest, SharedBookTicker>(
                this,
                client => client.GetBookTickerOptions,
                request,
                async () =>
                {
                    var symbol = request.Symbol!.GetSymbol(FormatSymbol);
                    var resultTicker = await ExchangeData.GetOrderBookAsync(symbol, Precision.PrecisionLevel0, 1, ct: ct).ConfigureAwait(false);
                    if (!resultTicker.Success)
                        return HttpResult.Fail<SharedBookTicker>(resultTicker);

                    return HttpResult.Ok(resultTicker, new SharedBookTicker(
                        ExchangeSymbolCache.ParseSymbol(_topicId, symbol),
                        symbol,
                        resultTicker.Data.Asks[0].Price,
                        resultTicker.Data.Asks[0].Quantity,
                        resultTicker.Data.Bids[0].Price,
                        resultTicker.Data.Bids[0].Quantity));

                });
        }

        #endregion

        #region Recent Trade client

        GetRecentTradesOptions IRecentTradeRestClient.GetRecentTradesOptions { get; } = new GetRecentTradesOptions(_exchangeName, 10000, false);

        Task<HttpResult<SharedTrade[]>> IRecentTradeRestClient.GetRecentTradesAsync(GetRecentTradesRequest request, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<IRecentTradeRestClient, GetRecentTradesRequest, SharedTrade[]>(
                this,
                client => client.GetRecentTradesOptions,
                request,
                async () =>
                {
                    var symbol = request.Symbol!.GetSymbol(FormatSymbol);
                    var result = await ExchangeData.GetTradeHistoryAsync(
                        symbol,
                        limit: request.Limit,
                        ct: ct).ConfigureAwait(false);
                    if (!result.Success)
                        return HttpResult.Fail<SharedTrade[]>(result);

                    return HttpResult.Ok(result, result.Data.Select(x =>
                    new SharedTrade(request.Symbol, symbol, Math.Abs(x.Quantity), x.Price, x.Timestamp)
                    {
                        Side = x.Quantity > 0 ? SharedOrderSide.Buy : SharedOrderSide.Sell
                    }).ToArray());

                });
        }

        #endregion

        #region Balance client
        GetBalancesOptions IBalanceRestClient.GetBalancesOptions { get; } = new GetBalancesOptions(_exchangeName, AccountTypeFilter.Funding, AccountTypeFilter.Spot, AccountTypeFilter.Margin);

        Task<HttpResult<SharedBalance[]>> IBalanceRestClient.GetBalancesAsync(GetBalancesRequest request, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<IBalanceRestClient, GetBalancesRequest, SharedBalance[]>(
                this,
                client => client.GetBalancesOptions,
                request,
                async () =>
                {
                    var result = await Account.GetBalancesAsync(ct: ct).ConfigureAwait(false);
                    if (!result.Success)
                        return HttpResult.Fail<SharedBalance[]>(result);

                    var filterType = request.AccountType?.IsMarginAccount() == true ? WalletType.Margin : request.AccountType == SharedAccountType.Funding ? WalletType.Funding : WalletType.Exchange;
                    return HttpResult.Ok(result, result.Data.Where(x => x.Type == filterType).Select(x =>
                        new SharedBalance(BitfinexExchange.AssetAliases.ExchangeToCommonName(x.Asset), x.Available ?? 0, x.Total)).ToArray());

                });
        }

        #endregion

        #region Spot Order client

        SharedFeeDeductionType ISpotOrderRestClient.SpotFeeDeductionType => SharedFeeDeductionType.DeductFromOutput;
        SharedFeeAssetType ISpotOrderRestClient.SpotFeeAssetType => SharedFeeAssetType.OutputAsset;
        SharedOrderType[] ISpotOrderRestClient.SpotSupportedOrderTypes { get; } = new[] { SharedOrderType.Limit, SharedOrderType.Market, SharedOrderType.LimitMaker };
        SharedTimeInForce[] ISpotOrderRestClient.SpotSupportedTimeInForce { get; } = new[] { SharedTimeInForce.GoodTillCanceled, SharedTimeInForce.ImmediateOrCancel, SharedTimeInForce.FillOrKill };
        SharedQuantitySupport ISpotOrderRestClient.SpotSupportedOrderQuantity { get; } = new SharedQuantitySupport(
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset);

        string ISpotOrderRestClient.GenerateClientOrderId() => ExchangeHelpers.RandomLong(10).ToString();

        PlaceSpotOrderOptions ISpotOrderRestClient.PlaceSpotOrderOptions { get; } = new PlaceSpotOrderOptions(_exchangeName);
        Task<HttpResult<SharedId>> ISpotOrderRestClient.PlaceSpotOrderAsync(PlaceSpotOrderRequest request, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<ISpotOrderRestClient, PlaceSpotOrderRequest, SharedId>(
                this,
                client => client.PlaceSpotOrderOptions,
                request,
                async () =>
                {
                    int clientOrderId = 0;
                    if (request.ClientOrderId != null && !int.TryParse(request.ClientOrderId, out clientOrderId))
                        return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(PlaceSpotOrderRequest.ClientOrderId), "ClientOrderId needs to be parsable to `int` for `Bitfinex`"));

                    var result = await Trading.PlaceOrderAsync(
                        request.Symbol!.GetSymbol(FormatSymbol),
                        request.Side == SharedOrderSide.Buy ? Enums.OrderSide.Buy : Enums.OrderSide.Sell,
                        GetPlaceOrderType(request.OrderType, request.TimeInForce),
                        quantity: request.Quantity?.QuantityInBaseAsset ?? 0,
                        flags: request.OrderType == SharedOrderType.LimitMaker ? Enums.OrderFlags.PostOnly : null,
                        price: request.Price ?? 0,
                        clientOrderId: request.ClientOrderId != null ? clientOrderId : null,
                        ct: ct).ConfigureAwait(false);

                    if (!result.Success)
                        return HttpResult.Fail<SharedId>(result);

                    return HttpResult.Ok(result, new SharedId(result.Data.Data!.Id.ToString()));

                });
        }

        EndpointOptions<GetOrderRequest, ISpotOrderRestClient> ISpotOrderRestClient.GetSpotOrderOptions { get; } = new EndpointOptions<GetOrderRequest, ISpotOrderRestClient>(_exchangeName, true);
        Task<HttpResult<SharedSpotOrder>> ISpotOrderRestClient.GetSpotOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<ISpotOrderRestClient, GetOrderRequest, SharedSpotOrder>(
                this,
                client => client.GetSpotOrderOptions,
                request,
                async () =>
                {
                    if (!long.TryParse(request.OrderId, out var orderId))
                        return HttpResult.Fail<SharedSpotOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderRequest.OrderId), "Invalid order id"));

                    var symbol = request.Symbol!.GetSymbol(FormatSymbol);
                    var result = await Trading.GetOpenOrdersAsync(symbol, new[] { orderId }, ct: ct).ConfigureAwait(false);
                    if (!result.Success)
                        return HttpResult.Fail<SharedSpotOrder>(result);

                    if (!result.Data.Any())
                        result = await Trading.GetClosedOrdersAsync(symbol, new[] { orderId }, ct: ct).ConfigureAwait(false);

                    if (!result.Data.Any())
                        return HttpResult.Fail<SharedSpotOrder>(result, new ServerError(new ErrorInfo(ErrorType.UnknownOrder, $"Order with id {orderId} not found")));

                    var order = result.Data.Single();
                    return HttpResult.Ok(result, new SharedSpotOrder(
                        ExchangeSymbolCache.ParseSymbol(_topicId, order.Symbol),
                        order.Symbol,
                        order.Id.ToString(),
                        ParseOrderType(order.Type, order.Flags),
                        order.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                        ParseOrderStatus(order.Status),
                        order.CreateTime)
                    {
                        ClientOrderId = order.ClientOrderId?.ToString(),
                        AveragePrice = order.PriceAverage == 0 ? null : order.PriceAverage,
                        OrderQuantity = new SharedOrderQuantity(order.Quantity),
                        QuantityFilled = new SharedOrderQuantity(order.Quantity - order.QuantityRemaining),
                        TimeInForce = ParseTimeInForce(order.Type, order.Flags),
                        UpdateTime = order.UpdateTime,
                        IsTriggerOrder = order.Type == OrderType.ExchangeStop || order.Type == OrderType.ExchangeStopLimit,
                        OrderPrice = order.Type == OrderType.ExchangeStop || order.Type == OrderType.ExchangeStopLimit ? order.PriceAuxilliaryLimit : order.Price,
                        TriggerPrice = order.Type == OrderType.ExchangeStop || order.Type == OrderType.ExchangeStopLimit ? order.Price : null
                    });

                });
        }

        EndpointOptions<GetOpenOrdersRequest, ISpotOrderRestClient> ISpotOrderRestClient.GetOpenSpotOrdersOptions { get; } = new EndpointOptions<GetOpenOrdersRequest, ISpotOrderRestClient>(_exchangeName, true);
        Task<HttpResult<SharedSpotOrder[]>> ISpotOrderRestClient.GetOpenSpotOrdersAsync(GetOpenOrdersRequest request, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<ISpotOrderRestClient, GetOpenOrdersRequest, SharedSpotOrder[]>(
                this,
                client => client.GetOpenSpotOrdersOptions,
                request,
                async () =>
                {
                    var symbol = request.Symbol?.GetSymbol(FormatSymbol);
                    var result = await Trading.GetOpenOrdersAsync(symbol, ct: ct).ConfigureAwait(false);
                    if (!result.Success)
                        return HttpResult.Fail<SharedSpotOrder[]>(result);

                    return HttpResult.Ok(result, result.Data.Select(x => new SharedSpotOrder(
                        ExchangeSymbolCache.ParseSymbol(_topicId, x.Symbol),
                        x.Symbol,
                        x.Id.ToString(),
                        ParseOrderType(x.Type, x.Flags),
                        x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                        ParseOrderStatus(x.Status),
                        x.CreateTime)
                    {
                        ClientOrderId = x.ClientOrderId?.ToString(),
                        AveragePrice = x.PriceAverage == 0 ? null : x.PriceAverage,
                        OrderQuantity = new SharedOrderQuantity(x.Quantity),
                        QuantityFilled = new SharedOrderQuantity(x.Quantity - x.QuantityRemaining),
                        TimeInForce = ParseTimeInForce(x.Type, x.Flags),
                        UpdateTime = x.UpdateTime,
                        IsTriggerOrder = x.Type == OrderType.ExchangeStop || x.Type == OrderType.ExchangeStopLimit,
                        OrderPrice = x.Type == OrderType.ExchangeStop || x.Type == OrderType.ExchangeStopLimit ? x.PriceAuxilliaryLimit : x.Price,
                        TriggerPrice = x.Type == OrderType.ExchangeStop || x.Type == OrderType.ExchangeStopLimit ? x.Price : null
                    }).ToArray());

                });
        }

        GetSpotClosedOrdersOptions ISpotOrderRestClient.GetClosedSpotOrdersOptions { get; } = new GetSpotClosedOrdersOptions(_exchangeName, false, true, true, 2500);
        Task<HttpResult<SharedSpotOrder[]>> ISpotOrderRestClient.GetClosedSpotOrdersAsync(GetClosedOrdersRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<ISpotOrderRestClient, GetClosedOrdersRequest, SharedSpotOrder[]>(
                this,
                client => client.GetClosedSpotOrdersOptions,
                request,
                async () =>
                {
                    var direction = DataDirection.Descending;
                    var symbol = request.Symbol!.GetSymbol(FormatSymbol);
                    var limit = request.Limit ?? 2500;
                    var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

                    // Get data
                    var result = await Trading.GetClosedOrdersAsync(
                        symbol,
                        startTime: pageParams.StartTime,
                        endTime: pageParams.EndTime,
                        limit: limit,
                        ct: ct).ConfigureAwait(false);
                    if (!result.Success)
                        return HttpResult.Fail<SharedSpotOrder[]>(result);

                    var nextPageRequest = Pagination.GetNextPageRequest(
                            () => Pagination.NextPageFromTime(pageParams, result.Data.Min(x => x.CreateTime)),
                            result.Data.Length,
                            result.Data.Select(x => x.CreateTime),
                            request.StartTime,
                            request.EndTime ?? DateTime.UtcNow,
                            pageParams);

                    // Return
                    return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data, x => x.CreateTime, request.StartTime, request.EndTime, direction)
                        .Select(x =>
                            new SharedSpotOrder(
                                ExchangeSymbolCache.ParseSymbol(_topicId, x.Symbol),
                                x.Symbol,
                                x.Id.ToString(),
                                ParseOrderType(x.Type, x.Flags),
                                x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                                ParseOrderStatus(x.Status),
                                x.CreateTime)
                            {
                                ClientOrderId = x.ClientOrderId?.ToString(),
                                AveragePrice = x.PriceAverage == 0 ? null : x.PriceAverage,
                                OrderQuantity = new SharedOrderQuantity(x.Quantity),
                                QuantityFilled = new SharedOrderQuantity(x.Quantity - x.QuantityRemaining),
                                TimeInForce = ParseTimeInForce(x.Type, x.Flags),
                                UpdateTime = x.UpdateTime,
                                IsTriggerOrder = x.Type == OrderType.ExchangeStop || x.Type == OrderType.ExchangeStopLimit,
                                OrderPrice = x.Type == OrderType.ExchangeStop || x.Type == OrderType.ExchangeStopLimit ? x.PriceAuxilliaryLimit : x.Price,
                                TriggerPrice = x.Type == OrderType.ExchangeStop || x.Type == OrderType.ExchangeStopLimit ? x.Price : null
                            }).ToArray(), nextPageRequest);

                });
        }

        EndpointOptions<GetOrderTradesRequest, ISpotOrderRestClient> ISpotOrderRestClient.GetSpotOrderTradesOptions { get; } = new EndpointOptions<GetOrderTradesRequest, ISpotOrderRestClient>(_exchangeName, true);
        Task<HttpResult<SharedUserTrade[]>> ISpotOrderRestClient.GetSpotOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<ISpotOrderRestClient, GetOrderTradesRequest, SharedUserTrade[]>(
                this,
                client => client.GetSpotOrderTradesOptions,
                request,
                async () =>
                {
                    if (!long.TryParse(request.OrderId, out var orderId))
                        return HttpResult.Fail<SharedUserTrade[]>(Exchange, ArgumentError.Invalid(nameof(GetOrderTradesRequest.OrderId), "Invalid order id"));

                    var order = await Trading.GetOrderTradesAsync(
                        request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
                    if (!order.Success)
                        return HttpResult.Fail<SharedUserTrade[]>(order);

                    return HttpResult.Ok(order, order.Data.Select(x => new SharedUserTrade(
                        ExchangeSymbolCache.ParseSymbol(_topicId, x.Symbol),
                        x.Symbol,
                        x.OrderId.ToString(),
                        x.Id.ToString(),
                        x.QuantityRaw > 0 ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                        x.Quantity,
                        x.Price,
                        x.Timestamp)
                    {
                        Fee = Math.Abs(x.Fee),
                        FeeAsset = BitfinexExchange.AssetAliases.ExchangeToCommonName(x.FeeAsset),
                        Role = x.Maker == true ? SharedRole.Maker : x.Maker == false ? SharedRole.Taker : null,
                        ClientOrderId = x.ClientOrderId?.ToString()
                    }).ToArray());

                });
        }

        GetSpotUserTradesOptions ISpotOrderRestClient.GetSpotUserTradesOptions { get; } = new GetSpotUserTradesOptions(_exchangeName, false, true, true, 2500);
        Task<HttpResult<SharedUserTrade[]>> ISpotOrderRestClient.GetSpotUserTradesAsync(GetUserTradesRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<ISpotOrderRestClient, GetUserTradesRequest, SharedUserTrade[]>(
                this,
                client => client.GetSpotUserTradesOptions,
                request,
                async () =>
                {
                    var direction = DataDirection.Descending;
                    var symbol = request.Symbol!.GetSymbol(FormatSymbol);
                    var limit = request.Limit ?? 1000;
                    var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

                    // Get data
                    var result = await Trading.GetUserTradesAsync(
                        symbol,
                        startTime: pageParams.StartTime,
                        endTime: pageParams.EndTime,
                        limit: limit,
                        ct: ct).ConfigureAwait(false);
                    if (!result.Success)
                        return HttpResult.Fail<SharedUserTrade[]>(result);

                    var nextPageRequest = Pagination.GetNextPageRequest(
                            () => Pagination.NextPageFromTime(pageParams, result.Data.Min(x => x.Timestamp)),
                            result.Data.Length,
                            result.Data.Select(x => x.Timestamp),
                            request.StartTime,
                            request.EndTime ?? DateTime.UtcNow,
                            pageParams);

                    // Return
                    return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data, x => x.Timestamp, request.StartTime, request.EndTime, direction)
                        .Select(x => new SharedUserTrade(
                            ExchangeSymbolCache.ParseSymbol(_topicId, x.Symbol),
                            x.Symbol,
                            x.OrderId.ToString(),
                            x.Id.ToString(),
                            x.QuantityRaw > 0 ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                            x.Quantity,
                            x.Price,
                            x.Timestamp)
                        {
                            Fee = Math.Abs(x.Fee),
                            FeeAsset = BitfinexExchange.AssetAliases.ExchangeToCommonName(x.FeeAsset),
                            Role = x.Maker == true ? SharedRole.Maker : x.Maker == false ? SharedRole.Taker : null,
                            ClientOrderId = x.ClientOrderId?.ToString()
                        })
                        .ToArray(), nextPageRequest);

                });
        }

        EndpointOptions<CancelOrderRequest, ISpotOrderRestClient> ISpotOrderRestClient.CancelSpotOrderOptions { get; } = new EndpointOptions<CancelOrderRequest, ISpotOrderRestClient>(_exchangeName, true);
        Task<HttpResult<SharedId>> ISpotOrderRestClient.CancelSpotOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<ISpotOrderRestClient, CancelOrderRequest, SharedId>(
                this,
                client => client.CancelSpotOrderOptions,
                request,
                async () =>
                {
                    if (!long.TryParse(request.OrderId, out var orderId))
                        return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

                    var order = await Trading.CancelOrderAsync(orderId, ct: ct).ConfigureAwait(false);
                    if (!order.Success)
                        return HttpResult.Fail<SharedId>(order);

                    return HttpResult.Ok(order, new SharedId(order.Data.ToString()));

                });
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

        EndpointOptions<GetDepositAddressesRequest, IDepositRestClient> IDepositRestClient.GetDepositAddressesOptions { get; } = new EndpointOptions<GetDepositAddressesRequest, IDepositRestClient>(_exchangeName, true)
        {
            RequiredOptionalParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(GetDepositAddressesRequest.Network), typeof(string), "The network the deposit address should be for", "bitcoin")
            }
        };

        Task<HttpResult<SharedDepositAddress[]>> IDepositRestClient.GetDepositAddressesAsync(GetDepositAddressesRequest request, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<IDepositRestClient, GetDepositAddressesRequest, SharedDepositAddress[]>(
                this,
                client => client.GetDepositAddressesOptions,
                request,
                async () =>
                {
                    var depositAddresses = await Account.GetDepositAddressAsync(request.Network!, WithdrawWallet.Deposit, false, ct).ConfigureAwait(false);
                    if (!depositAddresses.Success)
                        return HttpResult.Fail<SharedDepositAddress[]>(depositAddresses);

                    return HttpResult.Ok(depositAddresses, new[] {
                new SharedDepositAddress(BitfinexExchange.AssetAliases.ExchangeToCommonName(depositAddresses.Data.Data!.Asset), !string.IsNullOrEmpty(depositAddresses.Data.Data.PoolAddress) ? depositAddresses.Data.Data.PoolAddress : depositAddresses.Data.Data.Address)
                {
                    Network = request.Network,
                    TagOrMemo = !string.IsNullOrEmpty(depositAddresses.Data.Data.PoolAddress) ? depositAddresses.Data.Data.Address : null,
                }
            });

                });
        }

        GetDepositsOptions IDepositRestClient.GetDepositsOptions { get; } = new GetDepositsOptions(_exchangeName, false, true, true, 1000);
        Task<HttpResult<SharedDeposit[]>> IDepositRestClient.GetDepositsAsync(GetDepositsRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<IDepositRestClient, GetDepositsRequest, SharedDeposit[]>(
                this,
                client => client.GetDepositsOptions,
                request,
                async () =>
                {
                    var direction = DataDirection.Descending;
                    var limit = request.Limit ?? 1000;
                    var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

                    // Get data
                    var result = await Account.GetMovementsAsync(
                        request.Asset,
                        startTime: pageParams.StartTime,
                        endTime: pageParams.EndTime,
                        limit: limit,
                        ct: ct).ConfigureAwait(false);
                    if (!result.Success)
                        return HttpResult.Fail<SharedDeposit[]>(result);

                    var nextPageRequest = Pagination.GetNextPageRequest(
                            () => Pagination.NextPageFromTime(pageParams, result.Data.Min(x => x.StartTime)),
                            result.Data.Length,
                            result.Data.Select(x => x.StartTime),
                            request.StartTime,
                            request.EndTime ?? DateTime.UtcNow,
                            pageParams);

                    var data = result.Data.Where(x => x.Quantity > 0);

                    // Return
                    return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(data, x => x.StartTime, request.StartTime, request.EndTime, direction)
                        .Select(x =>
                            new SharedDeposit(
                                BitfinexExchange.AssetAliases.ExchangeToCommonName(x.Asset),
                                x.Quantity,
                                x.Status == "COMPLETED",
                                x.StartTime,
                                x.Status == "COMPLETED" ? SharedTransferStatus.Completed :
                                x.Status == "CANCELED" ? SharedTransferStatus.Failed :
                                SharedTransferStatus.Unknown)
                            {
                                Id = x.Id,
                                TransactionId = x.TransactionId
                            })
                        .ToArray(), nextPageRequest);

                });
        }

        #endregion

        #region Order Book client
        GetOrderBookOptions IOrderBookRestClient.GetOrderBookOptions { get; } = new GetOrderBookOptions(_exchangeName, new[] { 1, 25, 100 }, false);
        Task<HttpResult<SharedOrderBook>> IOrderBookRestClient.GetOrderBookAsync(GetOrderBookRequest request, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<IOrderBookRestClient, GetOrderBookRequest, SharedOrderBook>(
                this,
                client => client.GetOrderBookOptions,
                request,
                async () =>
                {
                    var result = await ExchangeData.GetOrderBookAsync(
                                    request.Symbol!.GetSymbol(FormatSymbol),
                                    Precision.PrecisionLevel0,
                                    limit: request.Limit,
                                    ct: ct).ConfigureAwait(false);
                    if (!result.Success)
                        return HttpResult.Fail<SharedOrderBook>(result);

                    return HttpResult.Ok(result, new SharedOrderBook(result.Data.Asks, result.Data.Bids));

                });
        }
        #endregion

        #region Trade History client

        GetTradeHistoryOptions ITradeHistoryRestClient.GetTradeHistoryOptions { get; } = new GetTradeHistoryOptions(_exchangeName, true, true, true, 10000, false);
        Task<HttpResult<SharedTrade[]>> ITradeHistoryRestClient.GetTradeHistoryAsync(GetTradeHistoryRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<ITradeHistoryRestClient, GetTradeHistoryRequest, SharedTrade[]>(
                this,
                client => client.GetTradeHistoryOptions,
                request,
                async () =>
                {
                    var direction = request.Direction ?? DataDirection.Ascending;
                    var symbol = request.Symbol!.GetSymbol(FormatSymbol);
                    var limit = request.Limit ?? 10000;
                    var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

                    var result = await ExchangeData.GetTradeHistoryAsync(
                        symbol,
                        startTime: pageParams.StartTime,
                        endTime: pageParams.EndTime,
                        limit: limit,
                        sorting: direction == DataDirection.Ascending ? Sorting.OldFirst : Sorting.NewFirst,
                        ct: ct).ConfigureAwait(false);
                    if (!result.Success)
                        return HttpResult.Fail<SharedTrade[]>(result);

                    var nextPageRequest = Pagination.GetNextPageRequest(
                            () => direction == DataDirection.Ascending
                                ? Pagination.NextPageFromTime(pageParams, result.Data.Max(x => x.Timestamp))
                                : Pagination.NextPageFromTime(pageParams, result.Data.Min(x => x.Timestamp)),
                            result.Data.Length,
                            result.Data.Select(x => x.Timestamp),
                            request.StartTime,
                            request.EndTime ?? DateTime.UtcNow,
                            pageParams,
                            TimeSpan.FromDays(90));

                    // Return
                    return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data, x => x.Timestamp, request.StartTime, request.EndTime, direction)
                            .Select(x =>
                                new SharedTrade(request.Symbol, symbol, Math.Abs(x.Quantity), x.Price, x.Timestamp)
                                {
                                    Side = x.Quantity > 0 ? SharedOrderSide.Buy : SharedOrderSide.Sell
                                }).ToArray(), nextPageRequest);

                });
        }
        #endregion

        #region Withdrawal client

        GetWithdrawalsOptions IWithdrawalRestClient.GetWithdrawalsOptions { get; } = new GetWithdrawalsOptions(_exchangeName, false, true, true, 1000);
        Task<HttpResult<SharedWithdrawal[]>> IWithdrawalRestClient.GetWithdrawalsAsync(GetWithdrawalsRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<IWithdrawalRestClient, GetWithdrawalsRequest, SharedWithdrawal[]>(
                this,
                client => client.GetWithdrawalsOptions,
                request,
                async () =>
                {
                    var direction = DataDirection.Descending;
                    var limit = request.Limit ?? 1000;
                    var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

                    // Get data
                    var result = await Account.GetMovementsAsync(
                        request.Asset,
                        startTime: pageParams.StartTime,
                        endTime: pageParams.EndTime,
                        limit: limit,
                        ct: ct).ConfigureAwait(false);
                    if (!result.Success)
                        return HttpResult.Fail<SharedWithdrawal[]>(result);

                    var nextPageRequest = Pagination.GetNextPageRequest(
                            () => direction == DataDirection.Ascending
                                ? Pagination.NextPageFromTime(pageParams, result.Data.Max(x => x.StartTime))
                                : Pagination.NextPageFromTime(pageParams, result.Data.Min(x => x.StartTime)),
                            result.Data.Length,
                            result.Data.Select(x => x.StartTime),
                            request.StartTime,
                            request.EndTime ?? DateTime.UtcNow,
                            pageParams);

                    var data = result.Data.Where(x => x.Quantity < 0);

                    // Return
                    return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(data, x => x.StartTime, request.StartTime, request.EndTime, direction)
                        .Select(x =>
                            new SharedWithdrawal(BitfinexExchange.AssetAliases.ExchangeToCommonName(x.Asset), x.Address, Math.Abs(x.Quantity), x.Status == "COMPLETED", x.StartTime)
                            {
                                Id = x.Id,
                                TransactionId = x.TransactionId
                            })
                        .ToArray(), nextPageRequest);

                });
        }

        #endregion

        #region Withdraw client

        WithdrawOptions IWithdrawRestClient.WithdrawOptions { get; } = new WithdrawOptions(_exchangeName)
        {
            RequiredOptionalParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(WithdrawRequest.Network), typeof(string), "Network to use", "tetheruse")
            }
        };

        Task<HttpResult<SharedId>> IWithdrawRestClient.WithdrawAsync(WithdrawRequest request, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<IWithdrawRestClient, WithdrawRequest, SharedId>(
                this,
                client => client.WithdrawOptions,
                request,
                async () =>
                {
                    // Get data
                    var withdrawal = await Account.WithdrawV2Async(
                        request.Network!,
                        WithdrawWallet.Deposit,
                        request.Quantity,
                        address: request.Address,
                        paymentId: request.AddressTag,
                        ct: ct).ConfigureAwait(false);
                    if (!withdrawal.Success)
                        return HttpResult.Fail<SharedId>(withdrawal);

                    return HttpResult.Ok(withdrawal, new SharedId(withdrawal.Data.Data.WithdrawalId.ToString()));

                });
        }

        #endregion

        #region Fee Client
        EndpointOptions<GetFeeRequest, IFeeRestClient> IFeeRestClient.GetFeeOptions { get; } = new EndpointOptions<GetFeeRequest, IFeeRestClient>(_exchangeName, true);

        Task<HttpResult<SharedFee>> IFeeRestClient.GetFeesAsync(GetFeeRequest request, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<IFeeRestClient, GetFeeRequest, SharedFee>(
                this,
                client => client.GetFeeOptions,
                request,
                async () =>
                {
                    // Get data
                    var result = await Account.Get30DaySummaryAndFeesAsync(ct: ct).ConfigureAwait(false);
                    if (!result.Success)
                        return HttpResult.Fail<SharedFee>(result);

                    // Return
                    return HttpResult.Ok(result, new SharedFee(result.Data.Fees.MakerFees.MakerFee * 100, result.Data.Fees.TakerFees.TakerFeeCrypto * 100));

                });
        }
        #endregion

        #region Trigger Order Client
        PlaceSpotTriggerOrderOptions ISpotTriggerOrderRestClient.PlaceSpotTriggerOrderOptions { get; } = new PlaceSpotTriggerOrderOptions(_exchangeName, true);
        Task<HttpResult<SharedId>> ISpotTriggerOrderRestClient.PlaceSpotTriggerOrderAsync(PlaceSpotTriggerOrderRequest request, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<ISpotOrderRestClient, PlaceSpotTriggerOrderRequest, SharedId>(
                this,
                client => ((ISpotTriggerOrderRestClient)this).PlaceSpotTriggerOrderOptions,
                request,
                async () =>
                {
                    int clientOrderId = 0;
                    if (request.ClientOrderId != null && !int.TryParse(request.ClientOrderId, out clientOrderId))
                        return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(PlaceSpotTriggerOrderRequest.ClientOrderId), "ClientOrderId needs to be parsable to `int` for `Bitfinex`"));

                    var result = await Trading.PlaceOrderAsync(
                        request.Symbol!.GetSymbol(FormatSymbol),
                        request.OrderSide == SharedOrderSide.Buy ? OrderSide.Buy : OrderSide.Sell,
                        request.OrderPrice == null ? OrderType.ExchangeStop : OrderType.ExchangeStopLimit,
                        quantity: request.Quantity?.QuantityInBaseAsset ?? 0,
                        price: request.TriggerPrice,
                        priceAuxLimit: request.OrderPrice,
                        clientOrderId: request.ClientOrderId != null ? clientOrderId : null,
                        ct: ct).ConfigureAwait(false);
                    if (!result.Success)
                        return HttpResult.Fail<SharedId>(result);

                    // Return
                    return HttpResult.Ok(result, new SharedId(result.Data.Id.ToString()));

                });
        }

        EndpointOptions<GetOrderRequest, ISpotTriggerOrderRestClient> ISpotTriggerOrderRestClient.GetSpotTriggerOrderOptions { get; } = new EndpointOptions<GetOrderRequest, ISpotTriggerOrderRestClient>(_exchangeName, true);
        Task<HttpResult<SharedSpotTriggerOrder>> ISpotTriggerOrderRestClient.GetSpotTriggerOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<ISpotTriggerOrderRestClient, GetOrderRequest, SharedSpotTriggerOrder>(
                this,
                client => client.GetSpotTriggerOrderOptions,
                request,
                async () =>
                {
                    if (!long.TryParse(request.OrderId, out var id))
                        throw new ArgumentException($"Invalid order id");

                    var symbol = request.Symbol!.GetSymbol(FormatSymbol);
                    var result = await Trading.GetOpenOrdersAsync(symbol, new[] { id }, ct: ct).ConfigureAwait(false);
                    if (!result.Success)
                        return HttpResult.Fail<SharedSpotTriggerOrder>(result);

                    if (!result.Data.Any())
                        result = await Trading.GetClosedOrdersAsync(symbol, new[] { id }, ct: ct).ConfigureAwait(false);

                    if (!result.Data.Any())
                        return HttpResult.Fail<SharedSpotTriggerOrder>(result, new ServerError(new ErrorInfo(ErrorType.UnknownOrder, $"Order with id {id} not found")));

                    var order = result.Data.Single();
                    return HttpResult.Ok(result, new SharedSpotTriggerOrder(
                        ExchangeSymbolCache.ParseSymbol(_topicId, order.Symbol),
                        order.Symbol,
                        order.Id.ToString(),
                        order.Type == OrderType.ExchangeStop ? SharedOrderType.Market : SharedOrderType.Limit,
                        order.Side == OrderSide.Buy ? SharedTriggerOrderDirection.Enter : SharedTriggerOrderDirection.Exit,
                        ParseTriggerOrderStatus(order),
                        order.Price,
                        order.CreateTime)
                    {
                        PlacedOrderId = order.Id.ToString(),
                        AveragePrice = order.PriceAverage == 0 ? null : order.PriceAverage,
                        OrderPrice = order.PriceAuxilliaryLimit,
                        OrderQuantity = new SharedOrderQuantity(order.Quantity),
                        QuantityFilled = new SharedOrderQuantity(order.Quantity - order.QuantityRemaining),
                        TimeInForce = ParseTimeInForce(order.Type, order.Flags),
                        UpdateTime = order.UpdateTime,
                        ClientOrderId = order.ClientOrderId?.ToString()
                    });

                });
        }

        private SharedTriggerOrderStatus ParseTriggerOrderStatus(BitfinexOrder order)
        {
            if (order.Status == OrderStatus.Executed || order.Status == OrderStatus.ForcefullyExecuted)
                return SharedTriggerOrderStatus.Filled;

            if (order.Status == OrderStatus.Canceled)
                return SharedTriggerOrderStatus.CanceledOrRejected;

            if (order.Status == OrderStatus.Active || order.Status == OrderStatus.PartiallyFilled)
                return SharedTriggerOrderStatus.Active;

            return SharedTriggerOrderStatus.Unknown;
        }

        EndpointOptions<CancelOrderRequest, ISpotTriggerOrderRestClient> ISpotTriggerOrderRestClient.CancelSpotTriggerOrderOptions { get; } = new EndpointOptions<CancelOrderRequest, ISpotTriggerOrderRestClient>(_exchangeName, true);
        Task<HttpResult<SharedId>> ISpotTriggerOrderRestClient.CancelSpotTriggerOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<ISpotTriggerOrderRestClient, CancelOrderRequest, SharedId>(
                this,
                client => client.CancelSpotTriggerOrderOptions,
                request,
                async () =>
                {
                    if (!long.TryParse(request.OrderId, out var orderId))
                        return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

                    var order = await Trading.CancelOrderAsync(orderId, ct: ct).ConfigureAwait(false);
                    if (!order.Success)
                        return HttpResult.Fail<SharedId>(order);

                    return HttpResult.Ok(order, new SharedId(order.Data.Id.ToString()));

                });
        }
        #endregion

        #region Transfer client

        TransferOptions ITransferRestClient.TransferOptions { get; } = new TransferOptions(_exchangeName, [
            SharedAccountType.Funding,
            SharedAccountType.Spot,
            SharedAccountType.CrossMargin,
            SharedAccountType.IsolatedMargin
            ]);
        Task<HttpResult<SharedId>> ITransferRestClient.TransferAsync(TransferRequest request, CancellationToken ct)
        {
            return SharedUtils.ExecuteSharedAsync<ITransferRestClient, TransferRequest, SharedId>(
                this,
                client => client.TransferOptions,
                request,
                async () =>
                {
                    var fromAccount = GetTransferType(request.FromAccountType);
                    var toAccount = GetTransferType(request.ToAccountType);
                    if (fromAccount == null || toAccount == null)
                        return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid("To/From AccountType", "invalid to/from account combination"));

                    // Get data
                    var transfer = await Account.WalletTransferAsync(
                        request.Asset,
                        request.Quantity,
                        fromAccount.Value,
                        toAccount.Value,
                        ct: ct).ConfigureAwait(false);
                    if (!transfer.Success)
                        return HttpResult.Fail<SharedId>(transfer);

                    return HttpResult.Ok(transfer, new SharedId(""));

                });
        }

        private WithdrawWallet? GetTransferType(SharedAccountType type)
        {
            if (type == SharedAccountType.Funding) return WithdrawWallet.Deposit;
            if (type == SharedAccountType.Spot) return WithdrawWallet.Exchange;
            if (type.IsMarginAccount()) return WithdrawWallet.Trading;
            return null;
        }

        #endregion

    }
}