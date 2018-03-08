using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Converters;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.SocketObjects;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Net
{
    public class BitfinexSocketClient: ExchangeClient
    {
        #region fields
        private static BitfinexSocketClientOptions defaultOptions = new BitfinexSocketClientOptions();

        private string baseAddress;
        private IWebsocket socket;

        private AutoResetEvent messageEvent;
        private AutoResetEvent sendEvent;
        private ConcurrentQueue<string> receivedMessages;
        private ConcurrentQueue<string> toSendMessages;

        private Dictionary<string, Type> subscriptionResponseTypes;

        private Dictionary<BitfinexNewOrder, WaitAction<BitfinexOrder>> pendingOrders;
        private Dictionary<long, WaitAction> pendingCancels;

        private List<SubscriptionRequest> outstandingSubscriptionRequests;
        private List<SubscriptionRequest> confirmedRequests;
        private List<UnsubscriptionRequest> outstandingUnsubscriptionRequests;
        private List<SubscriptionRegistration> registrations;

        private bool running;
        private readonly object connectionLock = new object();
        private static readonly object streamIdLock = new object();
        private static readonly object nonceLock = new object();
        private static int lastStreamId;
        private static long lastNonce;
        
        private bool authenticated;

        private static string Nonce
        {
            get
            {
                lock (nonceLock)
                {
                    if (lastNonce == 0)
                        lastNonce = (long)Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds * 10);

                    lastNonce += 1;
                    return lastNonce.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        private static int NextStreamId
        {
            get
            {
                lock (streamIdLock)
                {
                    lastStreamId -= 1;
                    return lastStreamId;
                }
            }
        }
        #endregion

        #region properties
        public IWebsocketFactory SocketFactory { get; set; } = new WebsocketFactory();
        #endregion

        #region ctor
        /// <summary>
        /// Create a new instance of BinanceClient using the default options
        /// </summary>
        public BitfinexSocketClient(): this(defaultOptions)
        {
        }

        /// <summary>
        /// Create a new instance of BinanceClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public BitfinexSocketClient(BitfinexSocketClientOptions options): base(options, options.ApiCredentials == null ? null : new BitfinexAuthenticationProvider(options.ApiCredentials))
        {
            Init();
            Configure(options);
        }
        #endregion

        #region methods
        /// <summary>
        /// Sets the default options to use for new clients
        /// </summary>
        /// <param name="options">The options to use for new clients</param>
        public static void SetDefaultOptions(BitfinexSocketClientOptions options)
        {
            defaultOptions = options;
        }

        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        public void SetApiCredentials(string apiKey, string apiSecret)
        {
            SetAuthenticationProvider(new BitfinexAuthenticationProvider(new ApiCredentials(apiKey, apiSecret)));
        }

        /// <summary>
        /// Connect to the websocket and start processing data
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            lock (connectionLock)
            {
                if (CheckConnection())
                    return true;

                log.Write(LogVerbosity.Info, "Starting the socket");
                if (socket == null)
                    Create();

                return Open().Result;
            }
        }

        /// <summary>
        /// Disconnect from the socket and clear all subscriptions
        /// </summary>
        public void Stop()
        {
            lock (connectionLock)
            {
                log.Write(LogVerbosity.Info, "Stopping the socket");
                socket.Close();
            }

            running = false;
            messageEvent.Set();
            sendEvent.Set();

            Init();
        }

        /// <summary>
        /// Synchronized version of the <see cref="PlaceOrderAsync"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<BitfinexOrder> PlaceOrder(OrderType type, string symbol, decimal amount, int? groupId = null, int? clientOrderId = null, decimal? price = null, decimal? priceTrailing = null, decimal? priceAuxiliaryLimit = null, decimal? priceOcoStop = null, OrderFlags? flags = null) => PlaceOrderAsync(type, symbol, amount, groupId, clientOrderId, price, priceTrailing, priceAuxiliaryLimit, priceOcoStop, flags).Result;

        /// <summary>
        /// Places a new order
        /// </summary>
        /// <param name="type">The type of the order</param>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="amount">The amount of the order, positive for buying, negative for selling</param>
        /// <param name="groupId">Group id to assign to the order</param>
        /// <param name="clientOrderId">Client order id to assign to the order</param>
        /// <param name="price">Price of the order</param>
        /// <param name="priceTrailing">Trailing price of the order</param>
        /// <param name="priceAuxiliaryLimit">Auxiliary limit price of the order</param>
        /// <param name="priceOcoStop">Oco stop price of ther order</param>
        /// <param name="flags">Additional flags</param>
        /// <returns></returns>
        public async Task<CallResult<BitfinexOrder>> PlaceOrderAsync(OrderType type, string symbol, decimal amount, int? groupId = null, int? clientOrderId = null, decimal? price = null, decimal? priceTrailing = null, decimal? priceAuxiliaryLimit = null, decimal? priceOcoStop = null, OrderFlags? flags = null)
        {
            if (!CheckConnection())
                return new CallResult<BitfinexOrder>(null, new WebError("Socket needs to be started before placing an order"));

            if (!authenticated)
                return new CallResult<BitfinexOrder>(null, new NoApiCredentialsError());
            
            log.Write(LogVerbosity.Info, "Going to place order");
            var order = new BitfinexNewOrder()
            {
                Amount = amount,
                OrderType = type,
                Symbol = symbol,
                Price = price,
                ClientOrderId = clientOrderId,
                Flags = flags,
                GroupId = groupId,
                PriceAuxiliaryLimit = priceAuxiliaryLimit,
                PriceOCOStop = priceOcoStop,
                PriceTrailing = priceTrailing
            };

            var wrapper = new object[] { 0, "on", null, order };
            var data = JsonConvert.SerializeObject(wrapper, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Culture = CultureInfo.InvariantCulture
            });

            BitfinexOrder orderConfirm = null;
            await Task.Run(() =>
            {
                var waitAction = new WaitAction<BitfinexOrder>();
                pendingOrders.Add(order, waitAction);
                Send(data);
                orderConfirm = waitAction.Wait(20000);
                pendingOrders.Remove(order);
            }).ConfigureAwait(false);

            if (orderConfirm != null)
                log.Write(LogVerbosity.Info, "Order canceled");
            
            return new CallResult<BitfinexOrder>(orderConfirm, orderConfirm != null ? null : new ServerError("No confirmation received for placed order"));
        }

        /// <summary>
        /// Synchronized version of the <see cref="CancelOrderAsync"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<bool> CancelOrder(long orderId) => CancelOrderAsync(orderId).Result;

        /// <summary>
        /// Cancels an order
        /// </summary>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <returns></returns>
        public async Task<CallResult<bool>> CancelOrderAsync(long orderId)
        {
            if (!CheckConnection())
                return new CallResult<bool>(false, new WebError("Socket needs to be started before canceling an order"));

            if (!authenticated)
                return new CallResult<bool>(false, new NoApiCredentialsError());

            log.Write(LogVerbosity.Info, "Going to cancel order " + orderId);
            var obj = new JObject {["id"] = orderId};
            var wrapper = new JArray(0, "oc", null, obj);
            var data = JsonConvert.SerializeObject(wrapper);

            bool done = false;
            await Task.Run(() =>
            {
                var waitAction = new WaitAction();
                pendingCancels.Add(orderId, waitAction);
                Send(data);
                done = waitAction.Wait(20000);
                pendingCancels.Remove(orderId);
            }).ConfigureAwait(false);

            if (done)
                log.Write(LogVerbosity.Info, "Order canceled");
            
            return new CallResult<bool>(done, done ? null : new ServerError("No confirmation received for canceling order"));
        }

        #region subscribing
        /// <summary>
        /// Subscribes to wallet updates. A snapshot will be send when opening the socket so consider subscribing to this before opening the socket.
        /// </summary>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        public CallResult<int> SubscribeToWalletUpdates(Action<BitfinexWallet[]> handler)
        {
            if (authProvider == null)
                return new CallResult<int>(0, new NoApiCredentialsError());

            log.Write(LogVerbosity.Debug, "Subscribing to wallet updates");
            var id = NextStreamId;
            registrations.Add(new WalletUpdateRegistration(handler, id));
            return new CallResult<int>(id, null);
        }

        /// <summary>
        /// Subscribes to order updates. A snapshot will be send when opening the socket so consider subscribing to this before opening the socket.
        /// </summary>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        public CallResult<int> SubscribeToOrderUpdates(Action<BitfinexOrder[]> handler)
        {
            if (authProvider == null)
                return new CallResult<int>(0, new NoApiCredentialsError());

            log.Write(LogVerbosity.Debug, "Subscribing to order updates");
            var id = NextStreamId;
            registrations.Add(new OrderUpdateRegistration(handler, id));
            return new CallResult<int>(id, null);
        }

        /// <summary>
        /// Subscribes to position updates. A snapshot will be send when opening the socket so consider subscribing to this before opening the socket.
        /// </summary>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        public CallResult<int> SubscribeToPositionUpdates(Action<BitfinexPosition[]> handler)
        {
            if (authProvider == null)
                return new CallResult<int>(0, new NoApiCredentialsError());

            log.Write(LogVerbosity.Debug, "Subscribing to position updates");
            var id = NextStreamId;
            registrations.Add(new PositionUpdateRegistration(handler, id));
            return new CallResult<int>(id, null);
        }

        /// <summary>
        /// Subscribes to trade updates. A snapshot will be send when opening the socket so consider subscribing to this before opening the socket.
        /// </summary>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        public CallResult<int> SubscribeToTradeUpdates(Action<BitfinexTradeDetails[]> handler)
        {
            if (authProvider == null)
                return new CallResult<int>(0, new NoApiCredentialsError());

            log.Write(LogVerbosity.Debug, "Subscribing to trade updates");
            var id = NextStreamId;
            registrations.Add(new TradesUpdateRegistration(handler, id));
            return new CallResult<int>(id, null);
        }

        /// <summary>
        /// Subscribes to funding offer updates. A snapshot will be send when opening the socket so consider subscribing to this before opening the socket.
        /// </summary>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        public CallResult<int> SubscribeToFundingOfferUpdates(Action<BitfinexFundingOffer[]> handler)
        {
            if (authProvider == null)
                return new CallResult<int>(0, new NoApiCredentialsError());

            log.Write(LogVerbosity.Debug, "Subscribing to funding offer updates");
            var id = NextStreamId;
            registrations.Add(new FundingOffersUpdateRegistration(handler, id));
            return new CallResult<int>(id, null);
        }

        /// <summary>
        /// Subscribes to funding credits updates. A snapshot will be send when opening the socket so consider subscribing to this before opening the socket.
        /// </summary>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        public CallResult<int> SubscribeToFundingCreditsUpdates(Action<BitfinexFundingCredit[]> handler)
        {
            if (authProvider == null)
                return new CallResult<int>(0, new NoApiCredentialsError());

            log.Write(LogVerbosity.Debug, "Subscribing to funding credit updates");
            var id = NextStreamId;
            registrations.Add(new FundingCreditsUpdateRegistration(handler, id));
            return new CallResult<int>(id, null);
        }

        /// <summary>
        /// Subscribes to funding loans updates. A snapshot will be send when opening the socket so consider subscribing to this before opening the socket.
        /// </summary>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        public CallResult<int> SubscribeToFundingLoansUpdates(Action<BitfinexFundingLoan[]> handler)
        {
            if (authProvider == null)
                return new CallResult<int>(0, new NoApiCredentialsError());

            log.Write(LogVerbosity.Debug, "Subscribing to funding loan updates");
            var id = NextStreamId;
            registrations.Add(new FundingLoansUpdateRegistration(handler, id));
            return new CallResult<int>(id, null);
        }

        /// <summary>
        /// Subscribes to ticker updates for a symbol. Requires socket to be connected
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        public async Task<CallResult<int>> SubscribeToTicker(string symbol, Action<BitfinexMarketOverview[]> handler)
        {
            log.Write(LogVerbosity.Debug, "Subscribing to ticker updates for "+ symbol);
            return await SubscribeAndWait(new TickerSubscriptionRequest(symbol, handler)).ConfigureAwait(false); 
        }

        /// <summary>
        /// Subscribes to trade updates for a symbol. Requires socket to be connected
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        public async Task<CallResult<int>> SubscribeToTrades(string symbol, Action<BitfinexTradeSimple[]> handler)
        {
            log.Write(LogVerbosity.Debug, "Subscribing to trade updates for "+ symbol);
            return await SubscribeAndWait(new TradesSubscriptionRequest(symbol, handler)).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribes to orderbook update for a symbol. Requires socket to be connected
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="precision">The pricision of the udpates</param>
        /// <param name="frequency">The frequency of updates</param>
        /// <param name="length">The amount of data to recive in updates</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        public async Task<CallResult<int>> SubscribeToBook(string symbol, Precision precision, Frequency frequency, int length, Action<BitfinexOrderBookEntry[]> handler)
        {
            log.Write(LogVerbosity.Debug, "Subscribing to book updates for "+ symbol);
            return await SubscribeAndWait(new BookSubscriptionRequest(symbol, JsonConvert.SerializeObject(precision, new PrecisionConverter(false)), JsonConvert.SerializeObject(frequency, new FrequencyConverter(false)), length, handler)).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribes to candle updates for a symbol. Requires socket to be connected
        /// </summary>
        /// <param name="symbol">The symbol to subscribe to</param>
        /// <param name="interval">The interval of the candles</param>
        /// <param name="handler">The handler for the data</param>
        /// <returns>A stream id with which can be unsubscribed</returns>
        public async Task<CallResult<int>> SubscribeToCandles(string symbol, TimeFrame interval, Action<BitfinexCandle[]> handler)
        {
            log.Write(LogVerbosity.Debug, "Subscribing to candle updates for "+ symbol);
            return await SubscribeAndWait(new CandleSubscriptionRequest(symbol, JsonConvert.SerializeObject(interval, new TimeFrameConverter(false)), handler)).ConfigureAwait(false);
        }

        /// <summary>
        /// Unsubscribe from a specific channel using the id acquired when subscribing
        /// </summary>
        /// <param name="channelId">The channel id to unsubscribe from</param>
        /// <returns></returns>
        public async Task<CallResult<bool>> UnsubscribeFromChannel(int channelId)
        {
            log.Write(LogVerbosity.Debug, "Unsubscribing from channel " + channelId);
            if (confirmedRequests.Any(r => r.ChannelId == channelId))
            {
                var result = await UnsubscribeAndWait(new UnsubscriptionRequest(channelId)).ConfigureAwait(false);
                if (!result.Success)
                    return result;
            }
            else if (registrations.Any(r => r.StreamId == channelId))
            {
                registrations.Remove(registrations.Single(r => r.StreamId == channelId));
            }
            else
            {
                log.Write(LogVerbosity.Warning, "No subscription found for channel id " + channelId);
                return new CallResult<bool>(false, new ArgumentError("No subscription found for channel id " + channelId));
            }
            
            return new CallResult<bool>(true, null);
        }
        #endregion
        #endregion

        #region private
        private void Create()
        {
            socket = SocketFactory.CreateWebsocket(baseAddress);
            socket.OnClose += SocketClosed;
            socket.OnError += SocketError;
            socket.OnOpen += SocketOpened;
            socket.OnMessage += SocketMessage;
        }

        private async Task<bool> Open()
        {
            bool connectResult = await socket.Connect().ConfigureAwait(false); 
            if (!connectResult)
            {
                log.Write(LogVerbosity.Warning, "Couldn't connect to socket");
                return false;
            }

            running = true;
#pragma warning disable 4014
            Task.Run(() => ProcessData());
            Task.Run(() => ProcessSending());
#pragma warning restore 4014
            log.Write(LogVerbosity.Info, "Socket connection established");
            return true;
        }

        private async Task<CallResult<int>> SubscribeAndWait(SubscriptionRequest request)
        {
            if (!CheckConnection())
                return new CallResult<int>(0, new WebError("Socket needs to be started before subscribing to this"));

            outstandingSubscriptionRequests.Add(request);
            Send(JsonConvert.SerializeObject(request));
            bool confirmed = false;
            await Task.Run(() =>
            {
                confirmed = request.ConfirmedEvent.WaitOne(2000);
                outstandingSubscriptionRequests.Remove(request);
                confirmedRequests.Add(request);
                log.Write(LogVerbosity.Debug, !confirmed ? "No confirmation received" : "Subscription confirmed");
            }).ConfigureAwait(false);

            return new CallResult<int>(confirmed ? request.ChannelId : 0, confirmed ? null : new ServerError("No confirmation received"));
        }

        private async Task<CallResult<bool>> UnsubscribeAndWait(UnsubscriptionRequest request)
        {
            if (!CheckConnection())
                return new CallResult<bool>(false, new WebError("Can't unsubscribe when not connected"));

            outstandingUnsubscriptionRequests.Add(request);
            Send(JsonConvert.SerializeObject(request));
            bool confirmed = false;
            await Task.Run(() =>
            {
                confirmed = request.ConfirmedEvent.WaitOne(2000);
                outstandingUnsubscriptionRequests.Remove(request);
                confirmedRequests.RemoveAll(r => r.ChannelId == request.ChannelId);
                log.Write(LogVerbosity.Debug, !confirmed ? "No confirmation received" : "Subscription confirmed");
            }).ConfigureAwait(false);

            return new CallResult<bool>(confirmed, confirmed ? null : new ServerError("No confirmation received"));
        }

        private void SocketClosed()
        {
            log.Write(LogVerbosity.Debug, "Socket closed");
        }

        private void SocketError(Exception ex)
        {
            log.Write(LogVerbosity.Error, $"Socket error: {ex?.GetType().Name} - {ex?.Message}");
        }

        private void SocketOpened()
        {
            log.Write(LogVerbosity.Debug, "Socket opened");
            Authenticate();
        }

        private void SocketMessage(string msg)
        {
            log.Write(LogVerbosity.Debug, "Received message: " + msg);
            receivedMessages.Enqueue(msg);
            messageEvent.Set();
        }

        private void Send(string data)
        {
            toSendMessages.Enqueue(data);
            sendEvent.Set();
        }

        private void ProcessSending()
        {
            while (running)
            {
                sendEvent.WaitOne();
                if (!running)
                    break;

                while (toSendMessages.TryDequeue(out string data))
                {
                    log.Write(LogVerbosity.Debug, "Sending " + data);
                    socket.Send(data);
                }
            }
        }

        private void ProcessData()
        {
            while (running)
            {
                messageEvent.WaitOne();
                if (!running)
                    break;

                while (receivedMessages.TryDequeue(out string dequeued))
                {
                    log.Write(LogVerbosity.Debug, "Processing " + dequeued);

                    try
                    {
                        var dataObject = JToken.Parse(dequeued);
                        if (dataObject is JObject)
                        {
                            var evnt = dataObject["event"].ToString();
                            if (evnt == "auth")
                            {
                                ProcessAuthenticationResponse(dataObject.ToObject<BitfinexAuthenticationResponse>());
                            }
                            else if (evnt == "subscribed")
                            {
                                var channel = dataObject["channel"].ToString();
                                if (!subscriptionResponseTypes.ContainsKey(channel))
                                {
                                    log.Write(LogVerbosity.Warning, "Unknown response channel name: " + channel);
                                    continue;
                                }

                                SubscriptionResponse subResponse = (SubscriptionResponse)dataObject.ToObject(subscriptionResponseTypes[channel]);
                                var pending = outstandingSubscriptionRequests.SingleOrDefault(r => r.GetSubscriptionKey() == subResponse.GetSubscriptionKey());
                                if (pending == null)
                                {
                                    log.Write(LogVerbosity.Debug, "Couldn't find sub request for response");
                                    continue;
                                }

                                pending.ChannelId = subResponse.ChannelId;
                                pending.ConfirmedEvent.Set();
                            }
                            else if (evnt == "unsubscribed")
                            {
                                var pending = outstandingUnsubscriptionRequests.SingleOrDefault(r => r.ChannelId == (int)dataObject["chanId"]);
                                if (pending == null)
                                {
                                    log.Write(LogVerbosity.Debug, "Received unsub confirmation, but no pending unsubscriptions");
                                    continue;
                                }
                                pending.ConfirmedEvent.Set();
                            }
                        }
                        else
                        {
                            var channelId = (int)dataObject[0];
                            if (dataObject[1].ToString() == "hb")
                                continue;

                            var channelReg = confirmedRequests.SingleOrDefault(c => c.ChannelId == channelId);
                            if (channelReg != null)
                            {
                                channelReg.Handle((JArray)dataObject);
                                continue;
                            }

                            var messageType = dataObject[1].ToString();
                            HandleRequestResponse(messageType, (JArray)dataObject);
                            var accountReg = registrations.SingleOrDefault(r => r.UpdateKeys.Contains(messageType));
                            accountReg?.Handle((JArray)dataObject);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Write(LogVerbosity.Error, $"Error in processing loop. {e.GetType()}, {e.Message}, {e.StackTrace}, message: {dequeued}");
                    }
                }
            }
        }

        private void HandleRequestResponse(string messageType, JArray dataObject)
        {
            if (messageType == "on")
            {
                var orderResult = Deserialize<BitfinexOrder>(dataObject[2].ToString());
                if (!orderResult.Success)
                {
                    log.Write(LogVerbosity.Warning, "Failed to deserialize new order from stream: " + orderResult.Error);
                    return;
                }

                foreach (var pendingOrder in pendingOrders.ToList())
                {
                    var o = pendingOrder.Key;
                    if (o.Symbol == orderResult.Data.Symbol && o.Amount == orderResult.Data.AmountOriginal)
                    {
                        pendingOrder.Value.Set(orderResult.Data);
                        break;
                    }
                }
            }
            else if (messageType == "oc")
            {
                var orderResult = Deserialize<BitfinexOrder>(dataObject[2].ToString());
                if (!orderResult.Success)
                {
                    log.Write(LogVerbosity.Warning, "Failed to deserialize new order from stream: " + orderResult.Error);
                    return;
                }

                foreach (var pendingCancel in pendingCancels.ToList())
                {
                    var o = pendingCancel.Key;
                    if (o == orderResult.Data.Id)
                    {
                        pendingCancel.Value.Set();
                        break;
                    }
                }
            }
        }

        private void Authenticate()
        {
            if (authProvider == null)
                return;

            var n = Nonce;
            var authentication = new BitfinexAuthentication()
            {
                Event = "auth",
                ApiKey = authProvider.Credentials.Key,
                Nonce = n,
                Payload = "AUTH" + n
            };
            authentication.Signature = authProvider.Sign(authentication.Payload).ToLower();

            Send(JsonConvert.SerializeObject(authentication));
        }

        private void ProcessAuthenticationResponse(BitfinexAuthenticationResponse response)
        {
            if (response.Status == "OK")
            {
                authenticated = true;
                log.Write(LogVerbosity.Info, "Authentication successful");
            }
            else
            {
                authenticated = false;
                log.Write(LogVerbosity.Warning, "Authentication failed: " + response.ErrorMessage);
            }
        }

        private void GetSubscriptionResponseTypes()
        {
            subscriptionResponseTypes = new Dictionary<string, Type>();
            foreach (var t in typeof(SubscriptionResponse).Assembly.GetTypes())
            {
                if (typeof(SubscriptionResponse).IsAssignableFrom(t) && t.Name != typeof(SubscriptionResponse).Name)
                {
                    var attribute = (SubscriptionChannelAttribute)t.GetCustomAttributes(typeof(SubscriptionChannelAttribute), true)[0];
                    subscriptionResponseTypes.Add(attribute.ChannelName, t);
                }
            }
        }

        private void Configure(BitfinexSocketClientOptions options)
        {
            base.Configure(options);

            baseAddress = options.BaseAddress;
        }

        private bool CheckConnection()
        {
            lock (connectionLock)
                return socket != null && !socket.IsClosed;
        }

        private void Init()
        {
            receivedMessages = new ConcurrentQueue<string>();
            toSendMessages = new ConcurrentQueue<string>();
            messageEvent = new AutoResetEvent(true);
            sendEvent = new AutoResetEvent(true);
            outstandingSubscriptionRequests = new List<SubscriptionRequest>();
            outstandingUnsubscriptionRequests = new List<UnsubscriptionRequest>();
            confirmedRequests = new List<SubscriptionRequest>();
            pendingOrders = new Dictionary<BitfinexNewOrder, WaitAction<BitfinexOrder>>();
            pendingCancels = new Dictionary<long, WaitAction>();
            registrations = new List<SubscriptionRegistration>();

            GetSubscriptionResponseTypes();
        }

        public override void Dispose()
        {
            base.Dispose();
            Stop();
        }

        #endregion
    }
}
