using Bitfinex.Net.Converters;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.SocketObjects;
using Bitfinex.Net.Objects.SocketV2;
using CryptoExchange.Net;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitfinex.Net
{
    public class BitfinexSocketClient2 : SocketClient
    {
        private static BitfinexSocketClientOptions defaultOptions = new BitfinexSocketClientOptions();
        private static BitfinexSocketClientOptions DefaultOptions => defaultOptions.Copy();

        private static readonly object nonceLock = new object();
        private static long lastNonce;
        internal static string Nonce
        {
            get
            {
                lock (nonceLock)
                {
                    if (lastNonce == 0)
                        lastNonce = (long)Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds * 1000);

                    lastNonce += 1;
                    return lastNonce.ToString(CultureInfo.InvariantCulture);
                }
            }
        }
        #region ctor
        /// <summary>
        /// Create a new instance of BinanceClient using the default options
        /// </summary>
        public BitfinexSocketClient2() : this(DefaultOptions)
        {
        }

        /// <summary>
        /// Create a new instance of BinanceClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public BitfinexSocketClient2(BitfinexSocketClientOptions options) : base(options, options.ApiCredentials == null ? null : new BitfinexAuthenticationProvider(options.ApiCredentials))
        {
            Configure(options);
        }
        #endregion

        public CallResult<UpdateSubscription> SubscribeToTickerUpdates(string symbol, Action<BitfinexMarketOverview> handler) => SubscribeToTickerUpdatesAsync(symbol, handler).Result;
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<BitfinexMarketOverview> handler)
        {
            return await Subscribe(new BitfinexSubscriptionRequest("ticker", symbol), handler);
        }

        public CallResult<UpdateSubscription> SubscribeToRawBookUpdates(string symbol, int limit, Action<BitfinexRawOrderBookEntry[]> handler) => SubscribeToRawBookUpdatesAsync(symbol, limit, handler).Result;
        public async Task<CallResult<UpdateSubscription>> SubscribeToRawBookUpdatesAsync(string symbol, int limit, Action<BitfinexRawOrderBookEntry[]> handler)
        {
            return await Subscribe(new BitfinexBookSubscriptionRequest(symbol, "R0", limit), handler);
        }

        public CallResult<UpdateSubscription> SubscribeToOrderUpdates(Action<BitfinexSocketEvent<BitfinexOrder[]>> handler) => SubscribeToOrderUpdatesAsync(handler).Result;
        public async Task<CallResult<UpdateSubscription>> SubscribeToOrderUpdatesAsync(Action<BitfinexSocketEvent<BitfinexOrder[]>> handler)
        {
            return await SubscribeAuth("trading", new[] {
                BitfinexEventType.OrderCancel,
                BitfinexEventType.OrderNew,
                BitfinexEventType.OrderSnapshot,
                BitfinexEventType.OrderUpdate
            }, handler);
        }

        private async Task<CallResult<UpdateSubscription>> Subscribe<T>(BitfinexSubscriptionRequest request, Action<T> onData)
        {
            var connectResult = await CreateAndConnectSocket(onData);
            if (!connectResult.Success)
                return new CallResult<UpdateSubscription>(null, connectResult.Error);

            return await Subscribe(connectResult.Data, request);
        }

        private async Task<CallResult<UpdateSubscription>> Subscribe(SocketSubscription subscription, BitfinexSubscriptionRequest request)
        {
            var waitTask = subscription.WaitForEvent("Subscription", 5000);
            Send(subscription.Socket, request);

            var subResult = await waitTask;
            if (!subResult.Success)
            {
                await subscription.Close();
                return new CallResult<UpdateSubscription>(null, subResult.Error);
            }

            subscription.Request = request;
            subscription.Socket.ShouldReconnect = true;
            return new CallResult<UpdateSubscription>(new UpdateSubscription(subscription), null);
        }

        private async Task<CallResult<UpdateSubscription>> SubscribeAuth<T>(string filter, BitfinexEventType[] eventTypes, Action<BitfinexSocketEvent<T>> onData)
        {
            var connectResult = await CreateAndConnectSocketAuth(onData);
            if (!connectResult.Success)
                return new CallResult<UpdateSubscription>(null, connectResult.Error);

            connectResult.Data.EventTypes = eventTypes;
            var authObject = GetAuthObject(filter);
            return await SubscribeAuth(authObject, connectResult.Data);
        }

        private async Task<CallResult<UpdateSubscription>> SubscribeAuth(BitfinexAuthentication auth, SocketSubscription subscription)
        {
            var waitTask = subscription.WaitForEvent("Authentication", 5000);
            Send(subscription.Socket, auth);

            var subResult = await waitTask;
            if (!subResult.Success)
            {
                await subscription.Close();
                return new CallResult<UpdateSubscription>(null, subResult.Error);
            }

            subscription.Request = auth;
            subscription.Socket.ShouldReconnect = true;
            return new CallResult<UpdateSubscription>(new UpdateSubscription(subscription), null);
        }

        private async Task<CallResult<BitfinexSocketSubscription>> CreateAndConnectSocket<T>(Action<T> onMessage)
        {
            var socket = CreateSocket(baseAddress);
            var subscription = new BitfinexSocketSubscription(socket);
            subscription.MessageHandlers.Add(HeartbeatHandler);
            subscription.MessageHandlers.Add((subs, data) => DataHandler(subs, data, onMessage));
            subscription.MessageHandlers.Add(SubscriptionHandler);
            subscription.AddEvent("Subscription");
            subscription.MessageHandlers.Add(InfoHandler);

            var connectResult = await ConnectSocket(subscription);
            if (!connectResult.Success)
                return new CallResult<BitfinexSocketSubscription>(null, connectResult.Error);

            return new CallResult<BitfinexSocketSubscription>(subscription, null);
        }

        private async Task<CallResult<BitfinexSocketSubscription>> CreateAndConnectSocketAuth<T>(Action<BitfinexSocketEvent<T>> onMessage)
        {
            var socket = CreateSocket(baseAddress);
            var subscription = new BitfinexSocketSubscription(socket);
            subscription.MessageHandlers.Add(HeartbeatHandler);
            subscription.MessageHandlers.Add((subs, data) => DataHandlerAuth((BitfinexSocketSubscription)subs, data, onMessage));
            subscription.MessageHandlers.Add(AuthenticationHandler);
            subscription.AddEvent("Authentication");
            subscription.MessageHandlers.Add(InfoHandler);

            var connectResult = await ConnectSocket(subscription);
            if (!connectResult.Success)
                return new CallResult<BitfinexSocketSubscription>(null, connectResult.Error);

            return new CallResult<BitfinexSocketSubscription>(subscription, null);
        }


        private bool InfoHandler(SocketSubscription subscription, JToken data)
        {
            var infoEvent = data.Type == JTokenType.Object && (string)data["event"] == "info";
            if (!infoEvent)
                return false;

            log.Write(LogVerbosity.Debug, $"Info event received: {data}");
            return true;
        }

        private bool HeartbeatHandler(SocketSubscription subscription, JToken data)
        {
            return data.Type == JTokenType.Array && data[1].Type == JTokenType.String && (string)data[1] == "hb";
        }

        private bool AuthenticationHandler(SocketSubscription subscription, JToken data)
        {
            if (data.Type != JTokenType.Object)
                return false;

            var authEvent = (string)data["event"] == "auth";
            if (!authEvent)
                return false;

            var authResponse = Deserialize<BitfinexAuthenticationResponse>(data, false);
            if (!authResponse.Success)
            {
                log.Write(LogVerbosity.Warning, $"Authentication failed: " + authResponse.Error);
                subscription.SetEvent("Authentication", false, authResponse.Error);
                return true;
            }

            if (authResponse.Data.Status != "OK")
            {
                var error = new ServerError(authResponse.Data.ErrorCode, authResponse.Data.ErrorMessage);
                log.Write(LogVerbosity.Debug, $"Authentication failed: " + error);
                subscription.SetEvent("Authentication", false, error);
                return true;
            }

            log.Write(LogVerbosity.Debug, $"Authentication completed");
            subscription.SetEvent("Authentication", true, null);
            return true;
        }

        private bool SubscriptionHandler(SocketSubscription subscription, JToken data)
        {
            if (data.Type != JTokenType.Object)
                return false;

            var infoEvent = (string)data["event"] == "subscribed";
            var errorEvent = (string)data["event"] == "error";
            if (!infoEvent && !errorEvent)
                return false;

            var evnt = subscription.GetWaitingEvent("Subscription");
            if (evnt == null)
                return false;

            if (infoEvent)
            {
                var subResponse = Deserialize<BitfinexSubscribeResponse>(data, false);
                if (!subResponse.Success)
                {
                    log.Write(LogVerbosity.Warning, $"Subscription failed: " + subResponse.Error);
                    subscription.SetEvent("Subscription", false, subResponse.Error);
                    return true;
                }

                log.Write(LogVerbosity.Debug, $"Subscription completed");
                subscription.SetEvent("Subscription", true, null);
                return true;
            }
            else
            {
                var subResponse = Deserialize<BitfinexErrorResponse>(data, false);
                if (!subResponse.Success)
                {
                    log.Write(LogVerbosity.Warning, $"Subscription failed: " + subResponse.Error);
                    subscription.SetEvent("Subscription", false, subResponse.Error);
                    return true;
                }

                var error = new ServerError(subResponse.Data.Code, subResponse.Data.Message);
                log.Write(LogVerbosity.Debug, $"Subscription failed: " + error);
                subscription.SetEvent("Subscription", false, error);
                return true;
            }
        }

        private bool DataHandler<T>(SocketSubscription subscription, JToken data, Action<T> handler)
        {
            var dataObject = data.Type == JTokenType.Array;
            if (!dataObject)
                return false;

            var desResult = Deserialize<BitfinexChannelUpdate<T>>(data, false);
            if (!desResult.Success)
            {
                log.Write(LogVerbosity.Warning, $"Failed to deserialize data: {desResult.Error}. Data: {data}");
                return true;
            }

            handler(desResult.Data.Data);
            return true;
        }

        private bool DataHandlerAuth<T>(BitfinexSocketSubscription subscription, JToken data, Action<BitfinexSocketEvent<T>> handler)
        {
            var dataObject = data.Type == JTokenType.Array;
            if (!dataObject)
                return false;

            var eventTypeString = (string)data[1];
            var eventType = BitfinexEvents.EventMapping[eventTypeString];
            if (!subscription.EventTypes.Contains(eventType))
                return false;

            var desResult = Deserialize<BitfinexSocketEvent<T>>(data, false);
            if (!desResult.Success)
            {
                log.Write(LogVerbosity.Warning, $"Failed to deserialize data: {desResult.Error}. Data: {data}");
                return true;
            }

            desResult.Data.EventType = eventType;
            handler(desResult.Data);
            return true;
        }

        private BitfinexAuthentication GetAuthObject(params string[] filter)
        {
            var n = Nonce;
            var authentication = new BitfinexAuthentication()
            {
                Event = "auth",
                ApiKey = authProvider.Credentials.Key.GetString(),
                Nonce = n,
                Payload = "AUTH" + n,
                Filter = filter
            };
            authentication.Signature = authProvider.Sign(authentication.Payload).ToLower();
            return authentication;
        }

        protected override bool SocketReconnect(SocketSubscription subscription, TimeSpan disconnectedTime)
        {
            if (subscription.Request is BitfinexAuthentication)
            {
                var resubResult = SubscribeAuth((BitfinexAuthentication)subscription.Request, subscription).Result;
                if (!resubResult.Success)
                    return false;
            }
            else
            {
                var resubResult = Subscribe(subscription, (BitfinexSubscriptionRequest)subscription.Request).Result;
                if (!resubResult.Success)
                    return false;
            }
            return true;
        }
    }
}
