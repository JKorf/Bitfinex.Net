using Bitfinex.Net;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Interfaces.Clients;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.SharedApis.Enums;
using CryptoExchange.Net.SharedApis.Interfaces;
using CryptoExchange.Net.SharedApis.Models;
using CryptoExchange.Net.SharedApis.Models.FilterOptions;
using CryptoExchange.Net.SharedApis.Models.Rest;
using CryptoExchange.Net.SharedApis.RequestModels;
using CryptoExchange.Net.SharedApis.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients.SpotApi
{
    internal partial class BitfinexRestClientSpotApi : IBitfinexRestClientSpotApiShared
    {
        public string Exchange => BitfinexExchange.ExchangeName;

        #region Kline client

        GetKlinesOptions IKlineRestClient.GetKlinesOptions { get; } = new GetKlinesOptions(true)
        {
            MaxRequestDataPoints = 10000
        };

        async Task<ExchangeWebResult<IEnumerable<SharedKline>>> IKlineRestClient.GetKlinesAsync(GetKlinesRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var interval = (Enums.KlineInterval)request.Interval;
            if (!Enum.IsDefined(typeof(Enums.KlineInterval), interval))
                return new ExchangeWebResult<IEnumerable<SharedKline>>(Exchange, new ArgumentError("Interval not supported"));

            // Determine page token
            DateTime? fromTimestamp = null;
            if (pageToken is DateTimeToken dateTimeToken)
                fromTimestamp = dateTimeToken.LastTime;

            // Get data
            var baseAsset = request.BaseAsset == "USDT" ? "UST" : request.BaseAsset;
            var quoteAsset = request.QuoteAsset == "USDT" ? "UST" : request.QuoteAsset;

            var result = await ExchangeData.GetKlinesAsync(
                request.GetSymbol(FormatSymbol),
                interval,
                startTime: fromTimestamp ?? request.Filter?.StartTime,
                endTime: request.Filter?.EndTime?.AddSeconds(-1),
                limit: request.Filter?.Limit ?? 10000,
                sorting: Sorting.OldFirst,
                ct: ct
                ).ConfigureAwait(false);

            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedKline>>(Exchange, default);

            // Get next token
            DateTimeToken? nextToken = null;
            if (request.Filter?.StartTime != null && result.Data.Any())
            {
                var maxOpenTime = result.Data.Max(x => x.OpenTime);
                if (maxOpenTime < request.Filter.EndTime!.Value.AddSeconds(-(int)request.Interval))
                    nextToken = new DateTimeToken(maxOpenTime.AddSeconds((int)interval));
            }

            return result.AsExchangeResult(Exchange, result.Data.Select(x => new SharedKline(x.OpenTime, x.ClosePrice, x.HighPrice, x.LowPrice, x.OpenPrice, x.Volume)), nextToken);
        }

        #endregion

        #region Asset client

        async Task<ExchangeWebResult<IEnumerable<SharedAsset>>> IAssetRestClient.GetAssetsAsync(CancellationToken ct)
        {
            // Execute needed config requests in parallel
            var assetSymbols = ExchangeData.GetAssetSymbolsAsync(ct: ct);
            var assetList = ExchangeData.GetAssetsListAsync(ct: ct);
            var assetMethods = ExchangeData.GetAssetDepositWithdrawalMethodsAsync(ct: ct);
            var assetFees = ExchangeData.GetAssetWithdrawalFeesAsync(ct: ct);
            var assetTxStatus = ExchangeData.GetDepositWithdrawalStatusAsync(ct: ct);
            await Task.WhenAll(assetList, assetMethods).ConfigureAwait(false);
            if (!assetSymbols.Result)
                return assetSymbols.Result.AsExchangeResult<IEnumerable<SharedAsset>>(Exchange, default);
            if (!assetList.Result)
                return assetList.Result.AsExchangeResult<IEnumerable<SharedAsset>>(Exchange, default);
            if (!assetMethods.Result)
                return assetMethods.Result.AsExchangeResult<IEnumerable<SharedAsset>>(Exchange, default);
            if (!assetFees.Result)
                return assetFees.Result.AsExchangeResult<IEnumerable<SharedAsset>>(Exchange, default);
            if (!assetTxStatus.Result)
                return assetTxStatus.Result.AsExchangeResult<IEnumerable<SharedAsset>>(Exchange, default);

            return assetList.Result.AsExchangeResult<IEnumerable<SharedAsset>>(Exchange, 
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
                        }).ToList()
                    };
                }).Where(x => x != null).ToList()!
            );
        }

        #endregion

        #region Spot Symbol client

        async Task<ExchangeWebResult<IEnumerable<SharedSpotSymbol>>> ISpotSymbolRestClient.GetSpotSymbolsAsync(CancellationToken ct)
        {
            var result = await ExchangeData.GetSymbolsAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedSpotSymbol>>(Exchange, default);

            return result.AsExchangeResult(Exchange, result.Data.Select(s => new SharedSpotSymbol(s.Key.Split(new[] { ':' })[0], s.Key.Split(new[] { ':' })[1], s.Key)
            {
                MinTradeQuantity = s.Value.MinOrderQuantity,
                MaxTradeQuantity = s.Value.MaxOrderQuantity
            }));
        }

        #endregion

        #region Ticker client

        async Task<ExchangeWebResult<SharedTicker>> ITickerRestClient.GetTickerAsync(GetTickerRequest request, CancellationToken ct)
        {
            var baseAsset = request.BaseAsset == "USDT" ? "UST" : request.BaseAsset;
            var quoteAsset = request.QuoteAsset == "USDT" ? "UST" : request.QuoteAsset;

            var result = await ExchangeData.GetTickerAsync(request.GetSymbol(FormatSymbol), ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedTicker>(Exchange, default);

            return result.AsExchangeResult(Exchange, new SharedTicker(result.Data.Symbol, result.Data.LastPrice, result.Data.HighPrice, result.Data.LowPrice));
        }

        async Task<ExchangeWebResult<IEnumerable<SharedTicker>>> ITickerRestClient.GetTickersAsync(ApiType? apiType, CancellationToken ct)
        {
            var result = await ExchangeData.GetTickersAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedTicker>>(Exchange, default);

            return result.AsExchangeResult<IEnumerable<SharedTicker>>(Exchange, result.Data.Select(x => new SharedTicker(x.Symbol, x.LastPrice, x.HighPrice, x.LowPrice)));
        }

        #endregion

        #region Recent Trade client

        GetRecentTradesOptions IRecentTradeRestClient.GetRecentTradesOptions { get; } = new GetRecentTradesOptions(10000);

        async Task<ExchangeWebResult<IEnumerable<SharedTrade>>> IRecentTradeRestClient.GetRecentTradesAsync(GetRecentTradesRequest request, CancellationToken ct)
        {
            var result = await ExchangeData.GetTradeHistoryAsync(
                request.GetSymbol(FormatSymbol),
                limit: request.Limit,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedTrade>>(Exchange, default);

            return result.AsExchangeResult(Exchange, result.Data.Select(x => new SharedTrade(x.Quantity, x.Price, x.Timestamp)));
        }

        #endregion

        #region Balance client

        async Task<ExchangeWebResult<IEnumerable<SharedBalance>>> IBalanceRestClient.GetBalancesAsync(ApiType? apiType, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            var result = await Account.GetBalancesAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedBalance>>(Exchange, default);

            return result.AsExchangeResult(Exchange, result.Data.Select(x => new SharedBalance(x.Asset, x.Available ?? 0, x.Total)));
        }

        #endregion

        #region Spot Order client

        PlaceSpotOrderOptions ISpotOrderRestClient.PlaceSpotOrderOptions { get; } = new PlaceSpotOrderOptions(
            new[]
            {
                SharedOrderType.Limit,
                SharedOrderType.Market,
                SharedOrderType.LimitMaker
            },
            new[]
            {
                SharedTimeInForce.GoodTillCanceled,
                SharedTimeInForce.ImmediateOrCancel,
                SharedTimeInForce.FillOrKill
            },
            new SharedQuantitySupport(
                SharedQuantityType.BaseAssetQuantity,
                SharedQuantityType.BaseAssetQuantity,
                SharedQuantityType.BaseAssetQuantity,
                SharedQuantityType.BaseAssetQuantity));

        async Task<ExchangeWebResult<SharedId>> ISpotOrderRestClient.PlaceOrderAsync(PlaceSpotOrderRequest request, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            if (request.OrderType == SharedOrderType.Other)
                throw new ArgumentException("OrderType can't be `Other`", nameof(request.OrderType));

            int clientOrderId = 0;
            if (request.ClientOrderId != null && !int.TryParse(request.ClientOrderId, out clientOrderId))
                return new ExchangeWebResult<SharedId>(Exchange, new ArgumentError("ClientOrderId needs to be parsable to `int` for `Bitfinex`"));

            var result = await Trading.PlaceOrderAsync(
                request.GetSymbol(FormatSymbol),
                request.Side == SharedOrderSide.Buy ? Enums.OrderSide.Buy : Enums.OrderSide.Sell,
                GetPlaceOrderType(request.OrderType, request.TimeInForce),
                quantity: request.Quantity ?? 0,
                flags: request.OrderType == SharedOrderType.LimitMaker ? Enums.OrderFlags.PostOnly : null,
                price: request.Price ?? 0,
                clientOrderId: request.ClientOrderId != null ? clientOrderId: null).ConfigureAwait(false);

            if (!result)
                return result.AsExchangeResult<SharedId>(Exchange, default);

            return result.AsExchangeResult(Exchange, new SharedId(result.Data.Id.ToString()));
        }

        async Task<ExchangeWebResult<SharedSpotOrder>> ISpotOrderRestClient.GetOrderAsync(GetOrderRequest request, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedSpotOrder>(Exchange, new ArgumentError("Invalid order id"));

            var symbol = request.GetSymbol(FormatSymbol);
            var result = await Trading.GetOpenOrdersAsync(symbol, new[] { orderId }, ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedSpotOrder>(Exchange, null);

            if (!result.Data.Any())
                result = await Trading.GetClosedOrdersAsync(symbol, new[] { orderId }, ct: ct).ConfigureAwait(false);

            if (!result.Data.Any())
                return result.AsExchangeError<SharedSpotOrder>(Exchange, new ServerError($"Order with id {orderId} not found"));

            var order = result.Data.Single();
            return result.AsExchangeResult(Exchange, new SharedSpotOrder(
                order.Symbol,
                order.Id.ToString(),
                ParseOrderType(order.Type, order.Flags),
                order.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                ParseOrderStatus(order.Status),
                order.CreateTime)
            {
                ClientOrderId = order.ClientOrderId?.ToString(),
                AveragePrice = order.PriceAverage,
                Price = order.Price,
                Quantity = order.Quantity,
                QuantityFilled = order.Quantity - order.QuantityRemaining,
                TimeInForce = ParseTimeInForce(order.Type, order.Flags),
                UpdateTime = order.UpdateTime
            });
        }

        async Task<ExchangeWebResult<IEnumerable<SharedSpotOrder>>> ISpotOrderRestClient.GetOpenOrdersAsync(GetSpotOpenOrdersRequest request, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            string? symbol = null;
            if (request.BaseAsset != null && request.QuoteAsset != null)
                symbol = FormatSymbol(request.BaseAsset, request.QuoteAsset, request.ApiType);

            var result = await Trading.GetOpenOrdersAsync(symbol, ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedSpotOrder>>(Exchange, null);

            return result.AsExchangeResult(Exchange, result.Data.Select(x => new SharedSpotOrder(
                x.Symbol,
                x.Id.ToString(),
                ParseOrderType(x.Type, x.Flags),
                x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                ParseOrderStatus(x.Status),
                x.CreateTime)
            {
                ClientOrderId = x.ClientOrderId?.ToString(),
                AveragePrice = x.PriceAverage,
                Price = x.Price,
                Quantity = x.Quantity,
                QuantityFilled = x.Quantity - x.QuantityRemaining,
                TimeInForce = ParseTimeInForce(x.Type, x.Flags),
                UpdateTime = x.UpdateTime
            }));
        }

        async Task<ExchangeWebResult<IEnumerable<SharedSpotOrder>>> ISpotOrderRestClient.GetClosedOrdersAsync(GetSpotClosedOrdersRequest request, INextPageToken? pageToken, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            // Determine page token
            DateTime? fromTimestamp = null;
            if (pageToken is DateTimeToken dateTimeToken)
                fromTimestamp = dateTimeToken.LastTime;

            // Get data
            var result = await Trading.GetClosedOrdersAsync(request.GetSymbol(FormatSymbol),
                startTime: fromTimestamp ?? request.Filter?.StartTime,
                endTime: request.Filter?.EndTime,
                limit: request.Filter?.Limit ?? 100,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedSpotOrder>>(Exchange, null);

            // Get next token
            DateTimeToken? nextToken = null;
            if (result.Data.Count() == (request.Filter?.Limit ?? 100))
                nextToken = new DateTimeToken(result.Data.Max(o => o.CreateTime));

            return result.AsExchangeResult(Exchange, result.Data.Select(x => new SharedSpotOrder(
                x.Symbol,
                x.Id.ToString(),
                ParseOrderType(x.Type, x.Flags),
                x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                ParseOrderStatus(x.Status),
                x.CreateTime)
            {
                ClientOrderId = x.ClientOrderId?.ToString(),
                AveragePrice = x.PriceAverage,
                Price = x.Price,
                Quantity = x.Quantity,
                QuantityFilled = x.Quantity - x.QuantityRemaining,
                TimeInForce = ParseTimeInForce(x.Type, x.Flags),
                UpdateTime = x.UpdateTime
            }), nextToken);
        }

        async Task<ExchangeWebResult<IEnumerable<SharedUserTrade>>> ISpotOrderRestClient.GetOrderTradesAsync(GetOrderTradesRequest request, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<IEnumerable<SharedUserTrade>>(Exchange, new ArgumentError("Invalid order id"));

            var order = await Trading.GetOrderTradesAsync(
                request.GetSymbol(FormatSymbol), orderId).ConfigureAwait(false);
            if (!order)
                return order.AsExchangeResult<IEnumerable<SharedUserTrade>>(Exchange, default);

            return order.AsExchangeResult(Exchange, order.Data.Select(x => new SharedUserTrade(
                x.Symbol,
                x.OrderId.ToString(),
                x.Id.ToString(),
                x.Quantity,
                x.Price,
                x.Timestamp)
            {
                Fee = x.Fee,
                FeeAsset = x.FeeAsset,
                Role = x.Maker == true ? SharedRole.Maker : x.Maker == false ? SharedRole.Taker : null
            }));
        }

        async Task<ExchangeWebResult<IEnumerable<SharedUserTrade>>> ISpotOrderRestClient.GetUserTradesAsync(GetUserTradesRequest request, INextPageToken? pageToken, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            // Determine page token
            DateTime? fromTimestamp = null;
            if (pageToken is DateTimeToken dateTimeToken)
                fromTimestamp = dateTimeToken.LastTime;

            // Get data
            var order = await Trading.GetUserTradesAsync(
                request.GetSymbol(FormatSymbol),
                startTime: fromTimestamp ?? request.Filter?.StartTime,
                endTime: request.Filter?.EndTime,
                limit: request.Filter?.Limit ?? 1000).ConfigureAwait(false);
            if (!order)
                return order.AsExchangeResult<IEnumerable<SharedUserTrade>>(Exchange, default);

            // Get next token
            DateTimeToken? nextToken = null;
            if (order.Data.Count() == (request.Filter?.Limit ?? 1000))
                nextToken = new DateTimeToken(order.Data.Max(o => o.Timestamp));

            return order.AsExchangeResult(Exchange, order.Data.Select(x => new SharedUserTrade(
                x.Symbol,
                x.OrderId.ToString(),
                x.Id.ToString(),
                x.Quantity,
                x.Price,
                x.Timestamp)
            {
                Fee = x.Fee,
                FeeAsset = x.FeeAsset,
                Role = x.Maker == true ? SharedRole.Maker : x.Maker == false ? SharedRole.Taker : null
            }), nextToken);
        }

        async Task<ExchangeWebResult<SharedId>> ISpotOrderRestClient.CancelOrderAsync(CancelOrderRequest request, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedId>(Exchange, new ArgumentError("Invalid order id"));

            var order = await Trading.CancelOrderAsync(orderId).ConfigureAwait(false);
            if (!order)
                return order.AsExchangeResult<SharedId>(Exchange, default);

            return order.AsExchangeResult(Exchange, new SharedId(order.Data.ToString()));
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
            if (type == SharedOrderType.Market) return Enums.OrderType.Market;

            throw new ArgumentException($"The combination of order type `{type}` and time in force `{tif}` in invalid");
        }

        #endregion

        #region Deposit client

        async Task<ExchangeWebResult<IEnumerable<SharedDepositAddress>>> IDepositRestClient.GetDepositAddressesAsync(GetDepositAddressesRequest request, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(request.Network))
                return new ExchangeWebResult<IEnumerable<SharedDepositAddress>>(Exchange, new ArgumentError("Network is required"));

            var depositAddresses = await Account.GetDepositAddressAsync(request.Network!, WithdrawWallet.Deposit, false, ct).ConfigureAwait(false);
            if (!depositAddresses)
                return depositAddresses.AsExchangeResult<IEnumerable<SharedDepositAddress>>(Exchange, default);

            return depositAddresses.AsExchangeResult<IEnumerable<SharedDepositAddress>>(Exchange, new[] { new SharedDepositAddress(depositAddresses.Data.Data!.Asset, !string.IsNullOrEmpty(depositAddresses.Data.Data.PoolAddress) ? depositAddresses.Data.Data.PoolAddress : depositAddresses.Data.Data.Address)
            {
                Network = request.Network,
                Tag = !string.IsNullOrEmpty(depositAddresses.Data.Data.PoolAddress) ? depositAddresses.Data.Data.Address : null,
            }
            });
        }

        async Task<ExchangeWebResult<IEnumerable<SharedDeposit>>> IDepositRestClient.GetDepositsAsync(GetDepositsRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            // Determine page token
            DateTime? offset = null;
            if (pageToken is DateTimeToken timeToken)
                offset = timeToken.LastTime;

            // Get data
            var deposits = await Account.GetMovementsAsync(
                request.Asset,
                startTime: offset ?? request.Filter?.StartTime,
                endTime: request.Filter?.EndTime,
                limit: request.Filter?.Limit ?? 1000,
                ct: ct).ConfigureAwait(false);
            if (!deposits)
                return deposits.AsExchangeResult<IEnumerable<SharedDeposit>>(Exchange, default);

            // Determine next token
            DateTimeToken? nextToken = null;
            if (deposits.Data.Count() == (request.Filter?.Limit ?? 1000))
                nextToken = new DateTimeToken(deposits.Data.Max(x => x.StartTime));

            return deposits.AsExchangeResult(Exchange, deposits.Data.Where(x => x.Quantity > 0).Select(x => new SharedDeposit(x.Asset, x.Quantity, x.Status == "COMPLETED", x.StartTime)
            {
                TransactionId = x.TransactionId
            }), nextToken);
        }

        #endregion

        #region Order Book client
        GetOrderBookOptions IOrderBookRestClient.GetOrderBookOptions { get; } = new GetOrderBookOptions(new[] { 1, 25, 100 });
        async Task<ExchangeWebResult<SharedOrderBook>> IOrderBookRestClient.GetOrderBookAsync(GetOrderBookRequest request, CancellationToken ct)
        {
            var validationError = ((IOrderBookRestClient)this).GetOrderBookOptions.Validate(request);
            if (validationError != null)
                return new ExchangeWebResult<SharedOrderBook>(Exchange, validationError);

            var result = await ExchangeData.GetOrderBookAsync(
                request.GetSymbol(FormatSymbol),
                Precision.PrecisionLevel0,
                limit: request.Limit,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedOrderBook>(Exchange, default);

            return result.AsExchangeResult(Exchange, new SharedOrderBook(result.Data.Asks, result.Data.Bids));
        }
        #endregion

        #region Trade History client

        async Task<ExchangeWebResult<IEnumerable<SharedTrade>>> ITradeHistoryRestClient.GetTradeHistoryAsync(GetTradeHistoryRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            // Determine page token
            DateTime? fromTimestamp = null;
            if (pageToken is DateTimeToken dateTimeToken)
                fromTimestamp = dateTimeToken.LastTime;

            // Get data
            var result = await ExchangeData.GetTradeHistoryAsync(
                request.GetSymbol(FormatSymbol),
                startTime: fromTimestamp ?? request.StartTime,
                endTime: request.EndTime,
                limit: 10000,
                sorting: Sorting.OldFirst).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedTrade>>(Exchange, default);

            // Get next token
            DateTimeToken? nextToken = null;
            if (result.Data.Count() == ( 10000))
                nextToken = new DateTimeToken(result.Data.Max(o => o.Timestamp));

            // Return
            return result.AsExchangeResult(Exchange, result.Data./*Where(x => x. < request.EndTime).*/Select(x => new SharedTrade(x.Quantity, x.Price, x.Timestamp)), nextToken);
        }
        #endregion

        #region Withdrawal client

        async Task<ExchangeWebResult<IEnumerable<SharedWithdrawal>>> IWithdrawalRestClient.GetWithdrawalsAsync(GetWithdrawalsRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            // Determine page token
            DateTime? offset = null;
            if (pageToken is DateTimeToken timeToken)
                offset = timeToken.LastTime;

            // Get data
            var withdrawals = await Account.GetMovementsAsync(
                request.Asset,
                startTime: offset ?? request.Filter?.StartTime,
                endTime: request.Filter?.EndTime,
                limit: request.Filter?.Limit ?? 1000,
                ct: ct).ConfigureAwait(false);
            if (!withdrawals)
                return withdrawals.AsExchangeResult<IEnumerable<SharedWithdrawal>>(Exchange, default);

            // Determine next token
            DateTimeToken? nextToken = null;
            if (withdrawals.Data.Count() == (request.Filter?.Limit ?? 1000))
                nextToken = new DateTimeToken(withdrawals.Data.Max(x => x.StartTime));

            return withdrawals.AsExchangeResult(Exchange, withdrawals.Data.Where(x => x.Quantity < 0).Select(x => new SharedWithdrawal(x.Asset, x.Address, Math.Abs(x.Quantity), x.Status == "COMPLETED", x.StartTime)
            {
                TransactionId = x.TransactionId
            }), nextToken);
        }

        #endregion

        #region Withdraw client

        WithdrawOptions IWithdrawRestClient.WithdrawOptions { get; } = new WithdrawOptions();

        async Task<ExchangeWebResult<SharedId>> IWithdrawRestClient.WithdrawAsync(WithdrawRequest request, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(request.Network))
                return new ExchangeWebResult<SharedId>(Exchange, new ArgumentError("Network is required"));

            var validationError = ((IWithdrawRestClient)this).WithdrawOptions.Validate(request);
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
                return withdrawal.AsExchangeResult<SharedId>(Exchange, default);

            return withdrawal.AsExchangeResult(Exchange, new SharedId(withdrawal.Data.Data.WithdrawalId.ToString()));
        }

        #endregion
    }
}
