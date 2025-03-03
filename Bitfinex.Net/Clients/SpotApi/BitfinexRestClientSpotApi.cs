﻿using Bitfinex.Net.Enums;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Options;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.CommonObjects;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Interfaces.CommonClients;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.SharedApis;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients.SpotApi
{
    /// <inheritdoc cref="IBitfinexRestClientSpotApi" />
    internal partial class BitfinexRestClientSpotApi : RestApiClient, IBitfinexRestClientSpotApi, ISpotClient
    {
        #region fields
        internal string? AffiliateCode { get; set; }

        /// <inheritdoc />
        public new BitfinexRestOptions ClientOptions => (BitfinexRestOptions)base.ClientOptions;
        #endregion

        /// <inheritdoc />
        public string ExchangeName => "Bitfinex";

        #region Api clients
        /// <inheritdoc />
        public IBitfinexRestClientSpotApiAccount Account { get; }
        /// <inheritdoc />
        public IBitfinexRestClientSpotApiExchangeData ExchangeData { get; }
        /// <inheritdoc />
        public IBitfinexRestClientSpotApiTrading Trading { get; }
        #endregion

        /// <summary>
        /// Event triggered when an order is placed via this client
        /// </summary>
        public event Action<OrderId>? OnOrderPlaced;
        /// <summary>
        /// Event triggered when an order is canceled via this client
        /// </summary>
        public event Action<OrderId>? OnOrderCanceled;

        #region ctor

        internal BitfinexRestClientSpotApi(ILogger logger, HttpClient? httpClient, BitfinexRestOptions options) :
            base(logger, httpClient, options.Environment.RestAddress, options, options.SpotOptions)
        {
            Account = new BitfinexRestClientSpotApiAccount(this);
            ExchangeData = new BitfinexRestClientSpotApiExchangeData(this);
            Trading = new BitfinexRestClientSpotApiTrading(this);

            AffiliateCode = options.AffiliateCode;
        }

        #endregion

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new BitfinexAuthenticationProvider(credentials, ClientOptions.NonceProvider ?? new BitfinexNonceProvider());

        /// <inheritdoc />
        public override string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverTime = null)
                => BitfinexExchange.FormatSymbol(baseAsset, quoteAsset, tradingMode, deliverTime);

        /// <inheritdoc />
        protected override IMessageSerializer CreateSerializer() => new SystemTextJsonMessageSerializer();
        /// <inheritdoc />
        protected override IStreamMessageAccessor CreateAccessor() => new SystemTextJsonStreamMessageAccessor();

        #region common interface

        /// <summary>
        /// Get the name of a symbol for Bitfinex based on the base and quote asset
        /// </summary>
        /// <param name="baseAsset"></param>
        /// <param name="quoteAsset"></param>
        /// <returns></returns>
        public string GetSymbolName(string baseAsset, string quoteAsset) =>
            "t" + (baseAsset + quoteAsset).ToUpper(CultureInfo.InvariantCulture);

        internal void InvokeOrderPlaced(OrderId id)
        {
            OnOrderPlaced?.Invoke(id);
        }

        internal void InvokeOrderCanceled(OrderId id)
        {
            OnOrderCanceled?.Invoke(id);
        }

        private static KlineInterval GetTimeFrameFromTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.FromMinutes(1)) return KlineInterval.OneMinute;
            if (timeSpan == TimeSpan.FromMinutes(5)) return KlineInterval.FiveMinutes;
            if (timeSpan == TimeSpan.FromMinutes(15)) return KlineInterval.FifteenMinutes;
            if (timeSpan == TimeSpan.FromMinutes(30)) return KlineInterval.ThirtyMinutes;
            if (timeSpan == TimeSpan.FromHours(1)) return KlineInterval.OneHour;
            if (timeSpan == TimeSpan.FromHours(3)) return KlineInterval.ThreeHours;
            if (timeSpan == TimeSpan.FromHours(6)) return KlineInterval.SixHours;
            if (timeSpan == TimeSpan.FromHours(12)) return KlineInterval.TwelveHours;
            if (timeSpan == TimeSpan.FromDays(1)) return KlineInterval.OneDay;
            if (timeSpan == TimeSpan.FromDays(7)) return KlineInterval.SevenDays;
            if (timeSpan == TimeSpan.FromDays(14)) return KlineInterval.FourteenDays;
            if (timeSpan == TimeSpan.FromDays(30) || timeSpan == TimeSpan.FromDays(31)) return KlineInterval.OneMonth;

            throw new ArgumentException("Unsupported timespan for Bitfinex Klines, check supported intervals using Bitfinex.Net.Objects.TimeFrame");
        }

        private static OrderSide GetOrderSide(CommonOrderSide side)
        {
            if (side == CommonOrderSide.Sell) return OrderSide.Sell;
            if (side == CommonOrderSide.Buy) return OrderSide.Buy;

            throw new ArgumentException("Unsupported order side for Bitfinex order: " + side);
        }

        private static OrderType GetOrderType(CommonOrderType type)
        {
            if (type == CommonOrderType.Limit) return OrderType.ExchangeLimit;
            if (type == CommonOrderType.Market) return OrderType.ExchangeMarket;

            throw new ArgumentException("Unsupported order type for Bitfinex order: " + type);
        }

        internal Task<WebCallResult<T>> SendAsync<T>(
            RequestDefinition definition,
            ParameterCollection? parameters,
            CancellationToken cancellationToken) where T : class
                => SendToAddressAsync<T>(BaseAddress, definition, parameters, cancellationToken);

        internal Task<WebCallResult<T>> SendToAddressAsync<T>(
            string uri,
            RequestDefinition definition,
            ParameterCollection? parameters,
            CancellationToken cancellationToken) where T : class
                => base.SendAsync<T>(uri, definition, parameters, cancellationToken);

        /// <inheritdoc />
        public override TimeSyncInfo? GetTimeSyncInfo() => null;

        /// <inheritdoc />
        public override TimeSpan? GetTimeOffset() => null;

        /// <inheritdoc />
        public ISpotClient CommonSpotClient => this;
        public IBitfinexRestClientSpotApiShared SharedClient => this;

        async Task<WebCallResult<OrderId>> ISpotClient.PlaceOrderAsync(string symbol, CommonOrderSide side, CommonOrderType type, decimal quantity, decimal? price, string? accountId, string? clientOrderId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(nameof(symbol) + " required for Bitfinex " + nameof(ISpotClient.PlaceOrderAsync), nameof(symbol));

            int? clientId = null;
            if (clientOrderId != null) 
            {  
                if(!int.TryParse(clientOrderId, out var id))
                    throw new ArgumentException("ClientOrderId for Bitfinex should be parsable to int");
                else
                    clientId = id;
            }

            var result = await Trading.PlaceOrderAsync(symbol, GetOrderSide(side), GetOrderType(type), quantity, price ?? 0, clientOrderId: clientId, ct: ct).ConfigureAwait(false);
            if (!result)
                return result.As<OrderId>(null);

            return result.As(new OrderId
            {
                SourceObject = result.Data,
                Id = result.Data.Id.ToString(CultureInfo.InvariantCulture)
            });
        }

        async Task<WebCallResult<Order>> IBaseRestClient.GetOrderAsync(string orderId, string? symbol, CancellationToken ct)
        {
            if (!long.TryParse(orderId, out var id))
                throw new ArgumentException($"Invalid orderId provided for Bitfinex {nameof(ISpotClient.GetOrderAsync)}", nameof(orderId));

            var result = await Trading.GetOpenOrdersAsync(symbol, new[] { id }, ct: ct).ConfigureAwait(false);
            if (!result)
                return result.As<Order>(null);

            if (!result.Data.Any())
                result = await Trading.GetClosedOrdersAsync(symbol, new[] { id }, ct: ct).ConfigureAwait(false);

            if (!result.Data.Any())
                return result.AsError<Order>(new ServerError("Order with id not found"));

            var order = result.Data.FirstOrDefault();
            return result.As(new Order
            {
                SourceObject = order,
                Id = order.Id.ToString(CultureInfo.InvariantCulture),
                Symbol = order.Symbol,
                Timestamp = order.CreateTime,
                Price = order.Price,
                Quantity = order.Quantity,
                QuantityFilled = order.Quantity - order.QuantityRemaining,                
                Side = order.Side == OrderSide.Buy ? CommonOrderSide.Buy: CommonOrderSide.Sell,
                Status = order.Status == OrderStatus.Canceled ? CommonOrderStatus.Canceled: order.Status == OrderStatus.Executed ? CommonOrderStatus.Filled: CommonOrderStatus.Active,
                Type = order.Type == OrderType.ExchangeLimit ? CommonOrderType.Limit : order.Type == OrderType.ExchangeMarket ? CommonOrderType.Market: CommonOrderType.Other
            });
        }

        async Task<WebCallResult<IEnumerable<UserTrade>>> IBaseRestClient.GetOrderTradesAsync(string orderId, string? symbol, CancellationToken ct)
        {
            if (!long.TryParse(orderId, out var id))
                throw new ArgumentException($"Invalid orderId provided for Bitfinex {nameof(ISpotClient.GetOrderAsync)}", nameof(orderId));

            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException(nameof(symbol) + " required for Bitfinex " + nameof(ISpotClient.GetOrderTradesAsync), nameof(symbol));

            var result = await Trading.GetOrderTradesAsync(symbol!, id, ct: ct).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<UserTrade>>(null);

            return result.As(result.Data.Select(d => new UserTrade
            {
                SourceObject = d,
                Id = d.Id.ToString(CultureInfo.InvariantCulture),
                OrderId = d.OrderId.ToString(CultureInfo.InvariantCulture),
                Price = d.Price,
                Quantity = d.Quantity,
                Symbol = d.Symbol,
                Fee = d.Fee,
                FeeAsset = d.FeeAsset,
                Timestamp = d.Timestamp
            }));
        }

        async Task<WebCallResult<IEnumerable<Order>>> IBaseRestClient.GetOpenOrdersAsync(string? symbol, CancellationToken ct)
        {
            var result = await Trading.GetOpenOrdersAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<Order>>(null);

            return result.As(result.Data.Select(o =>
                new Order
                {
                    SourceObject = o,
                    Id = o.Id.ToString(CultureInfo.InvariantCulture),
                    Symbol = o.Symbol,
                    Timestamp = o.CreateTime,
                    Price = o.Price,
                    Quantity = o.Quantity,
                    QuantityFilled = o.Quantity - o.QuantityRemaining,
                    Side = o.Side == OrderSide.Buy ? CommonOrderSide.Buy : CommonOrderSide.Sell,
                    Status = o.Status == OrderStatus.Canceled ? CommonOrderStatus.Canceled: o.Status == OrderStatus.Executed? CommonOrderStatus.Filled: CommonOrderStatus.Active,
                    Type = o.Type == OrderType.ExchangeLimit ? CommonOrderType.Limit: o.Type == OrderType.ExchangeMarket ? CommonOrderType.Market: CommonOrderType.Other
                }
            ));
        }

        async Task<WebCallResult<IEnumerable<Order>>> IBaseRestClient.GetClosedOrdersAsync(string? symbol, CancellationToken ct)
        {
            var result = await Trading.GetClosedOrdersAsync(symbol, ct: ct).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<Order>>(null);

            return result.As(result.Data.Select(o =>
                new Order
                {
                    SourceObject = o,
                    Id = o.Id.ToString(CultureInfo.InvariantCulture),
                    Symbol = o.Symbol,
                    Timestamp = o.CreateTime,
                    Price = o.Price,
                    Quantity = o.Quantity,
                    QuantityFilled = o.Quantity - o.QuantityRemaining,
                    Side = o.Side == OrderSide.Buy ? CommonOrderSide.Buy : CommonOrderSide.Sell,
                    Status = o.Status == OrderStatus.Canceled ? CommonOrderStatus.Canceled : o.Status == OrderStatus.Executed ? CommonOrderStatus.Filled : CommonOrderStatus.Active,
                    Type = o.Type == OrderType.ExchangeLimit ? CommonOrderType.Limit : o.Type == OrderType.ExchangeMarket ? CommonOrderType.Market : CommonOrderType.Other
                }
            ));
        }

        async Task<WebCallResult<OrderId>> IBaseRestClient.CancelOrderAsync(string orderId, string? symbol, CancellationToken ct)
        {
            if (!long.TryParse(orderId, out var id))
                throw new ArgumentException($"Invalid orderId provided for Bitfinex {nameof(ISpotClient.GetOrderAsync)}", nameof(orderId));

            var result = await Trading.CancelOrderAsync(id, ct: ct).ConfigureAwait(false);
            if (!result)
                return result.As<OrderId>(null);

            return result.As(new OrderId
            {
                SourceObject = result.Data,
                Id = result.Data.Id.ToString(CultureInfo.InvariantCulture)
            });
        }

        async Task<WebCallResult<IEnumerable<Symbol>>> IBaseRestClient.GetSymbolsAsync(CancellationToken ct)
        {
            var symbols = await ExchangeData.GetSymbolsAsync(ct: ct).ConfigureAwait(false);
            if (!symbols)
                return symbols.As<IEnumerable<Symbol>>(null);

            return symbols.As(symbols.Data.Select(s =>
                new Symbol
                {
                    SourceObject = s,
                    Name = s.Key,
                    MinTradeQuantity = s.Value.MinOrderQuantity
                }));
        }

        async Task<WebCallResult<Ticker>> IBaseRestClient.GetTickerAsync(string symbol, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(nameof(symbol) + " required for Bitfinex " + nameof(ISpotClient.GetTickerAsync), nameof(symbol));

            var tickersResult = await ExchangeData.GetTickerAsync(symbol, ct: ct).ConfigureAwait(false);
            if (!tickersResult)
                return tickersResult.As<Ticker>(null);

            return tickersResult.As(new Ticker
            {
                SourceObject = tickersResult.Data,
                HighPrice = tickersResult.Data.HighPrice,
                LowPrice = tickersResult.Data.LowPrice,
                LastPrice = tickersResult.Data.LastPrice,
                Price24H = tickersResult.Data.LastPrice - tickersResult.Data.DailyChange,
                Symbol = tickersResult.Data.Symbol,
                Volume = tickersResult.Data.Volume
            });
        }

        async Task<WebCallResult<IEnumerable<Ticker>>> IBaseRestClient.GetTickersAsync(CancellationToken ct)
        {
            var tickersResult = await ExchangeData.GetTickersAsync(ct: ct).ConfigureAwait(false);
            if (!tickersResult)
                return tickersResult.As<IEnumerable<Ticker>>(null);

            return tickersResult.As(tickersResult.Data.Select(t =>
            new Ticker
            {
                SourceObject = t,
                HighPrice = t.HighPrice,
                LowPrice = t.LowPrice,
                LastPrice = t.LastPrice,
                Price24H = t.LastPrice - t.DailyChange,
                Symbol = t.Symbol,
                Volume = t.Volume
            }));
        }

        async Task<WebCallResult<IEnumerable<Kline>>> IBaseRestClient.GetKlinesAsync(string symbol, TimeSpan timespan, DateTime? startTime, DateTime? endTime, int? limit, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(nameof(symbol) + " required for Bitfinex " + nameof(ISpotClient.GetKlinesAsync), nameof(symbol));

            var klines = await ExchangeData.GetKlinesAsync(symbol, GetTimeFrameFromTimeSpan(timespan), startTime: startTime, endTime: endTime, limit: limit, ct: ct).ConfigureAwait(false);
            if (!klines)
                return klines.As<IEnumerable<Kline>>(null);

            return klines.As(klines.Data.Select(k =>
            new Kline
            {
                SourceObject = k,
                OpenPrice = k.OpenPrice,
                OpenTime = k.OpenTime,
                ClosePrice = k.ClosePrice,
                HighPrice = k.HighPrice,
                LowPrice = k.LowPrice,
                Volume = k.Volume
            }));
        }

        async Task<WebCallResult<OrderBook>> IBaseRestClient.GetOrderBookAsync(string symbol, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(nameof(symbol) + " required for Bitfinex " + nameof(ISpotClient.GetOrderAsync), nameof(symbol));

            var orderBookResult = await ExchangeData.GetOrderBookAsync(symbol, Precision.PrecisionLevel0, ct: ct).ConfigureAwait(false);
            if (!orderBookResult)
                return orderBookResult.As<OrderBook>(null);

            return orderBookResult.As(new OrderBook
            {
                SourceObject = orderBookResult.Data,
                Asks = orderBookResult.Data.Asks.Select(a => new OrderBookEntry() { Price = a.Price, Quantity = a.Quantity }),
                Bids = orderBookResult.Data.Bids.Select(b => new OrderBookEntry() { Price = b.Price, Quantity = b.Quantity }),
            });
        }

        async Task<WebCallResult<IEnumerable<Trade>>> IBaseRestClient.GetRecentTradesAsync(string symbol, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(nameof(symbol) + " required for Bitfinex " + nameof(ISpotClient.GetRecentTradesAsync), nameof(symbol));

            var tradesResult = await ExchangeData.GetTradeHistoryAsync(symbol, ct: ct).ConfigureAwait(false);
            if (!tradesResult)
                return tradesResult.As<IEnumerable<Trade>>(null);

            return tradesResult.As(tradesResult.Data.Select(t => new Trade
            {
                SourceObject = t,
                Price = t.Price,
                Quantity = t.Quantity,
                Symbol = symbol,
                Timestamp = t.Timestamp
            }));
        }

        async Task<WebCallResult<IEnumerable<Balance>>> IBaseRestClient.GetBalancesAsync(string? accountId, CancellationToken ct)
        {
            var result = await Account.GetBalancesAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<Balance>>(null);

            return result.As(result.Data.Select(b => new Balance
            {
                SourceObject = b,
                Asset = b.Asset,
                Available = b.Available,
                Total = b.Total
            }));
        }
        #endregion

        /// <inheritdoc />
        protected override Error ParseErrorResponse(int httpStatusCode, IEnumerable<KeyValuePair<string, IEnumerable<string>>> responseHeaders, IMessageAccessor accessor)
        {
            if (!accessor.IsJson)
                return new ServerError(accessor.GetOriginalString());

            if (accessor.GetNodeType() != NodeType.Array)
            {
                var error = accessor.GetValue<string?>(MessagePath.Get().Property("error"));
                var errorCode = accessor.GetValue<int?>(MessagePath.Get().Property("code"));
                var errorDesc = accessor.GetValue<string?>(MessagePath.Get().Property("error_description"));
                if (error != null && errorCode != null && errorDesc != null)
                    return new ServerError(errorCode.Value, $"{error}: {errorDesc}");
                
                var message = accessor.GetValue<string?>(MessagePath.Get().Property("message"));
                if (message != null)
                    return new ServerError(message);

                return new ServerError(accessor.GetOriginalString());
            }

            var code = accessor.GetValue<int?>(MessagePath.Get().Index(1));
            var msg = accessor.GetValue<string>(MessagePath.Get().Index(2));
            if (msg == null)
                return new ServerError(accessor.GetOriginalString());

            if (code == null)
                return new ServerError(msg);

            return new ServerError(code.Value, msg);
        }
    }
}
