using Bitfinex.Net.Converters;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CryptoExchange.Net.Authentication;
using System.Threading;
using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Models.Socket;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using Bitfinex.Net.Objects.Options;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Objects.Sockets;
using Bitfinex.Net.Objects.Sockets.Subscriptions;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Enums;
using System.Collections.Generic;
using System.Linq;
using CryptoExchange.Net.Sockets;
using System.Globalization;
using Bitfinex.Net.Objects.Sockets.Queries;
using Bitfinex.Net.ExtensionMethods;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Clients;

namespace Bitfinex.Net.Clients.SpotApi
{
    /// <inheritdoc cref="IBitfinexSocketClientSpotApi" />
    public class BitfinexSocketClientSpotApi : SocketApiClient, IBitfinexSocketClientSpotApi
    {
        private static readonly MessagePath _0Path = MessagePath.Get().Index(0);
        private static readonly MessagePath _eventPath = MessagePath.Get().Property("event");
        private static readonly MessagePath _channelPath = MessagePath.Get().Property("channel");
        private static readonly MessagePath _symbolPath = MessagePath.Get().Property("symbol");
        private static readonly MessagePath _precPath = MessagePath.Get().Property("prec");
        private static readonly MessagePath _freqPath = MessagePath.Get().Property("freq");
        private static readonly MessagePath _lenPath = MessagePath.Get().Property("len");
        private static readonly MessagePath _keyPath = MessagePath.Get().Property("key");
        private static readonly MessagePath _chanIdPath = MessagePath.Get().Property("chanId");

        #region fields
        private readonly JsonSerializer _bookSerializer = new JsonSerializer();
        private readonly JsonSerializer _fundingBookSerializer = new JsonSerializer();
        private readonly Random _random = new Random();
        private readonly string? _affCode;

        /// <inheritdoc />
        public new BitfinexSocketOptions ClientOptions => (BitfinexSocketOptions)base.ClientOptions;

        #endregion

        #region ctor
        internal BitfinexSocketClientSpotApi(ILogger logger, BitfinexSocketOptions options) :
            base(logger, options.Environment.SocketAddress, options, options.SpotOptions)
        {
            UnhandledMessageExpected = true;

            AddSystemSubscription(new BitfinexInfoSubscription(_logger));

            _affCode = options.AffiliateCode;
            _bookSerializer.Converters.Add(new OrderBookEntryConverter());
            _fundingBookSerializer.Converters.Add(new OrderBookFundingEntryConverter());
        }
        #endregion

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new BitfinexAuthenticationProvider(credentials, ClientOptions.NonceProvider ?? new BitfinexNonceProvider());

