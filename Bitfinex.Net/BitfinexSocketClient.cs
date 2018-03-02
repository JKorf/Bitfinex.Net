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
using Bitfinex.Net.Objects.SocketObjects2;
using Bitfinex.Net.Objects.SocketObjets;
using CryptoExchange.Net;
using CryptoExchange.Net.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace Bitfinex.Net
{
    public partial class BitfinexSocketClient: ExchangeClient
    {
        private static BitfinexSocketClientOptions defaultOptions = new BitfinexSocketClientOptions();

        private const string BaseAddress = "wss://api.bitfinex.com/ws/2";
        private static WebSocket socket;

        private static AutoResetEvent messageEvent;
        private static AutoResetEvent sendEvent;
        private static ConcurrentQueue<string> receivedMessages;
        private static ConcurrentQueue<string> toSendMessages;

        private static Dictionary<string, Type> subscriptionResponseTypes;

        private static Dictionary<BitfinexNewOrder, Action<BitfinexOrder>> pendingOrders;
        private static Dictionary<long, Action<BitfinexOrder>> pendingCancels;

        private static List<SubscriptionRequest> outstandingRequests;
        private static List<SubscriptionRequest> confirmedRequests;

        private static bool running;
        private static object subscriptionLock;
        private string nonce => Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds * 10).ToString(CultureInfo.InvariantCulture);
        private bool authenticated;
        private List<SubscriptionRegistration> registrations = new List<SubscriptionRegistration>();
        

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
            receivedMessages = new ConcurrentQueue<string>();
            toSendMessages = new ConcurrentQueue<string>();
            messageEvent = new AutoResetEvent(true);
            sendEvent = new AutoResetEvent(true);
            outstandingRequests = new List<SubscriptionRequest>();
            confirmedRequests = new List<SubscriptionRequest>();
            pendingOrders = new Dictionary<BitfinexNewOrder, Action<BitfinexOrder>>();
            pendingCancels = new Dictionary<long, Action<BitfinexOrder>>();

            subscriptionResponseTypes = new Dictionary<string, Type>();
            foreach (Type t in typeof(SubscriptionResponse).Assembly.GetTypes())
            {
                if (typeof(SubscriptionResponse).IsAssignableFrom(t) && t.Name != typeof(SubscriptionResponse).Name)
                {
                    var attribute = (SubscriptionChannelAttribute)t.GetCustomAttributes(typeof(SubscriptionChannelAttribute), true)[0];
                    subscriptionResponseTypes.Add(attribute.ChannelName, t);
                }
            }

            Configure(options);
        }

        public void Start()
        {
            socket = new WebSocket(BaseAddress);
            socket.Closed += SocketClosed;
            socket.Error += SocketError;
            socket.Opened += SocketOpened;
            socket.MessageReceived += SocketMessage;

            socket.Open();

            running = true;
            Task.Run(() => ProcessData());
            Task.Run(() => ProcessSending());
        }
        
        private async Task SubscribeAndWait(SubscriptionRequest request)
        {
            outstandingRequests.Add(request);
            Send(JsonConvert.SerializeObject(request));
            await Task.Run(() =>
            {
                bool result = request.ConfirmedEvent.WaitOne(2000);
                outstandingRequests.Remove(request);
                confirmedRequests.Add(request);
                log.Write(LogVerbosity.Debug, !result ? "No confirmation received" : "Subscription confirmed");
            });
        }

        private void SocketClosed(object sender, EventArgs args)
        {
            log.Write(LogVerbosity.Debug, "Socket closed");
        }

        private void SocketError(object sender, ErrorEventArgs args)
        {
            log.Write(LogVerbosity.Error, $"Socket error: {args.Exception?.GetType().Name} - {args.Exception?.Message}");
        }

        private void SocketOpened(object sender, EventArgs args)
        {
            log.Write(LogVerbosity.Debug, $"Socket opened");
            Authenticate();
        }

        private void SocketMessage(object sender, MessageReceivedEventArgs args)
        {
            log.Write(LogVerbosity.Debug, "Received message: " + args.Message);
            receivedMessages.Enqueue(args.Message);
            messageEvent.Set();
        }

        private void Send(string data)
        {
            toSendMessages.Enqueue(data);
            sendEvent.Set();
        }

        private void ProcessSending()
        {
            while (true)
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
            while (true)
            {
                messageEvent.WaitOne();
                if (!running)
                    break;

                while (receivedMessages.TryDequeue(out string dequeued))
                {
                    log.Write(LogVerbosity.Debug, "Processing " + dequeued);

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
                            var pending = outstandingRequests.SingleOrDefault(r => r.GetSubscriptionKey() == subResponse.GetSubscriptionKey());
                            if (pending == null)
                            {
                                log.Write(LogVerbosity.Debug, "Couldn't find sub request for response");
                                continue;
                            }
                            // Do something with channelid
                            pending.ChannelId = subResponse.ChannelId;
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
                        if (accountReg != null)
                        {
                            accountReg.Handle((JArray)dataObject);
                            continue;
                        }
                    }
                }
            }

            socket.Close();
        }

        private void HandleRequestResponse(string messageType, JArray dataObject)
        {
            if (messageType == "on")
            {
                var order = dataObject[2].ToObject<BitfinexOrder>();
                foreach (var pendingOrder in pendingOrders.ToList())
                {
                    var o = pendingOrder.Key;
                    if (o.Symbol == order.Symbol && o.Amount == order.AmountOriginal)
                    {
                        pendingOrder.Value(order);
                        break;
                    }
                }
            }
            else if (messageType == "oc")
            {
                var order = dataObject[2].ToObject<BitfinexOrder>();
                foreach (var pendingCancel in pendingCancels.ToList())
                {
                    var o = pendingCancel.Key;
                    if (o == order.Id)
                    {
                        pendingCancel.Value(order);
                        break;
                    }
                }
            }
        }

        public void Stop()
        {
            running = false;
            messageEvent.Set();
            sendEvent.Set();
        }

        private void Authenticate()
        {
            if (authProvider == null)
                return;

            var n = nonce;
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
                log.Write(LogVerbosity.Debug, "Authenticated");
            }
            else
            {
                authenticated = false;
                log.Write(LogVerbosity.Warning, "Authentication failed: " + response.ErrorMessage);
            }
        }

        public CallResult<BitfinexOrder> PlaceOrder(OrderType type, string symbol, decimal amount, int? groupId = null, int? clientOrderId = null, decimal? price = null, decimal? priceTrailing = null, decimal? priceAuxiliaryLimit = null, decimal? priceOCOStop = null, OrderFlags? flags = null)
        {
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
                PriceOCOStop = priceOCOStop,
                PriceTrailing = priceTrailing
            };

            var wrapper = new object[] { 0, "on", null, order };
            var data = JsonConvert.SerializeObject(wrapper, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Culture = CultureInfo.InvariantCulture
            });
            var evnt = new ManualResetEvent(false);
            BitfinexOrder confirmedOrder = null;
            pendingOrders.Add(order, receivedOrder =>
            {
                confirmedOrder = receivedOrder;
                evnt.Set();
            });
            Send(data);
            var done = evnt.WaitOne(20000);
            pendingOrders.Remove(order);
            return new CallResult<BitfinexOrder>(confirmedOrder, done ? null: new ServerError("No confirmation received for placed order"));
        }

        public CallResult<bool> CancelOrder(long orderId)
        {
            var obj = new JObject {["id"] = orderId};
            var wrapper = new JArray(0, "oc", null, obj);
            var data = JsonConvert.SerializeObject(wrapper);

            var evnt = new ManualResetEvent(false);
            bool confirmed = false;
            pendingCancels.Add(orderId, receivedOrder =>
            {
                confirmed = true;
                evnt.Set();
            });
            Send(data);
            var done = evnt.WaitOne(20000);
            pendingCancels.Remove(orderId);
            return new CallResult<bool>(confirmed, done ? null : new ServerError("No confirmation received for canceling order"));
        }

        #region subscribing
        public void SubscribeToWalletUpdates(Action<BitfinexWallet[]> handler)
        {
            registrations.Add(new WalletUpdateRegistration(handler));
        }

        public void SubscribeToOrderUpdates(Action<BitfinexOrder[]> handler)
        {
            registrations.Add(new OrderUpdateRegistration(handler));
        }

        public void SubscribeToPositionUpdates(Action<BitfinexPosition[]> handler)
        {
            registrations.Add(new PositionUpdateRegistration(handler));
        }

        public void SubscribeToTradeUpdates(Action<BitfinexTradeDetails[]> handler)
        {
            registrations.Add(new TradesUpdateRegistration(handler));
        }

        public void SubscribeToFundingOfferUpdates(Action<BitfinexFundingOffer[]> handler)
        {
            registrations.Add(new FundingOffersUpdateRegistration(handler));
        }

        public void SubscribeToFundingCreditsUpdates(Action<BitfinexFundingCredit[]> handler)
        {
            registrations.Add(new FundingCreditsUpdateRegistration(handler));
        }

        public void SubscribeToFundingLoansUpdates(Action<BitfinexFundingLoan[]> handler)
        {
            registrations.Add(new FundingLoansUpdateRegistration(handler));
        }

        public async Task SubscribeToTicker(string symbol, Action<BitfinexMarketOverview[]> handler)
        {
            await SubscribeAndWait(new TickerSubscriptionRequest(symbol, handler));
        }

        public async Task SubscribeToTrades(string symbol, Action<BitfinexTradeSimple[]> handler)
        {
            await SubscribeAndWait(new TradesSubscriptionRequest(symbol, handler));
        }

        public async Task SubscribeToBook(string symbol, string precision, string frequency, int limit, Action<BitfinexOrderBookEntry[]> handler)
        {
            await SubscribeAndWait(new BookSubscriptionRequest(symbol, precision, frequency, limit, handler));
        }

        public async Task SubscribeToCandles(string symbol, string interval, Action<BitfinexCandle[]> handler)
        {
            await SubscribeAndWait(new CandleSubscriptionRequest(symbol, interval, handler));
        }
        #endregion
    }
}
