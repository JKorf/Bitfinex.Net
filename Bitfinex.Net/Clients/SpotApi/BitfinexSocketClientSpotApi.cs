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
using Bitfinex.Net.Objects.Sockets;
using Bitfinex.Net.Objects.Sockets.Subscriptions;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Enums;
using System.Collections.Generic;
using System.Linq;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Sockets;
using System.Globalization;
using Bitfinex.Net.Objects.Sockets.Queries;

namespace Bitfinex.Net.Clients.SpotApi
{
    /// <inheritdoc cref="IBitfinexSocketClientSpotApi" />
    public class BitfinexSocketClientSpotApi : SocketApiClient, IBitfinexSocketClientSpotApi
    {
        #region fields
        private readonly JsonSerializer _bookSerializer = new JsonSerializer();
        private readonly JsonSerializer _fundingBookSerializer = new JsonSerializer();
        private readonly Random _random = new Random();
        private readonly string? _affCode;

        /// <inheritdoc />
        public new BitfinexSocketOptions ClientOptions => (BitfinexSocketOptions)base.ClientOptions;

        /// <inheritdoc />
        public override MessageInterpreterPipeline Pipeline { get; } = new MessageInterpreterPipeline
        {
            GetStreamIdentifier = GetStreamIdentifier,
            GetTypeIdentifier = GetTypeIdentifier
        };
        #endregion

        #region ctor
        internal BitfinexSocketClientSpotApi(ILogger logger, BitfinexSocketOptions options) :
            base(logger, options.Environment.SocketAddress, options, options.SpotOptions)
        {
            ContinueOnQueryResponse = true;
            UnhandledMessageExpected = true;

            //AddGenericHandler("Conf", ConfHandler);

            AddSystemSubscription(new BitfinexInfoSubscription(_logger));

            _affCode = options.AffiliateCode;
            _bookSerializer.Converters.Add(new OrderBookEntryConverter());
            _fundingBookSerializer.Converters.Add(new OrderBookFundingEntryConverter());
        }
        #endregion
        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new BitfinexAuthenticationProvider(credentials, ClientOptions.NonceProvider ?? new BitfinexNonceProvider());

        protected override BaseQuery GetAuthenticationRequest()
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
            symbol.ValidateBitfinexTradingSymbol();

