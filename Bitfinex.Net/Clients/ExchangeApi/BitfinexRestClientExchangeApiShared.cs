using Bitfinex.Net.Enums;
using Bitfinex.Net.Interfaces.Clients.ExchangeApi;
using Bitfinex.Net.Objects.Models;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients.ExchangeApi
{
    internal partial class BitfinexRestClientExchangeApi : IBitfinexRestClientExchangeApiShared
    {
        private const string _exchangeName = "Bitfinex";
        private const string _topicSpotId = "BitfinexSpot";
        private const string _topicFuturesId = "BitfinexFutures";

        public TradingMode[] SupportedTradingModes { get; } = new[] { TradingMode.Spot, TradingMode.PerpetualLinear };
        public void SetDefaultExchangeParameter(string key, object value) => ExchangeParameters.SetStaticParameter(Exchange, key, value);
        public void ResetDefaultExchangeParameters() => ExchangeParameters.ResetStaticParameters();
        public SharedClientInfo Discover() => SharedUtils.GetClientInfo(BitfinexExchange.Metadata, this);

        private static HashSet<string> _exchangeSupportedFiat = ["USD", "EUR", "GBP"];

        #region Kline client

        GetKlinesOptions IKlineRestClient.GetKlinesOptions { get; } = new GetKlinesOptions(_exchangeName, true, true, true, 10000, false);
        async Task<HttpResult<SharedKline[]>> IKlineRestClient.GetKlinesAsync(GetKlinesRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = SharedClient.GetKlinesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedKline[]>(Exchange, validationError);

            var direction = request.Direction ?? DataDirection.Ascending;
            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var limit = request.Limit ?? 10000;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

            // Get data
            var result = await ExchangeData.GetKlinesAsync(
                symbol,
                (KlineInterval)request.Interval,
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
        }

        #endregion

        #region Asset client
        GetAssetOptions IAssetsRestClient.GetAssetOptions { get; } = new GetAssetOptions(_exchangeName, false);
        async Task<HttpResult<SharedAsset>> IAssetsRestClient.GetAssetAsync(GetAssetRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetAssetOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedAsset>(Exchange, validationError);

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
        }

        GetAssetsOptions IAssetsRestClient.GetAssetsOptions { get; } = new GetAssetsOptions(_exchangeName, false);

        async Task<HttpResult<SharedAsset[]>> IAssetsRestClient.GetAssetsAsync(GetAssetsRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetAssetsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedAsset[]>(Exchange, validationError);

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

            return HttpResult.Ok<SharedAsset[]>(assetList.Result, assetList.Result.Data.Select(x =>
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
        }

        #endregion

        #region Spot Symbol client

        SharedSymbolCatalog? ISpotSymbolRestClient.SpotSymbolCatalog => ExchangeSymbolCache.GetSymbolCatalog(_topicSpotId, EnvironmentName, null);
        GetSpotSymbolsOptions ISpotSymbolRestClient.GetSpotSymbolsOptions { get; } = new GetSpotSymbolsOptions(_exchangeName, false);
        async Task<HttpResult<SharedSpotSymbol[]>> ISpotSymbolRestClient.GetSpotSymbolsAsync(GetSymbolsRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotSymbolsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotSymbol[]>(Exchange, validationError);

            var result = await ExchangeData.GetSymbolsAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotSymbol[]>(result);

            var data = result.Data
                .Select(x => ParseSymbol(x))
                .ToArray();

            ExchangeSymbolCache.UpdateSymbolInfo(_topicSpotId, EnvironmentName, null, data);
            return HttpResult.Ok(result, SharedUtils.ApplySymbolFilter(data, request));
        }

        private SharedSpotSymbol ParseSymbol(KeyValuePair<string, BitfinexSymbolInfo> s)
        {
            var assets = GetAssets(s.Key);
            var result = new SharedSpotSymbol(assets.BaseAsset, assets.QuoteAsset, "t" + s.Key, true)
            {
                // These apply to all symbols
                PriceSignificantFigures = 5,
                PriceDecimals = 8,
                QuantityDecimals = 8,

                MinTradeQuantity = s.Value.MinOrderQuantity,
                MaxTradeQuantity = s.Value.MaxOrderQuantity,
                DisplayName = s.Key
            };

            if (_exchangeSupportedFiat.Contains(result.BaseAsset))
            {
                result.BaseAssetType = SharedAssetType.Fiat;
            }
            else if (LibraryHelpers.IsCommodity(result.BaseAsset))
            {
                result.BaseAssetType = SharedAssetType.TradFi;
                result.BaseAssetSubType = SharedAssetSubType.Commodity;
            }
            else
            {
                result.BaseAssetType = SharedAssetType.Crypto;
            }

            if (_exchangeSupportedFiat.Contains(result.QuoteAsset))
            {
                result.QuoteAssetType = SharedAssetType.Fiat;
            }
            else if (LibraryHelpers.IsCommodity(result.QuoteAsset))
            {
                result.QuoteAssetType = SharedAssetType.TradFi;
                result.QuoteAssetSubType = SharedAssetSubType.Commodity;
            }
            else
            {
                result.QuoteAssetType = SharedAssetType.Crypto;
                if (LibraryHelpers.IsStableCoin(result.QuoteAsset, ["UST"]))
                    result.QuoteAssetSubType = SharedAssetSubType.StableCoin;
            }

            return result;
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
            if (!ExchangeSymbolCache.HasCached(_topicSpotId, EnvironmentName, null))
            {
                var symbols = await ((ISpotSymbolRestClient)this).GetSpotSymbolsAsync(new GetSymbolsRequest()).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<SharedSymbol[]>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<SharedSymbol[]>.Ok(Exchange, ExchangeSymbolCache.GetSymbolsForBaseAsset(_topicSpotId, EnvironmentName, null, baseAsset));
        }

        async Task<ExchangeCallResult<bool>> ISpotSymbolRestClient.SupportsSpotSymbolAsync(SharedSymbol symbol)
        {
            if (symbol.TradingMode != TradingMode.Spot)
                throw new ArgumentException(nameof(symbol), "Only Spot symbols allowed");

            if (!ExchangeSymbolCache.HasCached(_topicSpotId, EnvironmentName, null))
            {
                var symbols = await ((ISpotSymbolRestClient)this).GetSpotSymbolsAsync(new GetSymbolsRequest()).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicSpotId, EnvironmentName, null, symbol));
        }

        async Task<ExchangeCallResult<bool>> ISpotSymbolRestClient.SupportsSpotSymbolAsync(string symbolName)
        {
            if (!ExchangeSymbolCache.HasCached(_topicSpotId, EnvironmentName, null))
            {
                var symbols = await ((ISpotSymbolRestClient)this).GetSpotSymbolsAsync(new GetSymbolsRequest()).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicSpotId, EnvironmentName, null, symbolName));
        }
        #endregion

        #region Futures Symbol client

        SharedSymbolCatalog? IFuturesSymbolRestClient.FuturesSymbolCatalog => ExchangeSymbolCache.GetSymbolCatalog(_topicFuturesId, EnvironmentName, null);
        GetFuturesSymbolsOptions IFuturesSymbolRestClient.GetFuturesSymbolsOptions { get; } = new GetFuturesSymbolsOptions(_exchangeName, false);
        async Task<HttpResult<SharedFuturesSymbol[]>> IFuturesSymbolRestClient.GetFuturesSymbolsAsync(GetSymbolsRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetFuturesSymbolsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesSymbol[]>(Exchange, validationError);

            var result = await ExchangeData.GetFuturesSymbolsAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFuturesSymbol[]>(result);

            var data = result.Data
                .Select(x => ParseFuturesSymbol(x))
                .ToArray();

            ExchangeSymbolCache.UpdateSymbolInfo(_topicSpotId, EnvironmentName, null, data);
            return HttpResult.Ok(result, SharedUtils.ApplySymbolFilter(data, request));
        }

        private SharedFuturesSymbol ParseFuturesSymbol(KeyValuePair<string, BitfinexSymbolInfo> s)
        {
            var assets = GetFuturesAssets(s.Key);
            var result = new SharedFuturesSymbol(TradingMode.PerpetualLinear, assets.BaseAsset, assets.QuoteAsset, "t" + s.Key, true)
            {
                // These apply to all symbols
                PriceSignificantFigures = 5,
                PriceDecimals = 8,
                QuantityDecimals = 8,

                MinTradeQuantity = s.Value.MinOrderQuantity,
                MaxTradeQuantity = s.Value.MaxOrderQuantity,
                DisplayName = s.Key
            };

            if (_exchangeSupportedFiat.Contains(result.BaseAsset))
            {
                result.BaseAssetType = SharedAssetType.Fiat;
            }
            else if (LibraryHelpers.IsCommodity(result.BaseAsset))
            {
                result.BaseAssetType = SharedAssetType.TradFi;
                result.BaseAssetSubType = SharedAssetSubType.Commodity;
            }
            else if (result.BaseAsset.EndsWith("IX"))
            {
                result.BaseAssetType = SharedAssetType.TradFi;
                result.BaseAssetSubType = SharedAssetSubType.Equity;
            }
            else
            {
                result.BaseAssetType = SharedAssetType.Crypto;
            }

            if (_exchangeSupportedFiat.Contains(result.QuoteAsset))
            {
                result.QuoteAssetType = SharedAssetType.Fiat;
            }
            else
            {
                result.QuoteAssetType = SharedAssetType.Crypto;
                result.QuoteAssetSubType = SharedAssetSubType.StableCoin;
            }

            return result;
        }

        private (string BaseAsset, string QuoteAsset) GetFuturesAssets(string input)
        {
            var split = input.Split(':');
            var baseAsset = split[0].Replace("F0", "");
            var quoteAsset = split[1].Replace("F0", "");

            return (BitfinexExchange.AssetAliases.ExchangeToCommonName(baseAsset), BitfinexExchange.AssetAliases.ExchangeToCommonName(quoteAsset));
        }

        async Task<ExchangeCallResult<SharedSymbol[]>> IFuturesSymbolRestClient.GetFuturesSymbolsForBaseAssetAsync(string baseAsset)
        {
            if (!ExchangeSymbolCache.HasCached(_topicFuturesId, EnvironmentName, null))
            {
                var symbols = await ((IFuturesSymbolRestClient)this).GetFuturesSymbolsAsync(new GetSymbolsRequest()).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<SharedSymbol[]>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<SharedSymbol[]>.Ok(Exchange, ExchangeSymbolCache.GetSymbolsForBaseAsset(_topicFuturesId, EnvironmentName, null, baseAsset));
        }

        async Task<ExchangeCallResult<bool>> IFuturesSymbolRestClient.SupportsFuturesSymbolAsync(SharedSymbol symbol)
        {
            if (symbol.TradingMode == TradingMode.Spot)
                throw new ArgumentException(nameof(symbol), "Spot symbols not allowed");

            if (!ExchangeSymbolCache.HasCached(_topicFuturesId, EnvironmentName, null))
            {
                var symbols = await ((IFuturesSymbolRestClient)this).GetFuturesSymbolsAsync(new GetSymbolsRequest()).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicFuturesId, EnvironmentName, null, symbol));
        }

        async Task<ExchangeCallResult<bool>> IFuturesSymbolRestClient.SupportsFuturesSymbolAsync(string symbolName)
        {
            if (!ExchangeSymbolCache.HasCached(_topicFuturesId, EnvironmentName, null))
            {
                var symbols = await ((IFuturesSymbolRestClient)this).GetFuturesSymbolsAsync(new GetSymbolsRequest()).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicFuturesId, EnvironmentName, null, symbolName));
        }
        #endregion

        #region Ticker client

        GetSpotTickerOptions ISpotTickerRestClient.GetSpotTickerOptions { get; } = new GetSpotTickerOptions(_exchangeName);
        async Task<HttpResult<SharedSpotTicker>> ISpotTickerRestClient.GetSpotTickerAsync(GetTickerRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotTicker>(Exchange, validationError);

            var result = await ExchangeData.GetTickerAsync(request.Symbol!.GetSymbol(FormatSymbol), ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotTicker>(result);

            return HttpResult.Ok(result,
                new SharedSpotTicker(
                    ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, result.Data.Symbol),
                    result.Data.Symbol,
                    result.Data.LastPrice, 
                    result.Data.HighPrice,
                    result.Data.LowPrice, 
                    result.Data.Volume, 
                    Math.Round(result.Data.DailyChangePercentage * 100, 2)));
        }

        GetSpotTickersOptions ISpotTickerRestClient.GetSpotTickersOptions { get; } = new GetSpotTickersOptions(_exchangeName);
        async Task<HttpResult<SharedSpotTicker[]>> ISpotTickerRestClient.GetSpotTickersAsync(GetTickersRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotTickersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotTicker[]>(Exchange, validationError);

            var result = await ExchangeData.GetTickersAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotTicker[]>(result);

            return HttpResult.Ok(result, result.Data.Where(x => !x.Symbol.Contains("F0") && !x.Symbol.StartsWith("f")).Select(x => 
                new SharedSpotTicker(
                    ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, x.Symbol),
                    x.Symbol, 
                    x.LastPrice,
                    x.HighPrice, 
                    x.LowPrice, 
                    x.Volume, 
                    Math.Round(x.DailyChangePercentage * 100, 2))).ToArray());
        }

        #endregion

        #region Book Ticker client

        GetBookTickerOptions IBookTickerRestClient.GetBookTickerOptions { get; } = new GetBookTickerOptions(_exchangeName, false);
        async Task<HttpResult<SharedBookTicker>> IBookTickerRestClient.GetBookTickerAsync(GetBookTickerRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetBookTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedBookTicker>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var resultTicker = await ExchangeData.GetOrderBookAsync(symbol, Precision.PrecisionLevel0, 1, ct: ct).ConfigureAwait(false);
            if (!resultTicker.Success)
                return HttpResult.Fail<SharedBookTicker>(resultTicker);

            return HttpResult.Ok(resultTicker, new SharedBookTicker(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, symbol),
                symbol,
                resultTicker.Data.Asks[0].Price,
                resultTicker.Data.Asks[0].Quantity,
                resultTicker.Data.Bids[0].Price,
                resultTicker.Data.Bids[0].Quantity));
        }

        #endregion

        #region Recent Trade client

        GetRecentTradesOptions IRecentTradeRestClient.GetRecentTradesOptions { get; } = new GetRecentTradesOptions(_exchangeName, 10000, false);
        async Task<HttpResult<SharedTrade[]>> IRecentTradeRestClient.GetRecentTradesAsync(GetRecentTradesRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetRecentTradesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedTrade[]>(Exchange, validationError);

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
        }

        #endregion

        #region Balance client
        GetBalancesOptions IBalanceRestClient.GetBalancesOptions { get; } = new GetBalancesOptions(_exchangeName, AccountTypeFilter.Funding, AccountTypeFilter.Spot, AccountTypeFilter.Margin);

        async Task<HttpResult<SharedBalance[]>> IBalanceRestClient.GetBalancesAsync(GetBalancesRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetBalancesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedBalance[]>(Exchange, validationError);

            var result = await Account.GetBalancesAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedBalance[]>(result);

            var filterType = request.AccountType?.IsMarginAccount() == true ? WalletType.Margin : request.AccountType == SharedAccountType.Funding ? WalletType.Funding : WalletType.Exchange;
            return HttpResult.Ok(result, result.Data.Where(x => x.Type == filterType).Select(x =>
                new SharedBalance(
                        SupportedTradingModes, 
                        BitfinexExchange.AssetAliases.ExchangeToCommonName(x.Asset),
                        x.Available ?? 0, 
                        x.Total)).ToArray());
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
        async Task<HttpResult<SharedId>> ISpotOrderRestClient.PlaceSpotOrderAsync(PlaceSpotOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.PlaceSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

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
        }

        GetSpotOrderOptions ISpotOrderRestClient.GetSpotOrderOptions { get; } = new GetSpotOrderOptions(_exchangeName, true);
        async Task<HttpResult<SharedSpotOrder>> ISpotOrderRestClient.GetSpotOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedSpotOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderRequest.OrderId), "Invalid order id"));

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await Trading.GetOpenOrdersAsync(symbol, new[] { orderId }, ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotOrder>(result);

            if (!result.Data.Any())
                result = await Trading.GetClosedOrdersAsync(symbol, new[] { orderId }, ct: ct).ConfigureAwait(false);
            
            if (!result.Success)
                return HttpResult.Fail<SharedSpotOrder>(result);

            if (!result.Data.Any())
                return HttpResult.Fail<SharedSpotOrder>(result, new ServerError(new ErrorInfo(ErrorType.UnknownOrder, $"Order with id {orderId} not found")));

            var order = result.Data.Single();
            return HttpResult.Ok(result, new SharedSpotOrder(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, order.Symbol),
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
        }

        GetOpenSpotOrdersOptions ISpotOrderRestClient.GetOpenSpotOrdersOptions { get; } = new GetOpenSpotOrdersOptions(_exchangeName, true);
        async Task<HttpResult<SharedSpotOrder[]>> ISpotOrderRestClient.GetOpenSpotOrdersAsync(GetOpenOrdersRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetOpenSpotOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder[]>(Exchange, validationError);

            var symbol = request.Symbol?.GetSymbol(FormatSymbol);
            var result = await Trading.GetOpenOrdersAsync(symbol, ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotOrder[]>(result);

            return HttpResult.Ok(result, result.Data.Select(x => new SharedSpotOrder(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, x.Symbol),
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
        }

        GetSpotClosedOrdersOptions ISpotOrderRestClient.GetClosedSpotOrdersOptions { get; } = new GetSpotClosedOrdersOptions(_exchangeName, false, true, true, 2500);
        async Task<HttpResult<SharedSpotOrder[]>> ISpotOrderRestClient.GetClosedSpotOrdersAsync(GetClosedOrdersRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = SharedClient.GetClosedSpotOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder[]>(Exchange, validationError);

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
                        ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, x.Symbol),
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
        }

        GetSpotOrderTradesOptions ISpotOrderRestClient.GetSpotOrderTradesOptions { get; } = new GetSpotOrderTradesOptions(_exchangeName, true);
        async Task<HttpResult<SharedUserTrade[]>> ISpotOrderRestClient.GetSpotOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotOrderTradesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, ArgumentError.Invalid(nameof(GetOrderTradesRequest.OrderId), "Invalid order id"));

            var order = await Trading.GetOrderTradesAsync(
                request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedUserTrade[]>(order);

            return HttpResult.Ok(order, order.Data.Select(x => new SharedUserTrade(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, x.Symbol),
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
        }

        GetSpotUserTradesOptions ISpotOrderRestClient.GetSpotUserTradesOptions { get; } = new GetSpotUserTradesOptions(_exchangeName, false, true, true, 2500);
        async Task<HttpResult<SharedUserTrade[]>> ISpotOrderRestClient.GetSpotUserTradesAsync(GetUserTradesRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotUserTradesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, validationError);

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
                    ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, x.Symbol),
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
        }

        CancelSpotOrderOptions ISpotOrderRestClient.CancelSpotOrderOptions { get; } = new CancelSpotOrderOptions(_exchangeName, true);
        async Task<HttpResult<SharedId>> ISpotOrderRestClient.CancelSpotOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.CancelSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await Trading.CancelOrderAsync(orderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(order.Data.ToString()));
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

        GetDepositAddressesOptions IDepositRestClient.GetDepositAddressesOptions { get; } = new GetDepositAddressesOptions(_exchangeName, true)
        {
            RequiredOptionalParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(GetDepositAddressesRequest.Network), typeof(string), "The network the deposit address should be for", "bitcoin")
            }
        };

        async Task<HttpResult<SharedDepositAddress[]>> IDepositRestClient.GetDepositAddressesAsync(GetDepositAddressesRequest request, CancellationToken ct)
        {

            var validationError = SharedClient.GetDepositAddressesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedDepositAddress[]>(Exchange, validationError);

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
        }

        GetDepositsOptions IDepositRestClient.GetDepositsOptions { get; } = new GetDepositsOptions(_exchangeName, false, true, true, 1000);
        async Task<HttpResult<SharedDeposit[]>> IDepositRestClient.GetDepositsAsync(GetDepositsRequest request, PageRequest? pageRequest, CancellationToken ct)
        {

            var validationError = SharedClient.GetDepositsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedDeposit[]>(Exchange, validationError);

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
        }

        #endregion

        #region Order Book client
        GetOrderBookOptions IOrderBookRestClient.GetOrderBookOptions { get; } = new GetOrderBookOptions(_exchangeName, new[] { 1, 25, 100 }, false);
        async Task<HttpResult<SharedOrderBook>> IOrderBookRestClient.GetOrderBookAsync(GetOrderBookRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetOrderBookOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedOrderBook>(Exchange, validationError);

            var result = await ExchangeData.GetOrderBookAsync(
                            request.Symbol!.GetSymbol(FormatSymbol),
                            Precision.PrecisionLevel0,
                            limit: request.Limit,
                            ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedOrderBook>(result);

            return HttpResult.Ok(result, new SharedOrderBook(result.Data.Asks, result.Data.Bids));
        }
        #endregion

        #region Trade History client

        GetTradeHistoryOptions ITradeHistoryRestClient.GetTradeHistoryOptions { get; } = new GetTradeHistoryOptions(_exchangeName, true, true, true, 10000, false);
        async Task<HttpResult<SharedTrade[]>> ITradeHistoryRestClient.GetTradeHistoryAsync(GetTradeHistoryRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = SharedClient.GetTradeHistoryOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedTrade[]>(Exchange, validationError);

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
        }
        #endregion

        #region Withdrawal client

        GetWithdrawalsOptions IWithdrawalRestClient.GetWithdrawalsOptions { get; } = new GetWithdrawalsOptions(_exchangeName, false, true, true, 1000);
        async Task<HttpResult<SharedWithdrawal[]>> IWithdrawalRestClient.GetWithdrawalsAsync(GetWithdrawalsRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = SharedClient.GetWithdrawalsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedWithdrawal[]>(Exchange, validationError);

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
                    new SharedWithdrawal(
                        BitfinexExchange.AssetAliases.ExchangeToCommonName(x.Asset), 
                        x.Address, 
                        Math.Abs(x.Quantity),
                        x.Status == "COMPLETED",
                        x.StartTime,
                        GetWithdrawalStatus(x))
                    {
                        Id = x.Id,
                        TransactionId = x.TransactionId
                    })
                .ToArray(), nextPageRequest);
        }

        private SharedTransferStatus GetWithdrawalStatus(BitfinexMovement x)
        {
            if (x.Status == "COMPLETED")
                return SharedTransferStatus.Completed;

            return SharedTransferStatus.Unknown;
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

        async Task<HttpResult<SharedId>> IWithdrawRestClient.WithdrawAsync(WithdrawRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.WithdrawOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

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
        }

        #endregion

        #region Fee Client
        GetFeeOptions IFeeRestClient.GetFeeOptions { get; } = new GetFeeOptions(_exchangeName, true);

        async Task<HttpResult<SharedFee>> IFeeRestClient.GetFeesAsync(GetFeeRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetFeeOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFee>(Exchange, validationError);

            // Get data
            var result = await Account.Get30DaySummaryAndFeesAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFee>(result);

            // Return
            return HttpResult.Ok(result, new SharedFee(result.Data.Fees.MakerFees.MakerFee * 100, result.Data.Fees.TakerFees.TakerFeeCrypto * 100));
        }
        #endregion

        #region Trigger Order Client
        PlaceSpotTriggerOrderOptions ISpotTriggerOrderRestClient.PlaceSpotTriggerOrderOptions { get; } = new PlaceSpotTriggerOrderOptions(_exchangeName, true);
        async Task<HttpResult<SharedId>> ISpotTriggerOrderRestClient.PlaceSpotTriggerOrderAsync(PlaceSpotTriggerOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.PlaceSpotTriggerOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

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
        }

        GetSpotTriggerOrderOptions ISpotTriggerOrderRestClient.GetSpotTriggerOrderOptions { get; } = new GetSpotTriggerOrderOptions(_exchangeName, true);
        async Task<HttpResult<SharedSpotTriggerOrder>> ISpotTriggerOrderRestClient.GetSpotTriggerOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotTriggerOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotTriggerOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var id))
                throw new ArgumentException($"Invalid order id");

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await Trading.GetOpenOrdersAsync(symbol, new[] { id }, ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotTriggerOrder>(result);

            if (!result.Data.Any())
                result = await Trading.GetClosedOrdersAsync(symbol, new[] { id }, ct: ct).ConfigureAwait(false);

            if (!result.Success)
                return HttpResult.Fail<SharedSpotTriggerOrder>(result);

            if (!result.Data.Any())
                return HttpResult.Fail<SharedSpotTriggerOrder>(result, new ServerError(new ErrorInfo(ErrorType.UnknownOrder, $"Order with id {id} not found")));

            var order = result.Data.Single();
            return HttpResult.Ok(result, new SharedSpotTriggerOrder(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, order.Symbol),
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

        CancelSpotTriggerOrderOptions ISpotTriggerOrderRestClient.CancelSpotTriggerOrderOptions { get; } = new CancelSpotTriggerOrderOptions(_exchangeName, true);
        async Task<HttpResult<SharedId>> ISpotTriggerOrderRestClient.CancelSpotTriggerOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.CancelSpotTriggerOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await Trading.CancelOrderAsync(orderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(order.Data.Id.ToString()));
        }
        #endregion

        #region Transfer client

        TransferOptions ITransferRestClient.TransferOptions { get; } = new TransferOptions(_exchangeName, [
            SharedAccountType.Funding,
            SharedAccountType.Spot,
            SharedAccountType.CrossMargin,
            SharedAccountType.IsolatedMargin
            ]);
        async Task<HttpResult<SharedId>> ITransferRestClient.TransferAsync(TransferRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.TransferOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

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
        }

        private WithdrawWallet? GetTransferType(SharedAccountType type)
        {
            if (type == SharedAccountType.Funding) return WithdrawWallet.Deposit;
            if (type == SharedAccountType.Spot) return WithdrawWallet.Exchange;
            if (type.IsMarginAccount()) return WithdrawWallet.Trading;
            return null;
        }

        #endregion

        #region Futures Ticker client

        GetFuturesTickerOptions IFuturesTickerRestClient.GetFuturesTickerOptions { get; } = new GetFuturesTickerOptions(_exchangeName);
        async Task<HttpResult<SharedFuturesTicker>> IFuturesTickerRestClient.GetFuturesTickerAsync(GetTickerRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetFuturesTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesTicker>(Exchange, validationError);

            var result = await ExchangeData.GetTickerAsync(request.Symbol!.GetSymbol(FormatSymbol), ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFuturesTicker>(result);

            return HttpResult.Ok(result, 
                new SharedFuturesTicker(
                    ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, result.Data.Symbol),
                    result.Data.Symbol,
                    result.Data.LastPrice,
                    result.Data.HighPrice,
                    result.Data.LowPrice,
                    result.Data.Volume, 
                    Math.Round(result.Data.DailyChangePercentage * 100, 2)));
        }

        GetFuturesTickersOptions IFuturesTickerRestClient.GetFuturesTickersOptions { get; } = new GetFuturesTickersOptions(_exchangeName);
        async Task<HttpResult<SharedFuturesTicker[]>> IFuturesTickerRestClient.GetFuturesTickersAsync(GetTickersRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotTickersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesTicker[]>(Exchange, validationError);

            var result = await ExchangeData.GetTickersAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFuturesTicker[]>(result);

            return HttpResult.Ok(result, result.Data.Where(x => x.Symbol.Contains("F0")).Select(x => 
                new SharedFuturesTicker(
                    ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, x.Symbol), 
                    x.Symbol, 
                    x.LastPrice,
                    x.HighPrice,
                    x.LowPrice,
                    x.Volume,
                    Math.Round(x.DailyChangePercentage * 100, 2))).ToArray());
        }

        #endregion

        #region Futures Order Client
        SharedFeeDeductionType IFuturesOrderRestClient.FuturesFeeDeductionType => SharedFeeDeductionType.AddToCost;
        SharedFeeAssetType IFuturesOrderRestClient.FuturesFeeAssetType => SharedFeeAssetType.QuoteAsset;

        SharedOrderType[] IFuturesOrderRestClient.FuturesSupportedOrderTypes { get; } = new[] { SharedOrderType.Limit, SharedOrderType.Market, SharedOrderType.LimitMaker };
        SharedTimeInForce[] IFuturesOrderRestClient.FuturesSupportedTimeInForce { get; } = new[] { SharedTimeInForce.GoodTillCanceled, SharedTimeInForce.ImmediateOrCancel, SharedTimeInForce.FillOrKill };
        SharedQuantitySupport IFuturesOrderRestClient.FuturesSupportedOrderQuantity { get; } = new SharedQuantitySupport(
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset);

        string IFuturesOrderRestClient.GenerateClientOrderId() => ExchangeHelpers.RandomLong(10).ToString();

        PlaceFuturesOrderOptions IFuturesOrderRestClient.PlaceFuturesOrderOptions { get; } = new PlaceFuturesOrderOptions(_exchangeName, true);
        async Task<HttpResult<SharedId>> IFuturesOrderRestClient.PlaceFuturesOrderAsync(PlaceFuturesOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.PlaceFuturesOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            int clientOrderId = 0;
            if (request.ClientOrderId != null && !int.TryParse(request.ClientOrderId, out clientOrderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(PlaceSpotOrderRequest.ClientOrderId), "ClientOrderId needs to be parsable to `int` for `Bitfinex`"));

            var result = await Trading.PlaceOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                request.Side == SharedOrderSide.Buy ? Enums.OrderSide.Buy : Enums.OrderSide.Sell,
                GetFuturesPlaceOrderType(request.OrderType, request.TimeInForce),
                quantity: request.Quantity?.QuantityInBaseAsset ?? 0,
                flags: request.OrderType == SharedOrderType.LimitMaker ? Enums.OrderFlags.PostOnly : null,
                price: request.Price ?? 0,
                leverage: (int?)request.Leverage,
                clientOrderId: request.ClientOrderId != null ? clientOrderId : null,                
                ct: ct).ConfigureAwait(false);

            if (!result.Success)
                return HttpResult.Fail<SharedId>(result);

            return HttpResult.Ok(result, new SharedId(result.Data.Data!.Id.ToString()));
        }

        private Enums.OrderType GetFuturesPlaceOrderType(SharedOrderType type, SharedTimeInForce? tif)
        {
            if ((type == SharedOrderType.Limit || type == SharedOrderType.LimitMaker) && (tif == null || tif == SharedTimeInForce.GoodTillCanceled)) return Enums.OrderType.Limit;
            if (type == SharedOrderType.Limit && tif == SharedTimeInForce.ImmediateOrCancel) return Enums.OrderType.ImmediateOrCancel;
            if (type == SharedOrderType.Limit && tif == SharedTimeInForce.FillOrKill) return Enums.OrderType.FillOrKill;
            if (type == SharedOrderType.Market) return Enums.OrderType.Market;

            throw new ArgumentException($"The combination of order type `{type}` and time in force `{tif}` in invalid");
        }

        GetFuturesOrderOptions IFuturesOrderRestClient.GetFuturesOrderOptions { get; } = new GetFuturesOrderOptions(_exchangeName, true);
        async Task<HttpResult<SharedFuturesOrder>> IFuturesOrderRestClient.GetFuturesOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetFuturesOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedFuturesOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderRequest.OrderId), "Invalid order id"));

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await Trading.GetOpenOrdersAsync(symbol, new[] { orderId }, ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFuturesOrder>(result);

            if (!result.Data.Any())
                result = await Trading.GetClosedOrdersAsync(symbol, new[] { orderId }, ct: ct).ConfigureAwait(false);

            if (!result.Success)
                return HttpResult.Fail<SharedFuturesOrder>(result);

            if (!result.Data.Any())
                return HttpResult.Fail<SharedFuturesOrder>(result, new ServerError(new ErrorInfo(ErrorType.UnknownOrder, $"Order with id {orderId} not found")));

            var order = result.Data.Single();
            return HttpResult.Ok(result, new SharedFuturesOrder(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, order.Symbol),
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
        }

        GetOpenFuturesOrdersOptions IFuturesOrderRestClient.GetOpenFuturesOrdersOptions { get; } = new GetOpenFuturesOrdersOptions(_exchangeName, true);
        async Task<HttpResult<SharedFuturesOrder[]>> IFuturesOrderRestClient.GetOpenFuturesOrdersAsync(GetOpenOrdersRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetOpenFuturesOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesOrder[]>(Exchange, validationError);

            var symbol = request.Symbol?.GetSymbol(FormatSymbol);
            var result = await Trading.GetOpenOrdersAsync(symbol, ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFuturesOrder[]>(result);

            return HttpResult.Ok(result, result.Data.Select(x => new SharedFuturesOrder(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, x.Symbol),
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
        }

        GetFuturesClosedOrdersOptions IFuturesOrderRestClient.GetClosedFuturesOrdersOptions { get; } = new GetFuturesClosedOrdersOptions(_exchangeName, false, true, true, 1000);
        async Task<HttpResult<SharedFuturesOrder[]>> IFuturesOrderRestClient.GetClosedFuturesOrdersAsync(GetClosedOrdersRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = SharedClient.GetClosedFuturesOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesOrder[]>(Exchange, validationError);

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
                return HttpResult.Fail<SharedFuturesOrder[]>(result);

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
                    new SharedFuturesOrder(
                        ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, x.Symbol),
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
        }

        GetFuturesOrderTradesOptions IFuturesOrderRestClient.GetFuturesOrderTradesOptions { get; } = new GetFuturesOrderTradesOptions(_exchangeName, true);
        async Task<HttpResult<SharedUserTrade[]>> IFuturesOrderRestClient.GetFuturesOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetFuturesOrderTradesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, ArgumentError.Invalid(nameof(GetOrderTradesRequest.OrderId), "Invalid order id"));

            var order = await Trading.GetOrderTradesAsync(
                request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedUserTrade[]>(order);

            return HttpResult.Ok(order, order.Data.Select(x => new SharedUserTrade(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, x.Symbol),
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
        }

        GetFuturesUserTradesOptions IFuturesOrderRestClient.GetFuturesUserTradesOptions { get; } = new GetFuturesUserTradesOptions(_exchangeName, false, true, true, 1000);
        async Task<HttpResult<SharedUserTrade[]>> IFuturesOrderRestClient.GetFuturesUserTradesAsync(GetUserTradesRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = SharedClient.GetFuturesUserTradesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, validationError);

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
                    ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, x.Symbol),
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
        }

        CancelFuturesOrderOptions IFuturesOrderRestClient.CancelFuturesOrderOptions { get; } = new CancelFuturesOrderOptions(_exchangeName, true);
        async Task<HttpResult<SharedId>> IFuturesOrderRestClient.CancelFuturesOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.CancelFuturesOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await Trading.CancelOrderAsync(orderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(order.Data.ToString()));
        }

        GetPositionsOptions IFuturesOrderRestClient.GetPositionsOptions { get; } = new GetPositionsOptions(_exchangeName, true);
        async Task<HttpResult<SharedPosition[]>> IFuturesOrderRestClient.GetPositionsAsync(GetPositionsRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetPositionsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedPosition[]>(Exchange, validationError);

            var result = await Trading.GetPositionsAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedPosition[]>(result);

            var data = result.Data;
            if (request.Symbol != null)            
                data = result.Data.Where(x => x.Symbol == request.Symbol.GetSymbol(FormatSymbol)).ToArray();

            return HttpResult.Ok(result, data.Select(x => 
            new SharedPosition(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, x.Symbol),
                x.Symbol, 
                Math.Abs(x.Quantity),
                DateTime.UtcNow)
            {
                UnrealizedPnl = x.ProfitLoss,
                LiquidationPrice = x.LiquidationPrice == 0 ? null : x.LiquidationPrice,
                UpdateTime = x.UpdateTime,
                Leverage = x.Leverage,
                AverageOpenPrice = x.BasePrice,
                PositionMode = SharedPositionMode.OneWay,
                PositionSide = x.Quantity < 0 ? SharedPositionSide.Short : SharedPositionSide.Long
            }).ToArray());
        }

        ClosePositionOptions IFuturesOrderRestClient.ClosePositionOptions { get; } = new ClosePositionOptions(_exchangeName, true)
        {
            RequiredOptionalParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(ClosePositionRequest.PositionSide), typeof(SharedPositionSide), "The position side to close", SharedPositionSide.Long),
                new ParameterDescription(nameof(ClosePositionRequest.Quantity), typeof(decimal), "Quantity of the position is required", 0.1m)
            }
        };
        async Task<HttpResult<SharedId>> IFuturesOrderRestClient.ClosePositionAsync(ClosePositionRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.ClosePositionOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await Trading.PlaceOrderAsync(
                symbol,
                request.PositionSide == SharedPositionSide.Long ? OrderSide.Sell : OrderSide.Buy,
                OrderType.Market,
                request.Quantity!.Value,
                0,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedId>(result);

            return HttpResult.Ok(result, new SharedId(result.Data.Id.ToString()));
        }

        #endregion

    }
}