        /// <inheritdoc />
        protected override Query GetAuthenticationRequest()
        {
            var authProvider = (BitfinexAuthenticationProvider)AuthenticationProvider!;
            var n = authProvider.GetNonce().ToString();
            var authentication = new BitfinexAuthentication
            {
                Event = "auth",
                ApiKey = authProvider.GetApiKey(),
                Nonce = n,
                Payload = "AUTH" + n
            };

            authentication.Signature = authProvider.Sign(authentication.Payload).ToLower(CultureInfo.InvariantCulture);
            return new BitfinexAuthQuery(authentication);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<DataEvent<BitfinexStreamTicker>> handler, CancellationToken ct = default)
        {
            var subscription = new BitfinexSubscription<BitfinexStreamTicker>(_logger, "ticker", symbol, x => handler(x.As(x.Data.First())));
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToFundingTickerUpdatesAsync(string symbol, Action<DataEvent<BitfinexStreamFundingTicker>> handler, CancellationToken ct = default)
        {
            var subscription = new BitfinexSubscription<BitfinexStreamFundingTicker>(_logger, "ticker", symbol, x => handler(x.As(x.Data.First())));
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, Precision precision, Frequency frequency, int length, Action<DataEvent<IEnumerable<BitfinexOrderBookEntry>>> handler, Action<DataEvent<int>>? checksumHandler = null, CancellationToken ct = default)
        {
            length.ValidateIntValues(nameof(length), 1, 25, 100, 250);
            if (precision == Precision.R0)
                throw new ArgumentException("Invalid precision R0, use SubscribeToRawBookUpdatesAsync instead");

            var subscription = new BitfinexSubscription<BitfinexOrderBookEntry>(_logger, "book", symbol, handler, checksumHandler, false, precision, frequency, length);
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToFundingOrderBookUpdatesAsync(string symbol, Precision precision, Frequency frequency, int length, Action<DataEvent<IEnumerable<BitfinexOrderBookFundingEntry>>> handler, Action<DataEvent<int>>? checksumHandler = null, CancellationToken ct = default)
        {
            length.ValidateIntValues(nameof(length), 1, 25, 100, 250);
            if (precision == Precision.R0)
                throw new ArgumentException("Invalid precision R0, use SubscribeToFundingRawOrderBookUpdatesAsync instead");

            var subscription = new BitfinexSubscription<BitfinexOrderBookFundingEntry>(_logger, "book", symbol, handler, checksumHandler, false, precision, frequency, length);
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToRawOrderBookUpdatesAsync(string symbol, int limit, Action<DataEvent<IEnumerable<BitfinexRawOrderBookEntry>>> handler, Action<DataEvent<int>>? checksumHandler = null, CancellationToken ct = default)
        {
            var subscription = new BitfinexSubscription<BitfinexRawOrderBookEntry>(_logger, "book", symbol, handler, checksumHandler, false, Precision.R0, Frequency.Realtime, limit);
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToRawFundingOrderBookUpdatesAsync(string symbol, int limit, Action<DataEvent<IEnumerable<BitfinexRawOrderBookFundingEntry>>> handler, Action<DataEvent<int>>? checksumHandler = null, CancellationToken ct = default)
        {
            var subscription = new BitfinexSubscription<BitfinexRawOrderBookFundingEntry>(_logger, "book", symbol, handler, checksumHandler, false, Precision.R0, Frequency.Realtime, limit);
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<DataEvent<IEnumerable<BitfinexTradeSimple>>> handler, CancellationToken ct = default)
        {
            var subscription = new BitfinexSubscription<BitfinexTradeSimple>(_logger, "trades", symbol, handler);
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, KlineInterval interval, Action<DataEvent<IEnumerable<BitfinexKline>>> handler, CancellationToken ct = default)
        {
            var subscription = new BitfinexSubscription<BitfinexKline>(_logger, "candles", symbol, handler, key: $"trade:{JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false))}:" + symbol, sendSymbol: false);
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToLiquidationUpdatesAsync(Action<DataEvent<IEnumerable<BitfinexLiquidation>>> handler, CancellationToken ct = default)
        {
            var subscription = new BitfinexSubscription<BitfinexLiquidation>(_logger, "status", null, handler, key: $"liq:global");
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToDerivativesUpdatesAsync(string symbol, Action<DataEvent<BitfinexDerivativesStatusUpdate>> handler, CancellationToken ct = default)
        {
            var subscription = new BitfinexSubscription<BitfinexDerivativesStatusUpdate>(_logger, "candles", null, x => handler(x.As(x.Data.Single())), key: $"deriv:" + symbol);
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToUserUpdatesAsync(
             Action<DataEvent<IEnumerable<BitfinexOrder>>>? orderHandler = null,
             Action<DataEvent<IEnumerable<BitfinexPosition>>>? positionHandler = null,
             Action<DataEvent<IEnumerable<BitfinexFundingOffer>>>? fundingOfferHandler = null,
             Action<DataEvent<IEnumerable<BitfinexFundingCredit>>>? fundingCreditHandler = null,
             Action<DataEvent<IEnumerable<BitfinexFunding>>>? fundingLoanHandler = null,
             Action<DataEvent<IEnumerable<BitfinexWallet>>>? walletHandler = null,
             Action<DataEvent<BitfinexBalance>>? balanceHandler = null,
             Action<DataEvent<BitfinexTradeDetails>>? tradeHandler = null,
             Action<DataEvent<BitfinexFundingTrade>>? fundingTradeHandler = null,
             Action<DataEvent<BitfinexFundingInfo>>? fundingInfoHandler = null,
             Action<DataEvent<BitfinexMarginBase>>? marginBaseHandler = null,
             Action<DataEvent<BitfinexMarginSymbol>>? marginSymbolHandler = null,
             CancellationToken ct = default)
        {
            var subscription = new BitfinexUserSubscription(_logger, positionHandler, walletHandler, orderHandler, fundingOfferHandler, fundingCreditHandler, fundingLoanHandler, balanceHandler, tradeHandler, fundingTradeHandler, fundingInfoHandler, marginBaseHandler, marginSymbolHandler);
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<BitfinexOrder>> PlaceOrderAsync(OrderSide side, OrderType type, string symbol, decimal quantity, long? groupId = null, long? clientOrderId = null, decimal? price = null, decimal? priceTrailing = null, decimal? priceAuxiliaryLimit = null, decimal? priceOcoStop = null, OrderFlags? flags = null, int? leverage = null, DateTime? cancelTime = null, string? affiliateCode = null)
        {
            _logger.Log(LogLevel.Information, "Going to place order");
            clientOrderId ??= GenerateClientOrderId();

            var affCode = affiliateCode ?? _affCode;
            var query = new BitfinexSocketQuery(clientOrderId.ToString(), BitfinexEventType.OrderNew, new BitfinexNewOrder
            {
                Amount = side == OrderSide.Buy ? quantity : -quantity,
                OrderType = type,
                Symbol = symbol,
                Price = price,
                ClientOrderId = clientOrderId,
                Flags = flags,
                GroupId = groupId,
                PriceAuxiliaryLimit = priceAuxiliaryLimit,
                PriceOCOStop = priceOcoStop,
                PriceTrailing = priceTrailing,
                Leverage = leverage,
                CancelAfter = cancelTime,
                Meta = affCode == null ? null : new BitfinexMeta() { AffiliateCode = affCode }
            });

            var bitfinexQuery = new BitfinexQuery<BitfinexOrder>(query);
            var result = await QueryAsync(BaseAddress.AppendPath("ws/2"), bitfinexQuery).ConfigureAwait(false);
            return result.As<BitfinexOrder>(result.Data?.Data.Data);
        }

        /// <inheritdoc />
        public async Task<CallResult<BitfinexOrder>> UpdateOrderAsync(long orderId, decimal? price = null, decimal? quantity = null, decimal? delta = null, decimal? priceAuxiliaryLimit = null, decimal? priceTrailing = null, OrderFlags? flags = null)
        {
            _logger.Log(LogLevel.Information, "Going to update order " + orderId);
            var query = new BitfinexSocketQuery(orderId.ToString(CultureInfo.InvariantCulture), BitfinexEventType.OrderUpdate, new BitfinexUpdateOrder
            {
                OrderId = orderId,
                Amount = quantity,
                Price = price,
                Flags = flags,
                PriceAuxiliaryLimit = priceAuxiliaryLimit?.ToString(CultureInfo.InvariantCulture),
                PriceTrailing = priceTrailing?.ToString(CultureInfo.InvariantCulture)
            });

            var bitfinexQuery = new BitfinexQuery<BitfinexOrder>(query);
            var result = await QueryAsync(BaseAddress.AppendPath("ws/2"), bitfinexQuery).ConfigureAwait(false);
            return result.As<BitfinexOrder>(result.Data?.Data.Data);
        }

        /// <inheritdoc />
        public async Task<CallResult<IEnumerable<BitfinexOrder>>> CancelAllOrdersAsync()
        {
            var query = new BitfinexSocketQuery(null, BitfinexEventType.OrderCancelMulti, new BitfinexMultiCancel { All = true });
            var bitfinexQuery = new BitfinexQuery<List<BitfinexOrder>>(query);
            var result = await QueryAsync(BaseAddress.AppendPath("ws/2"), bitfinexQuery).ConfigureAwait(false);
            return result.As<IEnumerable<BitfinexOrder>>(result.Data?.Data.Data);
        }

        /// <inheritdoc />
        public async Task<CallResult<BitfinexOrder>> CancelOrderAsync(long orderId)
        {
            var query = new BitfinexSocketQuery(orderId.ToString(CultureInfo.InvariantCulture), BitfinexEventType.OrderCancel, new Dictionary<string, long> { ["id"] = orderId });
            var bitfinexQuery = new BitfinexQuery<BitfinexOrder>(query);
            var result = await QueryAsync(BaseAddress.AppendPath("ws/2"), bitfinexQuery).ConfigureAwait(false);
            return result.As<BitfinexOrder>(result.Data?.Data.Data);
        }

        /// <inheritdoc />
        public async Task<CallResult<IEnumerable<BitfinexOrder>>> CancelOrdersByGroupIdAsync(long groupOrderId)
        {
            return await CancelOrdersAsync(null, null, new Dictionary<long, long?> { { groupOrderId, null } }).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<IEnumerable<BitfinexOrder>>> CancelOrdersByGroupIdsAsync(IEnumerable<long> groupOrderIds)
        {
            groupOrderIds.ValidateNotNull(nameof(groupOrderIds));
            return await CancelOrdersAsync(null, null, groupOrderIds.ToDictionary(v => v, k => (long?)null)).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<IEnumerable<BitfinexOrder>>> CancelOrdersAsync(IEnumerable<long> orderIds)
        {
            orderIds.ValidateNotNull(nameof(orderIds));
            return await CancelOrdersAsync(orderIds, null).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<IEnumerable<BitfinexOrder>>> CancelOrdersByClientOrderIdsAsync(Dictionary<long, DateTime> clientOrderIds)
        {
            return await CancelOrdersAsync(null, clientOrderIds).ConfigureAwait(false);
        }

        private async Task<CallResult<IEnumerable<BitfinexOrder>>> CancelOrdersAsync(IEnumerable<long>? orderIds = null, Dictionary<long, DateTime>? clientOrderIds = null, Dictionary<long, long?>? groupOrderIds = null)
        {
            if (orderIds == null && clientOrderIds == null && groupOrderIds == null)
                throw new ArgumentException("Either orderIds, clientOrderIds or groupOrderIds should be provided");

            _logger.Log(LogLevel.Information, "Going to cancel multiple orders");
            var cancelObject = new BitfinexMultiCancel { OrderIds = orderIds };
            if (clientOrderIds != null)
            {
                cancelObject.ClientIds = new object[clientOrderIds.Count][];
                for (var i = 0; i < cancelObject.ClientIds.Length; i++)
                {
                    cancelObject.ClientIds[i] = new object[]
                    {
                        clientOrderIds.ElementAt(i).Key,
                        clientOrderIds.ElementAt(i).Value.ToString("yyyy-MM-dd")
                    };
                }
            }
            if (groupOrderIds != null)
                cancelObject.GroupIds = new[] { groupOrderIds.Select(g => g.Key).ToArray() };

            var query = new BitfinexSocketQuery(null, BitfinexEventType.OrderCancelMulti, cancelObject);
            var bitfinexQuery = new BitfinexQuery<List<BitfinexOrder>>(query);
            var result = await QueryAsync(BaseAddress.AppendPath("ws/2"), bitfinexQuery).ConfigureAwait(false);
            return result.As<IEnumerable<BitfinexOrder>>(result.Data?.Data.Data);
        }

        /// <inheritdoc />
        public async Task<CallResult<BitfinexFundingOffer>> SubmitFundingOfferAsync(FundingOfferType type, string symbol, decimal quantity, decimal price, int period, int? flags = null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "type", EnumConverter.GetString(type) },
                { "symbol", symbol },
                { "amount", quantity },
                { "rate", price },
                { "period", period },
            };
            parameters.AddOptionalParameter("flags", flags);

            var query = new BitfinexSocketQuery(ExchangeHelpers.NextId().ToString(CultureInfo.InvariantCulture), BitfinexEventType.FundingOfferNew, parameters);
            var bitfinexQuery = new BitfinexQuery<BitfinexFundingOffer>(query);
            var result = await QueryAsync(BaseAddress.AppendPath("ws/2"), bitfinexQuery).ConfigureAwait(false);
            return result.As<BitfinexFundingOffer>(result.Data?.Data.Data);
        }

        /// <inheritdoc />
        public async Task<CallResult<BitfinexFundingOffer>> CancelFundingOfferAsync(long id)
        {
            var parameters = new Dictionary<string, object>
            {
                { "id", id }
            };

            var query = new BitfinexSocketQuery(id.ToString(CultureInfo.InvariantCulture), BitfinexEventType.FundingOfferCancel, parameters);
            var bitfinexQuery = new BitfinexQuery<BitfinexFundingOffer>(query);
            var result = await QueryAsync(BaseAddress.AppendPath("ws/2"), bitfinexQuery).ConfigureAwait(false);
            return result.As<BitfinexFundingOffer>(result.Data?.Data.Data);
        }

        /// <inheritdoc />
        public override string? GetListenerIdentifier(IMessageAccessor message)
        {
            var type = message.GetNodeType();
            if (type == NodeType.Array)
                return message.GetValue<int>(_0Path).ToString();

            var evnt = message.GetValue<string>(_eventPath);
            if (string.Equals(evnt, "info", StringComparison.Ordinal))
                return "info";

            var channel = message.GetValue<string>(_channelPath);
            var symbol = message.GetValue<string>(_symbolPath);
            var prec = message.GetValue<string>(_precPath);
            var freq = message.GetValue<string>(_freqPath);
            var len = message.GetValue<string>(_lenPath);
            var key = message.GetValue<string>(_keyPath);
            var chanId = string.Equals(evnt, "unsubscribed", StringComparison.Ordinal) ? message.GetValue<string>(_chanIdPath) : "";
            return chanId + evnt + channel + symbol + prec + freq + len + key;
        }

        private long GenerateClientOrderId()
        {
            var buffer = new byte[8];
            _random.NextBytes(buffer);
            return (long)Math.Round(Math.Abs(BitConverter.ToInt32(buffer, 0)) / 1000m);
        }
    }
}