            var subscription = new BitfinexSubscription<BitfinexStreamTicker>(_logger, "ticker", symbol, x => handler(x.As(x.Data.First())));
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToFundingTickerUpdatesAsync(string symbol, Action<DataEvent<BitfinexStreamFundingTicker>> handler, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexFundingSymbol();

            var subscription = new BitfinexSubscription<BitfinexStreamFundingTicker>(_logger, "ticker", symbol, x => handler(x.As(x.Data.First())));
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, Precision precision, Frequency frequency, int length, Action<DataEvent<IEnumerable<BitfinexOrderBookEntry>>> handler, Action<DataEvent<int>>? checksumHandler = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexTradingSymbol();
            length.ValidateIntValues(nameof(length), 1, 25, 100, 250);
            if (precision == Precision.R0)
                throw new ArgumentException("Invalid precision R0, use SubscribeToRawBookUpdatesAsync instead");

            var subscription = new BitfinexSubscription<BitfinexOrderBookEntry>(_logger, "book", symbol, handler, checksumHandler, false, precision, frequency, length);
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToFundingOrderBookUpdatesAsync(string symbol, Precision precision, Frequency frequency, int length, Action<DataEvent<IEnumerable<BitfinexOrderBookFundingEntry>>> handler, Action<DataEvent<int>>? checksumHandler = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexFundingSymbol();
            length.ValidateIntValues(nameof(length), 1, 25, 100, 250);
            if (precision == Precision.R0)
                throw new ArgumentException("Invalid precision R0, use SubscribeToFundingRawOrderBookUpdatesAsync instead");

            var subscription = new BitfinexSubscription<BitfinexOrderBookFundingEntry>(_logger, "book", symbol, handler, checksumHandler, false, precision, frequency, length);
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToRawOrderBookUpdatesAsync(string symbol, int limit, Action<DataEvent<IEnumerable<BitfinexRawOrderBookEntry>>> handler, Action<DataEvent<int>>? checksumHandler = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexTradingSymbol();

            var subscription = new BitfinexSubscription<BitfinexRawOrderBookEntry>(_logger, "book", symbol, handler, checksumHandler, false, Precision.R0, Frequency.Realtime, limit);
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToRawFundingOrderBookUpdatesAsync(string symbol, int limit, Action<DataEvent<IEnumerable<BitfinexRawOrderBookFundingEntry>>> handler, Action<DataEvent<int>>? checksumHandler = null, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexFundingSymbol();

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
            var subscription = new BitfinexSubscription<BitfinexKline>(_logger, "candles", null, handler, key: $"trade:{JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false))}:" + symbol);
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
             Action<DataEvent<IEnumerable<BitfinexOrder>>> orderHandler,
             Action<DataEvent<IEnumerable<BitfinexPosition>>> positionHandler,
             Action<DataEvent<IEnumerable<BitfinexFundingOffer>>> fundingOfferHandler,
             Action<DataEvent<IEnumerable<BitfinexFundingCredit>>> fundingCreditHandler,
             Action<DataEvent<IEnumerable<BitfinexFunding>>> fundingLoanHandler,
             Action<DataEvent<IEnumerable<BitfinexWallet>>> walletHandler,
             Action<DataEvent<BitfinexBalance>> balanceHandler,
             Action<DataEvent<BitfinexTradeDetails>> tradeHandler,
             Action<DataEvent<BitfinexFundingTrade>> fundingTradeHandler,
             Action<DataEvent<BitfinexFundingInfo>> fundingInfoHandler,
             CancellationToken ct = default)
        {
            var subscription = new BitfinexUserSubscription(_logger, positionHandler, walletHandler, orderHandler, fundingOfferHandler, fundingCreditHandler, fundingLoanHandler, balanceHandler, tradeHandler, fundingTradeHandler, fundingInfoHandler);
            return await SubscribeAsync(BaseAddress.AppendPath("ws/2"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<BitfinexOrder>> PlaceOrderAsync(OrderSide side, OrderType type, string symbol, decimal quantity, long? groupId = null, long? clientOrderId = null, decimal? price = null, decimal? priceTrailing = null, decimal? priceAuxiliaryLimit = null, decimal? priceOcoStop = null, OrderFlags? flags = null, int? leverage = null, DateTime? cancelTime = null, string? affiliateCode = null)
        {
            symbol.ValidateBitfinexSymbol();
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
            return result.As(result.Data?.Data.Data);
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
            return result.As(result.Data?.Data.Data);
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
            return result.As(result.Data?.Data.Data);
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
            return result.As(result.Data?.Data.Data);
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
            return result.As(result.Data?.Data.Data);
        }

        private static string? GetStreamIdentifier(IMessageAccessor accessor)
        {
            if (!accessor.IsObject(null))
            {
                return accessor.GetArrayIntValue(null, 0).ToString();
            }

            var evnt = accessor.GetStringValue("event");
            if (evnt == "info")
                return "info";

            var channel = accessor.GetStringValue("channel");
            var symbol = accessor.GetStringValue("symbol");
            var prec = accessor.GetStringValue("prec");
            var freq = accessor.GetStringValue("freq");
            var len = accessor.GetStringValue("len");
            var key = accessor.GetStringValue("key");
            var chanId = evnt == "unsubscribed" ? accessor.GetStringValue("chanId") : "";
            return chanId + evnt + channel + symbol + prec + freq + len + key;
        }

        private static string? GetTypeIdentifier(IMessageAccessor accessor)
        {
            if (accessor.IsObject(null))
                return null;

            var topic = accessor.GetArrayStringValue(null, 1);
            var dataIndex = topic == null ? 1 : 2;
            var x = topic + "-single";
            if (accessor.IsArray(new[] { dataIndex, 0 }) || accessor.IsEmptyArray(new[] { dataIndex }))
                x = topic + "-array";

            return x;
        }

        private long GenerateClientOrderId()
        {
            var buffer = new byte[8];
            _random.NextBytes(buffer);
            return (long)Math.Round(Math.Abs(BitConverter.ToInt32(buffer, 0)) / 1000m);
        }

        //        private void InfoHandler(MessageEvent messageEvent)
        //        {
        //            var infoEvent = messageEvent.JsonData.Type == JTokenType.Object && messageEvent.JsonData["event"]?.ToString() == "info";
        //            if (!infoEvent)
        //                return;

        //            _logger.Log(LogLevel.Debug, $"Socket {messageEvent.Connection.SocketId} Info event received: {messageEvent.JsonData}");
        //            if (messageEvent.JsonData["code"] == null)
        //            {
        //                // welcome event, send a config message for receiving checsum updates for order book subscriptions
        //                messageEvent.Connection.Send(ExchangeHelpers.NextId(), new BitfinexSocketConfig { Event = "conf", Flags = 131072 }, 1);
        //                return;
        //            }

        //            var code = messageEvent.JsonData["code"]?.Value<int>();
        //            switch (code)
        //            {
        //                case 20051:
        //                    _logger.Log(LogLevel.Information, $"Socket {messageEvent.Connection.SocketId} Code {code} received, reconnecting socket");
        //                    messageEvent.Connection.PausedActivity = true; // Prevent new operations to be send
        //                    _ = messageEvent.Connection.TriggerReconnectAsync();
        //                    break;
        //                case 20060:
        //                    _logger.Log(LogLevel.Information, $"Socket {messageEvent.Connection.SocketId} Code {code} received, entering maintenance mode");
        //                    messageEvent.Connection.PausedActivity = true;
        //                    break;
        //                case 20061:
        //                    _logger.Log(LogLevel.Information, $"Socket {messageEvent.Connection.SocketId} Code {code} received, leaving maintenance mode. Reconnecting/Resubscribing socket.");
        //                    _ = messageEvent.Connection.TriggerReconnectAsync(); // Closing it via socket will automatically reconnect
        //                    break;
        //                default:
        //                    _logger.Log(LogLevel.Warning, $"Socket {messageEvent.Connection.SocketId} Unknown info code received: {code}");
        //                    break;
        //            }
        //        }
    }
}
